using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Omnae.ShippingAPI.DHL.Models
{
    public class BkgDetailsResp
    {
        [Key]
        public int Id { get; set; }
        public ServiceArea OriginServiceArea { get; set; }
        public ServiceArea DestinationServiceArea { get; set; }
        public List<QtdShpResp> QtdShps { get; set; }

    }
}
