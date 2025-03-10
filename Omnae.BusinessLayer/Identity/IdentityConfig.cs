using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Libs.Notification;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using Omnae.BusinessLayer.Identity.Model;
using Omnae.Notification;
using Omnae.Notification.Model;

namespace Omnae.BusinessLayer.Identity
{
    public class EmailService : IIdentityMessageService
    {
        public EmailService(IEmailSender emailSender)
        {
            EmailSender = emailSender;
        }

        public IEmailSender EmailSender { get; }

        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.Run(() => EmailSender.SendEmail(Convert(message)));
        }

        private EmailMessage Convert(IdentityMessage message) => new EmailMessage { DestinationEmail = message.Destination, Subject = message.Subject, Body = message.Body, };
    }

    public class SmsService : IIdentityMessageService
    {
        public SmsService(ISmsSender smsSender)
        {
            SmsSender = smsSender;
        }

        public ISmsSender SmsSender { get; }

        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.Run(() => SmsSender.SendSms(Convert(message)));
        }

        private EmailMessage Convert(IdentityMessage message) => new EmailMessage { DestinationPhone = message.Destination, Subject = message.Subject, Body = message.Body, };
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(OmnaeUserStore store, EmailService emailService, SmsService smsService)
            : base(store)
        {
            // Configure validation logic for usernames
            this.UserValidator = new UserValidator<ApplicationUser>(this)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            this.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            // Configure user lockout defaults
            this.UserLockoutEnabledByDefault = true;
            this.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            this.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            this.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            this.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            this.EmailService = emailService;
            this.SmsService = smsService;

            var dataProtectionProvider = Startup.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                IDataProtector dataProtector = dataProtectionProvider.Create("ASP.NET Identity");

                this.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtector);
            }
        }

        public OmnaeUserStore OmnaeUserStore => this.Store as OmnaeUserStore;

        public virtual async Task<IEnumerable<ApplicationUser>> FindAllByCompanyIdAsync(int companyId)
        {
            return await this.OmnaeUserStore.FindAllByCompanyIdAsync(companyId);
        }

    }

    public class OmnaeUserStore : UserStore<ApplicationUser>
    {
        public OmnaeUserStore(ApplicationDbContext context) : base(context)
        {
        }

        public virtual async Task<IEnumerable<ApplicationUser>> FindAllByCompanyIdAsync(int companyId)
        {
            return await this.Users.Where(u => u.CompanyId == companyId).ToListAsync();
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }
    }
}
