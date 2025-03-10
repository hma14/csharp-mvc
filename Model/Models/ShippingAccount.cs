using Omnae.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Model.Models
{
    public class ShippingAccount
    {       
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        
        public string Carrier { get; set; }

        public SHIPPING_CARRIER_TYPE CarrierType { get; set; }

        public string AccountNumber { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsDefault { get; set; } = false;

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime? _updatedAt { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime? _createdAt { get; set; }

        ////////////////////////////////

        public virtual Company Company { get; set; }

        public virtual ICollection<ShippingProfile> ShippingProfiles { get; set; }

        ////////////////////////////////

    }
}
