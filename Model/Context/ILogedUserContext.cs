using System.Collections.Generic;
using Omnae.Common;
using Omnae.Model.Context.Model;

namespace Omnae.Model.Context
{
    public interface ILogedUserContext
    {
        string UserId { get; }
        USER_TYPE UserType { get; }
        CompanyInfo Company { get; }
        UserInfo User { get; }
        IReadOnlyCollection<string> Roles { get; }
    }
}