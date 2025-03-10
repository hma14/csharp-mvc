using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Web;

namespace Omnae.WebApi.DTO
{
    public class PlaceOrderDTO
    {
        public int ProductId { get; set; }
        public int TaskId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
        public decimal SalesTax { get; set; }

        public HttpPostedFileBase[] PurchaseOrder { get; set; }


        public string PONumber { get; set; }

        public int PaymentMethod { get; set; }

        public bool IsReorder { get; set; }
        public bool IsExtraQuantities { get; set; }

        public int StateId { get; set; }
        public bool IsForOrderTooling { get; set; }    

        public decimal ToolingCharges { get; set; }

        public string Buyer { get; set; }

        public DateTime DesireShippingDate { get; set; }

        public DateTime EarliestShippingDate { get; set; }
        public double TaxRate { get; set; }
        public string TaxRatePercentage { get; set; }
        public bool isEnterprise { get; set; }
        public string ShipVia { get; set; }
        public DateTime ShipDate { get; set; }
    }
}