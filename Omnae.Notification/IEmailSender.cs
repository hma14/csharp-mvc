using Omnae.Notification.Model;

namespace Omnae.Notification
{
    public interface IEmailSender
    {
        void SendEmail(EmailMessage message);
        void SendNormalEmail(string sender, string subject, string from, string destination, string content);
    }
}