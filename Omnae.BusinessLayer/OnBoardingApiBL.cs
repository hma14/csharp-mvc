using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Omnae.BusinessLayer.Identity.Model;
using Serilog;
using Libs.Notification;
using Microsoft.AspNet.Identity;
using Microsoft.Azure;
using Omnae.Data;
using Omnae.Hubspot;
using Omnae.Model.Context.Model;
using Omnae.Model.Models;
using RazorEngine;
using RazorEngine.Templating;
using Serilog.Context;
using Omnae.Notification;
using Omnae.Notification.Model;
using System.ComponentModel.DataAnnotations;
using System.Transactions;

namespace Omnae.BusinessLayer
{
    public class OnBoardingApiBL
    {
        private readonly ILogger Log;
        private readonly IEmailSender _emailSender;
        private readonly OnBoardingBL OnBoardingBL;
        private readonly CompanySyncService CompanySyncService;
        private readonly bool HubspotIntegration = false;


        private static string EMailTemplate = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Template/OnBoarding.InvitationWelcome.cshtml"));

        public OnBoardingApiBL(IEmailSender emailSender, ILogger log, OnBoardingBL onBoardingBL, CompanySyncService companySyncService)
        {
            _emailSender = emailSender;
            Log = log;
            OnBoardingBL = onBoardingBL;
            CompanySyncService = companySyncService;

            HubspotIntegration = CloudConfigurationManager.GetSetting("OnBoarding.HubspotIntegration")?.ToLowerInvariant() == "true";
        }

        public async Task CompanyImport(Company company, string inputFilePath, string invitationMessage, 
                                        UserInfo inviterUser, CompanyInfo inviterCompany, bool? needCheckPartnerLimits = null)
        {
            int companyId = company.Id;

            using (LogContext.PushProperty("OnBoarding", "Vendor"))
            using (LogContext.PushProperty("CompanyId", companyId))
            using (LogContext.PushProperty("File", inputFilePath))
            {
                Log.Information("Starting the Vendors OnBoarding for the Company: {CompanyID}, with the InvitationMessage: {invitationMessage}", companyId, invitationMessage);

                IList<(int companyId, ApplicationUserOnBoardInfo userToInvite)> companiesContacts;
                
                using (var tran = AsyncTransactionScope.StartNew())
                using (LogContext.PushProperty("OnBoarding.Step", "LoadingAndWritingDataToDatabase"))
                { 
                    try
                    {
                        companiesContacts = await OnBoardingBL.ImportVendorAsync(inputFilePath, true, companyId, needCheckPartnerLimits);                       
                        Log.Information("LoadingAndWritingDataToDatabase Done - Response WIth: {Qnt} entries, FullData: {@Data}", companiesContacts.Count, companiesContacts);

                        tran.Complete();
                    }
                    catch (Exception e)
                    {
                        Log.Warning(e, "Error In OnBoarding. {Step}", "LoadingAndWritingDataToDatabase");
                        throw;
                    }
                }

                using (LogContext.PushProperty("OnBoarding.Step", "SendInvitationViaFrontEndSystem"))
                {
                    try
                    {
                        foreach (var companiesContact in companiesContacts)
                        {
                            Log.Information("Calling the endpoint yo start the invitation process. For Company:{CompanyId}, To:{Email}", companiesContact.companyId, companiesContact.userToInvite.Email);

                            var inviterData = new OnBoardingBL.Inviter()
                            { 
                                companyId = inviterCompany.Id.ToString(),
                                userId = inviterUser.UserId,
                            };
                            await OnBoardingBL.SendInvitationToTheNewUserViaTheNewSystemAsync(inviterData, companiesContact.userToInvite);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Warning(e, "Error In OnBoarding. {Step}", "SendInvitationViaFrontEndSystem");
                        throw;
                    }
                }
#if false
                using (LogContext.PushProperty("OnBoarding.Step", "SendInvitationMessage"))
                {
                    try
                    {
                        foreach (var companiesContact in companiesContacts)
                        {
                            Log.Information("Sending the Invitation Email to Company:{CompanyId}, To:{Email}", companiesContact.companyId, companiesContact.userToInvite.Email);

                            //Notification
                            await SendInvitationEmail(companiesContact.userToInvite, invitationMessage, company, inviterUser);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Warning(e, "Error In OnBoarding. {Step} - Error ignored", "SendInvitationMessage");
                        //throw;
                    }
                }
#endif

#if !STAGING
                if (HubspotIntegration)
                {
                    using (LogContext.PushProperty("OnBoarding.Step", "CallHubspot"))
                    {
                        try
                        {
                            Log.Information("Send the Invited Companies to Hubspot");
                            var companies = companiesContacts.Select(c => c.companyId).Distinct().ToList();
                            await CompanySyncService.UpsertCompaniesInHubspotAsync(companies, null, true);
                        }
                        catch (Exception e)
                        {
                            Log.Warning(e, "Error In OnBoarding. {Step}", "CallHubspot");
                            //throw;
                        }
                    }
                }
#endif

                Log.Information("Ending the OnBoarding for the Company: {CompanyID}", companyId);
            }
        }

        private async Task SendInvitationEmail(ApplicationUser invitedUser, string invitationMessage, Company companyThatInviteThisUser, UserInfo userThatInviteThisUser)
        {
            if (string.IsNullOrWhiteSpace(invitationMessage))
                return;

            var emailContent = Engine.Razor.RunCompile(EMailTemplate, null, invitationMessage);

            var message = new EmailMessage() //TODO Add to WebConfig
            {
                Subject = "Welcome to Omnae",
                DestinationEmail = invitedUser.Email,
                Body = emailContent,
            };

            _emailSender.SendEmail(message);
        }

        public async Task<bool> PartsImport(Company company, string inputFilePath)
        {
            int companyId = company.Id;

            using (LogContext.PushProperty("OnBoarding", "Parts"))
            using (LogContext.PushProperty("CompanyId", companyId))
            using (LogContext.PushProperty("File", inputFilePath))
            {
                var option = new TransactionOptions() { Timeout = TimeSpan.FromMinutes(15), IsolationLevel = IsolationLevel.ReadCommitted };
                using (var tran = AsyncTransactionScope.StartNew(option))
                {
                    try
                    {
                        var hasDupe = await OnBoardingBL.ImportPartsAsync(inputFilePath, companyId);                       

                        tran.Complete();
                        return hasDupe;
                    }
                    catch (Exception e)
                    {
                        Log.Warning(e, "Error In OnBoarding");
                        throw;
                    }
                }
            }
        }


    }
}
