using Omnae.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Model.Models
{
    public class ExpeditedShipmentRequest
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Index("IX_ExpeditedShipmentRequest", 1, IsUnique = true)]
        public int OrderId { get; set; }

        [Index("IX_ExpeditedShipmentRequest", 2, IsUnique = true)]
        public int InitiateCompanyId { get; set; }
        public EXPEDITED_SHIPMENT_TYPE ExpeditedShipmentType { get; set; }
        public bool? IsRequestedByCustomer { get; set; }
        public bool? IsRequestedByVendor { get; set; }
        public DateTime NewDesireShippingDate { get; set; }
        public bool? IsAccepted { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime _createdAt { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime? _updatedAt { get; set; }
    }
}
