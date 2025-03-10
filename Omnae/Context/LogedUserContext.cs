using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JetBrains.Annotations;
using Microsoft.AspNet.Identity;
using Omnae.BusinessLayer.Identity;
using Omnae.Common;
using Omnae.Model.Context;
using Omnae.Model.Context.Model;
using Omnae.Model.Models;
using Omnae.Model.Security;
using Omnae.Service.Service.Interfaces;

namespace Omnae.Context
{
    [Serializable]
    public class LogedUserContext : ILogedUserContext
    {
        public LogedUserContext(ICompanyService companyService, ApplicationUserManager userManager)
        {
            if(HttpContext.Current?.User?.Identity == null)
                return;

            var session = HttpContext.Current.Session;
            var user = HttpContext.Current.User.Identity;
            
            if (user.IsAuthenticated)
            {
                var userId = user.GetUserId();
                var userDb = userManager.FindById(userId);
                var userType = userDb?.UserType;
                var company = companyService.FindCompanyByUserId(userId);

                UserId = userId;
                UserType = userType ?? USER_TYPE.Unknown;
                Company = (company != null) ? new CompanyInfo(company.Id, company.Name, company.CompanyType ?? CompanyType.None, company.isEnterprise) : null;
                User = (userDb != null) ? new UserInfo(UserId, userDb.UserName, userDb.Email, userDb.Active ?? false, userDb.EmailConfirmed) : null;
                if (User == null)
                    return;

                var roles = userManager.GetRolesAsync(UserId).GetAwaiter().GetResult() as IEnumerable<string>;
                Roles = new HashSet<string>(roles ?? Array.Empty<string>());

                session["UserType"] = UserType;
                session["CompanyName"] = Company?.Name ?? "";
            }
        }

        [CanBeNull]
        public string UserId { get; } = null;


        public USER_TYPE UserType { get; } = USER_TYPE.Unknown;

        [CanBeNull]
        public CompanyInfo Company { get; } = null;

        [CanBeNull]
        public UserInfo User { get; } = null;

        public IReadOnlyCollection<string> Roles { get; } = new HashSet<string>();
    }

   

    public static class LogedUserContextEx
    {
        public static bool IsAccountAdmin(this ILogedUserContext ctx)
        {
            return ctx.Roles.Contains(Roles.CompanyAdmin);
        }
    }
}