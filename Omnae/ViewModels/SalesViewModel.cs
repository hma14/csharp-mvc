using Omnae.BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.ViewModels
{
    public class SalesViewModel : TransactionViewModel
    {
        public string CustomerName { get; set; }
        public string PONumber { get; set; }
        public decimal Total1 { get; set; }
        public decimal ToolingSetup { get; set; }

        public decimal Total2 { get; set; }


    }
}