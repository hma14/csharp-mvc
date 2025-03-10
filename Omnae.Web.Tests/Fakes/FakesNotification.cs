using System.Collections.Generic;
using Libs.Notification;
using Microsoft.AspNet.Identity;
using Omnae.Libs;
using Omnae.Model.Models;

namespace Omnae.Web.Tests.Fakes
{
    class EmailSenderFake: IEmailSender
    {
        public static bool SmsSend { get; private set; }

        public string SendEmail(IdentityMessage message, bool toAccounting = false, List<string> toEmails = null, List<byte[]> attachData = null, List<string> attachName = null)
        {
            SmsSend = true;
            return "";
        }
        
        public string SendNormalEmail(string sender, string subject, string @from, string destination, string content)
        {
            SmsSend = true;
            return "";
        }
    }

    class SmsSenderFake : ISmsSender
    {
        public static bool SmsSend { get; private set; }

        public void SendSms(IdentityMessage message)
        {
            SmsSend = true;
        }
    }
}
