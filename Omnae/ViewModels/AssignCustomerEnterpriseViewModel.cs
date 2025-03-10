using Omnae.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Omnae.ViewModels
{
    public class AssignCustomerEnterpriseViewModel
    {
        public int CompanyId { get; set; }      
        public CUSTOMER_TYPE CustomerType { get; set; }
        public SelectList Companies { get; set; }
    }
}