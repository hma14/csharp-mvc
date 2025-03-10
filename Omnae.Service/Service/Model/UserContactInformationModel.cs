using Common.Extensions;
using Omnae.Common.Extensions;

namespace Omnae.Service.Service.Model
{
    public class UserContactInformationModel
    {
        public string UserId { get; }

        public string Email { get; }
        public string PhoneNumber { get; }
        public string FirstName { get; }
        public string LastName { get; }

        public UserContactInformationModel(string email, string phoneNumber, string firstName, string lastName, string userId)
        {
            Email = email;
            PhoneNumber = phoneNumber;
            FirstName = firstName;
            LastName = lastName;
            UserId = userId;
        }

        public string FullName => $"{FirstName} {LastName}".TrimAll().ToNullIfEmpty();
    }
}