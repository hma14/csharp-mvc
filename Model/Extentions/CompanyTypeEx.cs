using System;
using Omnae.Common;
using Omnae.Model.Models;

namespace Omnae.Model.Extentions
{
    public static class CompanyTypeEx
    {
        public static USER_TYPE ToUserType(this CompanyType type)
        {
            switch (type)
            {
                case CompanyType.None:
                    return USER_TYPE.Unknown;
                case CompanyType.Customer:
                    return USER_TYPE.Customer;
                case CompanyType.Vendor:
                    return USER_TYPE.Vendor;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}