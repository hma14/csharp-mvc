using System;

namespace Omnae.Model.Context.Model
{
    [Serializable]
    public class UserInfo
    {
        public UserInfo(string userId, string userName, string email, bool active = true, bool confirmed = true)
        {
            UserId = userId;
            Active = active;
            Confirmed = confirmed;
            Email = email;
            UserName = userName;
        }

        public string UserId { get; }
        public string UserName { get; }
        public string Email { get; }


        public bool Active { get; }
        public bool Confirmed { get; }
        public bool CanUseTheSystem => Active && Confirmed;
    }
}