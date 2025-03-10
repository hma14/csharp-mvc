using System.Collections.Generic;
using Omnae.Model.Models.Aspnet;

namespace Omnae.Service.ViewModels
{
    public class CompanyAccountListViewModel
    {
        public IList<SimplifiedUser> Users { get; set; }
    }
}