using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Omnae.ViewModels
{
    public class AdminProductDetailsViewModel
    {
        //public int ProductId { get; set; }
        public int TaskId { get; set; }
        public SelectList DdlProducts { get; set; }
    }
}