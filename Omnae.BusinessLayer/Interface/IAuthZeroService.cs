using System.Threading.Tasks;
using Omnae.BusinessLayer.Identity.Model;
using Omnae.BusinessLayer.Interface.Model;
using Omnae.Common;

namespace Omnae.BusinessLayer.Interface
{
    public interface IAuthZeroService
    {
        //Task<OAuthUserData> CreateOrGetUser(ApplicationUser newUser, int companyId, string newPassword);
    }

    public class NoImpAuthZeroService : IAuthZeroService
    {
        //public Task<OAuthUserData> CreateOrGetUser(ApplicationUser newUser, int companyId, string newPassword)
        //{
        //    //Do Nothing
        //    return null;
        //}
    }
}
namespace Omnae.BusinessLayer.Interface.Model 
{
    public class OAuthUserData
    {
        /// <summary>
        /// Id SUed By the new oAuth User Base
        /// </summary>
        public string oAuthUserId { get; set; }

        /// <summary>
        /// Id Used By Asp.Net Identity
        /// </summary>
        public string LegacyId { get; set; }

        public USER_TYPE UserType { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        //public bool TwoFactorEnabled { get; set; }
        public string UserName { get; set; }
        public int? CompanyId { get; set; }
        public bool Blocked { get; set; }
    }
}
