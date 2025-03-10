using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Omnae.WebApi.DTO
{
    public class PriceBreakDTO
    {
        public int ProductId { get; set; }

        public decimal Quantity { get; set; }

        public int? TaskId { get; set; }

        //[DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:n3}", ApplyFormatInEditMode = true)]
        public decimal? UnitPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:n3}", ApplyFormatInEditMode = true)]
        public decimal VendorUnitPrice { get; set; }

        //[DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal? ShippingUnitPrice { get; set; }

        public int? ShippingDays { get; set; }

        public int? NumberSampleIncluded { get; set; }

        public decimal? ToolingSetupCharges { get; set; }
        public decimal? CustomerToolingSetupCharges { get; set; }
    }
}