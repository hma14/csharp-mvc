using System;
using System.Collections.Generic;
using System.Linq;
using Omnae.Data;
using Omnae.Model.Models;
using Omnae.Service.Service.Model;

namespace Omnae.Service.Service
{
    public class UserContactService
    {
        public UserContactService(OmnaeContext dbContext)
        {
            DBContext = dbContext;
        }

        public OmnaeContext DBContext { get; }
        
        public IEnumerable<UserContactInformationModel> GetAllActiveUserConnectFromCompany(Company company)
        {
            if (company == null)
                return Array.Empty<UserContactInformationModel>();
            
            return GetAllActiveUserConnectFromCompany(company.Id);
        }

        public IEnumerable<UserContactInformationModel> GetAllActiveUserConnectFromCompany(int companyId)
        {
            var q = from user in DBContext.Users.AsNoTracking()
                    where user.CompanyId == companyId
                    where user.Active == true
                    select new {user.Email, user.PhoneNumber, user.FirstName, user.LastName, user.Id};
            
            var q2 = from user in q.ToList()
                     select new UserContactInformationModel(user.Email, user.PhoneNumber, user.FirstName, user.LastName, user.Id);
            
            return q2.ToList();
        }
    }
}
