using System;
using Microsoft.AspNet.Identity;
using Omnae.Common;
using System.Configuration;
using Omnae.Notification;
using Omnae.Notification.Model;
using Serilog;
using Twilio.Rest.Api.V2010.Account;

namespace Libs.Notification
{
    public interface ISmsSender
    {
        void SendSms(EmailMessage message);
    }

    public class SmsSender : ISmsSender
    {
        public bool IsEnable { get; }
        public ILogger Log { get; }

        public SmsSender(ILogger log)
        {
            Log = log;
            IsEnable = bool.TryParse(ConfigurationManager.AppSettings["SMS_Enable"], out var value) && value;
        }

        public void SendSms(EmailMessage message)
        {
            if(!IsEnable)
                return;
            
            if(string.IsNullOrWhiteSpace(message?.DestinationPhone))
                return;

            try
            {
                var result = MessageResource.Create(
                    body: message.Body,
                    from: new Twilio.Types.PhoneNumber(ConfigurationManager.AppSettings["SMSAccountFrom"]),
                    to: new Twilio.Types.PhoneNumber(message.DestinationPhone)
                );
                
                // Status is one of Queued, Sending, Sent, Failed or null if the number is not valid
                if (result != null && result.Status != MessageResource.StatusEnum.Failed)
                {
                    Log.Information("Twilio, sid:" + result.Sid);
                    return;
                }

                throw new Exception(IndicatingMessages.SmsWarningMsg);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error sending the SMS.");
                throw new Exception(IndicatingMessages.SmsWarningMsg, e);
            }
        }
    }
}