using JetBrains.Annotations;
using Omnae.Common;
using Omnae.Model.Context;
using Omnae.Model.Context.Model;
using Omnae.Model.Models;
using Omnae.Model.Security;
using Omnae.Service.Service.Interfaces;
using Omnae.WebApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Routing;
using Omnae.BusinessLayer;
using Omnae.BusinessLayer.Identity.Model;
using Omnae.BusinessLayer.Interface.Model;

namespace Omnae.WebApi.Context
{
    [Serializable]
    public class LogedUserApiContext : ILogedUserContext
    {
        public LogedUserApiContext(AuthZeroManager authZeroManager, ICompanyService companyService, ApplicationDbContext dbUserContext)
        {
            if(HttpContext.Current?.User?.Identity == null)
                return;

            var userIdFromContext = GetUserIdFromContext(HttpContext.Current);
            if(userIdFromContext == null)
                return;

            var userData = userIdFromContext.Contains("auth0|")
                                ? authZeroManager.GetUserByAuthZeroIdAsync(userIdFromContext)
                                : authZeroManager.GetUserByLagacyIdAsync(userIdFromContext);

            var company = userData.CompanyId != (int?)null ? companyService.FindCompanyById(userData.CompanyId.Value) : null;
         
            UserId = userData.LegacyId ?? userData.oAuthUserId;
            UserType = USER_TYPE.Customer; //TODO, Get the Correct Type
            Company = (company != null) ? new CompanyInfo(company.Id, company.Name, company.CompanyType ?? CompanyType.None, company.isEnterprise) : null;
            User = new UserInfo(UserId, userData.UserName, userData.Email, userData.Blocked, userData.EmailConfirmed);

            UpdateLocalDatabaseUser(dbUserContext, userData);
        }

        private void UpdateLocalDatabaseUser(ApplicationDbContext dbUserContext, OAuthUserData userData)
        {
            var dbUser = dbUserContext.Users.Find(UserId);
            if (dbUser == null)
            {
                var userName = (userData.UserName ?? userData.Email);

                var anotherUserHaveTheSameUserName = dbUserContext.Users.Any(u => u.UserName == userName);
                if (anotherUserHaveTheSameUserName)
                {
                    throw new BLException("A user with same Login exists in the current system.");
                }

                //Create a new user
                dbUserContext.Users.Add(new ApplicationUser()
                {
                    Id = UserId,
                    Active = true,
                    UserName = userName,
                    FirstName = userData.FirstName,
                    MiddleName = userData.MiddleName,
                    LastName = userData.LastName,
                    UserType = UserType,
                    CompanyId = Company?.Id,
                    Email = userData.Email,
                    EmailConfirmed = userData.EmailConfirmed,
                    PhoneNumber = userData.PhoneNumber,
                    PhoneNumberConfirmed = userData.PhoneNumberConfirmed,
                    LockoutEnabled = userData.Blocked,
                });

                dbUserContext.SaveChanges();
            }
            else
            {
                //Update the user
                dbUser.Id = UserId;
                dbUser.FirstName = userData.FirstName ?? dbUser.FirstName;
                dbUser.MiddleName = userData.MiddleName ?? dbUser.MiddleName;
                dbUser.LastName = userData.LastName ?? dbUser.LastName;
                //dbUser.UserType = UserType ?? dbUser.UserType;
                dbUser.CompanyId = Company?.Id ?? dbUser.CompanyId;
                dbUser.Email = userData.Email ?? dbUser.Email;
                dbUser.EmailConfirmed = userData.EmailConfirmed;
                dbUser.PhoneNumber = userData.PhoneNumber ?? dbUser.PhoneNumber;
                dbUser.PhoneNumberConfirmed = userData.PhoneNumberConfirmed;
                dbUser.LockoutEnabled = userData.Blocked;

                dbUserContext.SaveChanges();
            }
        }

        [CanBeNull]
        private static string GetUserIdFromContext(HttpContext context)
        {
            var routeData = context.Request.RequestContext.RouteData;

            if (routeData.Values.ContainsKey("userId"))
            {
                var userIdFromRouteContext = routeData.Values["userId"] as string;
                if (userIdFromRouteContext != null)
                    return userIdFromRouteContext;
            }

            if (routeData.Values.ContainsKey("MS_SubRoutes"))
            {
                var subRoutes = ((IHttpRouteData[]) routeData.Values["MS_SubRoutes"])?.FirstOrDefault();
                if (subRoutes?.Values.ContainsKey("userId")== true)
                {
                    var userIdFromSubRouteContext = subRoutes.Values["userId"] as string;
                    if (userIdFromSubRouteContext != null)
                        return userIdFromSubRouteContext;
                }
            }

            if (!string.IsNullOrWhiteSpace(context.Request.QueryString["userId"]))
            {
                var userIdFromContext = context.Request.QueryString["userId"];
                if (userIdFromContext != null)
                    return userIdFromContext;
            }

            if (!string.IsNullOrWhiteSpace(context.Request["userId"]))
            {
                var userIdFromContext = context.Request["userId"];
                if (userIdFromContext != null)
                    return userIdFromContext;
            }

            if (context.Items.Contains("userId"))
            {
                var userIdFromContext = context.Items["userId"] as string;
                if (userIdFromContext != null)
                    return userIdFromContext;
            }

            //throw new BLException("User id not found");
            return null;
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
   
    public static class LogedUserApiContextEx
    {
        public static bool IsAccountAdmin(this LogedUserApiContext ctx)
        {
            return ctx.Roles.Contains(Roles.CompanyAdmin);
        }
    }
}