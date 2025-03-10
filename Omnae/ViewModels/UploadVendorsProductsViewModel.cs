using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.ViewModels
{
    public class UploadVendorsProductsViewModel
    {
        public HttpPostedFileBase InputVendorsExcel { get; set; }
        public HttpPostedFileBase InputProductsExcel { get; set; }
    }
}