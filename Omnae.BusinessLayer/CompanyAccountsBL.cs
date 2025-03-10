using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Libs.Exceptions;
using Omnae.BusinessLayer.Identity;
using Omnae.BusinessLayer.Identity.Model;
using Omnae.Data;
using Omnae.Model.Models.Aspnet;
using Omnae.Model.Security;

namespace Omnae.BusinessLayer
{
    public class CompanyAccountsBL
    {
        private OmnaeContext DB { get; }
        private ApplicationUserManager UserManager { get; }


        public CompanyAccountsBL(OmnaeContext dbContext, ApplicationUserManager userManager)
        {
            DB = dbContext;
            UserManager = userManager;
        }

        public async Task<IEnumerable<SimplifiedUser>> ListAllUsersAsync(int? currentCompanyId, string ignoreThisUserID = null)
        {
            var usersFromCompany = await (from user in DB.Users.AsNoTracking()
                                          where user.CompanyId == currentCompanyId && user.Active == true
                                          where ignoreThisUserID == null || user.Id != ignoreThisUserID
                                          select user).ToListAsync();
            return usersFromCompany;
        }

        public async Task<SimplifiedUser> FindByIdAsync(string id, int? companyId)
        {
            var simplifiedUser = await DB.Users.FindAsync(id);
            return simplifiedUser == null || companyId == null || simplifiedUser.CompanyId == companyId ? simplifiedUser : null;
        }

        public async Task<SimplifiedUser> CreateAsync(SimplifiedUser user, int? companyId)
        {
            var userExists = await UserExistAsync(user);
            var userActive = await IsUserActiveAsync(user.Id);
            var userIsFromCompany = await UserHaveSameCompanyEmailAsync(user, companyId);

            if (userExists && !userIsFromCompany)
                throw new BusinessRuleException("This email is used by another company, please use another email for this user");
            if (userExists && userIsFromCompany && !userActive)
                throw new BusinessRuleException("This email already exists in your company and is deactivated.");
            if (userExists && userIsFromCompany)
                throw new BusinessRuleException("This email already exists in your company.");
           if (userExists)
                throw new BusinessRuleException("This email already exists in your system.");

            var appUser = new ApplicationUser
            {
                Id = user.Id,
                UserName = user.Email,
                Email = user.Email,
                UserType = user.UserType,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                CompanyId = user.CompanyId,
                Active = user.Active,
            };

            var result = await UserManager.CreateAsync(appUser);
            if (!result.Succeeded)
                throw new ApplicationException("Error Creating the User");

            await UserManager.AddToRoleAsync(user.Id, Roles.CompanyUser);

            return user;
        }

        private async Task<bool> UserHaveSameCompanyEmailAsync(SimplifiedUser user, int? companyId)
        {
            if (companyId == null)
                return false;

            var currentCompanyEmail = await (from allUsers in DB.Users
                                             where allUsers.CompanyId == companyId
                                             select allUsers.Email).FirstOrDefaultAsync();

            if (currentCompanyEmail == null)
                return false;

            var currentCompanyEmailDomain = new MailAddress(currentCompanyEmail).Host;
            var newUserEmailDomain = new MailAddress(user.Email).Host;

            return string.Equals(currentCompanyEmailDomain, newUserEmailDomain, StringComparison.InvariantCultureIgnoreCase);
        }

        private async Task<bool> UserExistAsync(SimplifiedUser user)
        {
            return await DB.Users.AnyAsync(u => u.Id == user.Id) ||
                   await DB.Users.AnyAsync(u => u.Email == user.Email);
        }

        public async Task EditAsync(SimplifiedUser user)
        {
            var dbUser = await DB.Users.FindAsync(user.Id);
            Debug.Assert(dbUser != null, nameof(dbUser) + " != null");
            
            dbUser.FirstName = user.FirstName;
            dbUser.MiddleName = user.MiddleName;
            dbUser.LastName = user.LastName;
            dbUser.PhoneNumber = user.PhoneNumber;
            dbUser.UserType = user.UserType;

            await DB.SaveChangesAsync();
        }
        
        public async Task ActivateAsync(string userId)
        {
            var simplifiedUser = await DB.Users.FindAsync(userId);
            simplifiedUser.Active = true;
            await DB.SaveChangesAsync();
        }
        public async Task DeactivateAsync(string userId)
        {
            var simplifiedUser = await DB.Users.FindAsync(userId);
            simplifiedUser.Active = false;
            await DB.SaveChangesAsync();
        }
        public async Task<bool> IsUserActiveAsync(string userId)
        {
            var userActive = (await DB.Users.FindAsync(userId))?.Active;
            return userActive ?? false;
        }

        public async Task<bool> UserIsFromCompanyAsync(string userId, int? companyId)
        {
            if (companyId == null)
                return false;

            var userCompany = (await DB.Users.FindAsync(userId))?.CompanyId;
            return userCompany == companyId;
        }
    }
}