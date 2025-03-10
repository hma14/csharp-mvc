using Omnae.Common;
using System;
using System.ComponentModel.DataAnnotations;
using Omnae.BusinessLayer.Models;
using static Omnae.Data.Query.OrderQuery;

namespace Omnae.WebApi.DTO
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        public int TaskId { get; set; }
        public States State { get; set; }

        [Display(Name = "Part Number")]
        public string PartNumber { get; set; }
        public string PartName { get; set; }

        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal? SalesPrice { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal? SalesTax { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal? ToolingCharge { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal Quantity { get; set; }

        [Display(Name = "Shipped Date")]
        public DateTime? ShippedDate { get; set; }

        /// <summary>
        /// ShippingAccount CarrierName to be used in the part Shipment
        /// </summary>
        [Display(Name = "Carrier Name")]
        public string CarrierName { get; set; }

        /// <summary>
        /// ShippingAccount CarrierType to be used in the part Shipment
        /// </summary>
        [Display(Name = "Carrier Type")]
        public SHIPPING_CARRIER_TYPE? CarrierType { get; set; }

        /// <summary>
        /// ShippingAccount ShippingAccountNumber to be used in the part Shipment
        /// </summary>
        public string ShippingAccountNumber { get; set; }

        [Display(Name = "Tracking Number")]
        public string TrackingNumber { get; set; }

        [Display(Name = "Shipping Id")]
        public int? ShippingId { get; set; }

        [Display(Name = "Ship Leading Time")]
        public int? ShipLeadingTime { get; set; }

        [Display(Name = "Customer PO Number")]
        public string CustomerPONumber { get; set; }
        public DateTime? EstimateCompletionDate { get; set; }
        public bool? IsForToolingOnly { get; set; }
        public bool? IsRiskBuild { get; set; }
        public bool IsReorder { get; set; }

        public string Buyer { get; set; }
        public DateTime? DesireShippingDate { get; set; }
        public DateTime? EarliestShippingDate { get; set; }


        public string CreatedByUserId { get; set; }
        
        public string ModifiedByUserId { get; set; }

        public string Notes { get; set; }
        public string RejectReason { get; set; }

        // Doc Uri
        public string[] ProductDrawingUri { get; set; }
        public string[] VendorQuoteUri { get; set; }
        public string[] PODocUri { get; set; }
        public string[] VendorPODocUri { get; set; }
        //public string[] RevisingDocUri { get; set; }
        public string[] ProofDocUri { get; set; }
        public string[] ProofRejectDocUri { get; set; }
        public string[] SampleRejectDocUri { get; set; }
        public string[] PackingSlipDocUri { get; set; }
        public string[] InspectionReportDocUri { get; set; }
        public string[] VendorInvoice { get; set; }
        public string[] PaymentDocUri { get; set; }

        public virtual ShippingDTO Shipping { get; set; }
        public DateTime? _updatedAt { get; set; }
        public int ProductSharingId { get; set; }

        public string OrderCompanyName { get; set; }

        // Expedited Shipment
        public int? ExpeditedShipmentRequestId { get; set; }
        public int? InitiateCompanyId { get; set; }
        public bool? IsExpeditedShipmentAccepted { get; set; }
        public DateTime? NewDesireShippingDate { get; set; }
        public EXPEDITED_SHIPMENT_TYPE? ExpeditedShipmentType { get; set; }
        public bool? IsRequestedByCustomer { get; set; }
        public bool? IsRequestedByVendor { get; set; }

        // Cancel order
        public bool CancelOrderRequest { get; set; } = false;
        public bool IsOrderCancelled { get; set; } = false;
        public string CancelOrderReason { get; set; }
        public string DenyCancelOrderReason { get; set; }
        public int? OrderCancelledBy { get; set; }
    }
}
