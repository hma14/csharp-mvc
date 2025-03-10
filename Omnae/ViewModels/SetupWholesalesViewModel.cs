using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Omnae.ViewModels
{
    public class SetupWholesalesViewModel
    {
        public int productId { get; set; }
        public IEnumerable<SelectListItem> PartRevisionList { get; set; }
    }
}