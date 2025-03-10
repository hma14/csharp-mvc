using System;
using System.Configuration;
using System.IO;
using System.Linq;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentEmail.Mailgun;
using Omnae.Notification.Model;
using Serilog;

namespace Omnae.Notification
{
    public class DirectEmailSender : IEmailSender
    {
        private string AccountingMail { get; } = ConfigurationManager.AppSettings["ACCOUNTING_EMAIL"];
        private string FromMail { get; } = ConfigurationManager.AppSettings["FROM_EMAIL"];

        public ILogger Log { get; }

        public DirectEmailSender(ILogger log)
        {
            Log = log;
            Email.DefaultSender = new MailgunSender(
                ConfigurationManager.AppSettings["Mailgun.Domain"], // Mailgun Domain
                ConfigurationManager.AppSettings["Mailgun.APIKEY"] // Mailgun API Key
            );
        }

        public void SendEmail(EmailMessage message)
        {
            var email = Email
                .From(FromMail, "Omnae Team")
                .To(message.DestinationEmail, message.DestinationName ?? "Omnae User")
                .Subject(message.Subject)
                .Body(message.Body, true);

            if (message.ToEmails != null)
            {
                foreach (var toMail in message.ToEmails.Select(e => e.Trim()).Where(e => message.DestinationEmail.Equals(e, StringComparison.CurrentCultureIgnoreCase) == false))
                {
                    email = email.To(toMail);
                }
            }
            if (message.ToAccounting)
            {
                email = email.CC(AccountingMail);
            }

            if (message.AttachData != null && message.AttachData.Any())
            {
                foreach (var data in message.AttachData)
                {
                    email = email.Attach(new Attachment { Data = new MemoryStream(data), IsInline = false });
                }
            }

            SendEmail(message.Subject, message.DestinationEmail, email);
        }

        public void SendNormalEmail(string sender, string subject, string from, string destination, string content)
        {
            var email = Email
                .From(from)
                .To(destination, "Dear Omnae Team")
                .Subject(subject + " - sent by " + sender)
                .Body(content, true);

            SendEmail(subject, destination, email);
        }

        private void SendEmail(string subject, string destination, IFluentEmail email)
        {
            try
            {
                Log.Debug("Sending email...");
                var response = email.Send();

                if (response.Successful)
                {
                    Log.Information("Email send. Subject:{Subject}, To:{To}, Successful:{Successful}", subject, destination, response.Successful);
                }
                else
                {
                    foreach (var errorMessage in response.ErrorMessages)
                    {
                        Log.Error("Error sending a e-mail. Subject:{Subject}, To:{To} - Msg: {Msg}", subject, destination, errorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error sending a e-mail. Subject:{Subject}, To:{To}", subject, destination);
                throw;
            }
        }
    }
}