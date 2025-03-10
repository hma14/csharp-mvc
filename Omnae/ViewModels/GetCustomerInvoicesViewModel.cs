using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Omnae.ViewModels
{
    public class GetCustomerInvoicesViewModel
    {
        public int CustomerId { get; set; }
        public SelectList Customers { get; set; }
    }
}