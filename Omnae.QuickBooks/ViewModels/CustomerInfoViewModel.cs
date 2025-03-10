using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.QuickBooks.ViewModels
{
    public class CustomerInfoViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
    }
}