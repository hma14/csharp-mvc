using System;

namespace Omnae.Notification.Model
{
    internal class EmailIdMessage
    {
        public EmailIdMessage()
        {
        }

        public EmailIdMessage(EmailMessage email)
        {
            this.EmailDataIdInTheBlob = email.Id;
        }

        public Guid EmailDataIdInTheBlob { get; set; }
    }
}