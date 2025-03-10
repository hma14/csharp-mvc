using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JetBrains.Annotations;
using Omnae.Common;
using Omnae.Model.Models.Aspnet;

namespace Omnae.Model.Models
{
    public class Order
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //public int CustomerId { get; set; }
        //public int VendorId { get; set; }
        public int ProductId { get; set; }

        [ForeignKey("TaskData")]
        public int? TaskId { get; set; }

        [Display(Name = "Part Number")]
        public string PartNumber { get; set; }

        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal? SalesPrice { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal? SalesTax { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        [Display(Name = "Unit Price")]
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

       // [ForeignKey("Shipping")]
        [Display(Name = "Shipping Id")]
        public int? ShippingId { get; set; }

        [Display(Name = "Ship Leading Time")]
        public int? ShipLeadingTime { get; set; }

        [Display(Name = "Customer PO Number")]
        public string CustomerPONumber { get; set; }
        public DateTime? EstimateCompletionDate { get; set; }
        public bool? IsForToolingOnly { get; set; }
        public bool IsReorder { get; set; }

        public string Buyer { get; set; }
        public DateTime? DesireShippingDate { get; set; }
        public DateTime? EarliestShippingDate { get; set; }


        [ForeignKey("CreatedByUser")]
        public string CreatedByUserId { get; set; }
        
        [ForeignKey("ModifiedByUser")]
        public string ModifiedByUserId { get; set; }

        public string Notes { get; set; }
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime? _updatedAt { get; set; }

        [ForeignKey("ProductSharing")]
        public int? ProductSharingId { get; set; }

        public string OrderCompanyName { get; set; }

        [ForeignKey("Customer")]
        public int? CustomerId { get; set; }

        // Expedited Shipment Request

        [ForeignKey("ExpeditedShipmentRequest")]
        public int? ExpeditedShipmentRequestId { get; set; }

        // Cancel order
        public bool IsOrderCancelled { get; set; } = false;
        public string CancelOrderReason { get; set; }
        public string DenyCancelOrderReason { get; set; }
        public int? OrderCancelledBy { get; set; }

        //[ForeignKey("Shipment")]
        //public List<int> ShipmentId { get; set; }
        //public bool? IsMultipleShipment { get; set; }


        // Navigation Property

        public virtual Shipping Shipping { get; set; }
        public virtual Product Product { get; set; }
        public virtual TaskData TaskData { get; set; }
        public virtual ProductSharing ProductSharing { get; set; }
        public virtual Company Customer { get; set; }

        public virtual ICollection<OmnaeInvoice> Invoices { get; set; }

        [CanBeNull]
        public virtual SimplifiedUser CreatedByUser { get; set; }

        [CanBeNull]
        public virtual SimplifiedUser ModifiedByUser { get; set; }

        [CanBeNull]
        public virtual ExpeditedShipmentRequest ExpeditedShipmentRequest { get; set; }

        //[CanBeNull]
        //public virtual Shipment Shipment { get; set; }

    }
}
