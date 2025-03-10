using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Omnae.Common;
using Omnae.Model.Models;
using Omnae.ShippingAPI.DHL.Models;

namespace Omnae.BusinessLayer.Models
{
    public class StateTrackingViewModel
    {
        [Required]
        [Display(Name = "Order Id")]
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
        public int TaskId { get; set; }
        public States StateId { get; set; }
        public Order Order { get; set; }
        public Product Product { get; set; }
        public bool? IsTagged { get; set; }
        public StateLastUpdatedViewModel LastUpdated { get; set; }

        public List<OrderStateTracking> OrderStateTrackings { get; set; }
        public List<ProductStateTracking> ProductStateTrackings { get; set; }

        public DHLResponse DHLResponse { get; set; }
        public bool isEnterprise { get; set; }

        public USER_TYPE UserType { get; set; }

        public int? NumberSampleIncluded { get; set; }

    }
}