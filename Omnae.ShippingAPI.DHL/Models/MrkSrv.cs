using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.ShippingAPI.DHL.Models
{
    public class MrkSrv
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Local Product Code (option)")]
        public string LocalProductCode { get; set; }

        [Display(Name = "Product Short Name (option)")]
        public string ProductShortName { get; set; }

        [Display(Name = "Local Product Name (option)")]
        public string LocalProductName { get; set; }

        [Display(Name = "Network Type Code (option)")]
        public string NetworkTypeCode { get; set; }

        [Display(Name = "POffered Customer Agreement (option)")]
        public string POfferedCustAgreement { get; set; }
        public string TransInd { get; set; }

        public string LocalServiceType { get; set; }
        public string GlobalServiceName { get; set; }
        public string LocalServiceTypeName { get; set; }
        public string ChargeCodeType { get; set; }
        public string MrkSrvInd { get; set; }
    }
}
