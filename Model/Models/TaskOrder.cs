using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Model.Models
{
    public class TaskOrder
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int StateId { get; set; }
        public bool IsTagged { get; set; }

        public string Attention { get; set; }
        public int PartNumber { get; set; }
        public string Name { get; set; }

        public int OrderId { get; set; }

        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        public int Quantity { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal? SalesPrice { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal? SalesTax { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        [Display(Name = "Unit Price")]
        public decimal? UnitPrice { get; set; }
        

        [Display(Name = "Carrier Name")]
        public string CarrierName { get; set; }

        [Display(Name = "Tracking Number")]
        public string TrackingNumber { get; set; }

        public int? ShipLeadingTime { get; set; }
        public int? ProductId { get; set; }
        public int? ShippingId { get; set; }

    }
}
