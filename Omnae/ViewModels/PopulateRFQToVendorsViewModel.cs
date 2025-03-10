using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Omnae.ViewModels
{
    public class PopulateRFQToVendorsViewModel
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }

        public int[] VendorIds { get; set; }

        public bool isEnterprise { get; set; }
    }

    public class UserNameCompanyId
    {
        public int CompanyId { get; set; }
        public string UserName { get; set; }
    }
}