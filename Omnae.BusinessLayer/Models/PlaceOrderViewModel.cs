using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Omnae.Model.Models;

namespace Omnae.BusinessLayer.Models
{
    public class PlaceOrderViewModel
    {
        public Product Product { get; set; }
        public int ProductId { get; set; }
        public int TaskId { get; set; }
        public List<PBreaks> PriceBreaks { get; set; }
        public List<PBreaks> ExtraQuantityPriceBreaks { get; set; }
        public decimal?  Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        // public decimal? SalesPrice { get; set; }
        public decimal? Total { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal? SalesTax { get; set; }

        public int CountryId { get; set; }
        public int? ProvinceId { get; set; }

        [Display(Name = "Purchase Order Doc")]
        [Required(ErrorMessage = "Purchase Order Doc is required")]
        public HttpPostedFileBase[] PurchaseOrder { get; set; }

        [Display(Name = "PO Number")]
        [Required(ErrorMessage = "PO Number is required")]
        public string PONumber { get; set; }

        [Display(Name = "Payment Methods")]
        public int PaymentMethod { get; set; }
        public SelectList PaymentMethods { get; set; }

        public bool IsReorder { get; set; }
        public bool IsExtraQuantities { get; set; }

        public int StateId { get; set; }
        public int OrderCompanyId { get; set; }
        public bool IsForOrderTooling { get; set; }
        public bool IsForRiskBuild { get; set; }

        public StateLastUpdatedViewModel LastUpdated { get; set; }
        public List<OrderStateTracking> OrderStateTrackings { get; set; }
        public List<ProductStateTracking> ProductStateTrackings { get; set; }
        public List<PartRevision> PartRevisions { get; set; }

        public decimal? ToolingCharges { get; set; }

        public string Buyer { get; set; }

        [Display(Name = "Requested Delivery Date")]
        public DateTime? DesireShippingDate { get; set; }

        [Display(Name = "Earliest Delivery Date")]
        public DateTime? EarliestShippingDate { get; set; }
        public double? TaxRate { get; set; }
        public string TaxRatePercentage { get; set; }

        /// <summary>
        /// If this order is a saas Model
        /// </summary>
        public bool isEnterprise { get; set; }

        /// <summary>
        /// (Webpage View only - DropDownList if the shipping company (Shippments accounts avaliable)
        /// </summary>
        public SelectList ShippingAccountDDL { get; set; }

        public DateTime? ShipDate { get; set; }

        public int NumberSampleIncluded { get; set; }

        /// <summary>
        /// API - The Shipping Addr to be used in this Order
        /// </summary>
        public virtual int? ShippingProfileId { get; set; } = null;

        /// <summary>
        /// API - The Shippments account to be used in this Order.
        /// This overwrite the ShippingAccount used in the `ShippingProfileId`.
        /// </summary>
        public virtual int? ShippingAccountId { get; set; } = null;

        /// <summary>
        /// API - A Full Addr to be used to this order.
        /// This will create a new Shipping to this Company (Customer)
        /// </summary>
        public virtual ShippingDTO Shipping { get; set; }

        public virtual bool HaveAddrInformation => ((ShippingProfileId != null) || (ShippingAccountId != null) || (Shipping != null));
        public bool? IsNewWorkflow { get; set; }
    }

    public class PBreaks
    {
        [Required]
        public decimal? Quantity { get; set; }

        [Required]
        [Display(Name = "Unit Price")]
        public decimal? UnitPrice { get; set; }
        public decimal? ToolingSetupCharges { get; set; }
    }
}