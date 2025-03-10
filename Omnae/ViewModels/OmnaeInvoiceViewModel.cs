using Omnae.Model.Models;
using System;

namespace Omnae.ViewModels
{
    public class OmnaeInvoiceViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string PartNumber { get; set; }
        public string PartRevision { get; set; }
        public Address BillAddr { get; set; }
        //public Address ShipAddr { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
        public decimal ToolingSetupCharges { get; set; }
        public decimal SalesTax { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public DateTime? ShippingDate { get; set; }
        public string ShipVia { get; set; }
        public string TrackingNo { get; set; }
        public int? Term { get; set; }
        public bool IsOpen { get; set; }
        public DateTime? ClosedDate { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentRefNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string CustomerPONumber { get; set; }
        public string BillNumber { get; set; }
    }
}