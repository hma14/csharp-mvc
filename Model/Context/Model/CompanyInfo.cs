using System;
using Omnae.Model.Models;

namespace Omnae.Model.Context.Model
{
    [Serializable]
    public class CompanyInfo
    {
        public CompanyInfo(int id, string name, CompanyType companyType, bool isEnterprise)
        {
            Id = id;
            Name = name;
            CompanyType = companyType;
            IsEnterprise = isEnterprise;
        }

        public int Id { get; }
        public string Name { get; }
        public CompanyType CompanyType { get; }
        public bool IsEnterprise { get; }
    }
}