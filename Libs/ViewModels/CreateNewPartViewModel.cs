using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.Libs.ViewModel
{
    public class CreateNewPartViewModel
    {
        public string UserName { get; set; }
        public string PartNumber { get; set; }
        public string Description { get; set; }
        public bool IsCustomer { get; set; }
    }
}