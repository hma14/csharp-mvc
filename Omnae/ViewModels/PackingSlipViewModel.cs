using Omnae.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.ViewModels
{
    public class PackingSlipViewModel
    {
        public int TaskId { get; set; }
        public States State { get; set; }
        public int ProductId { get; set; }
        public string PartNumber { get; set; }
        public string PartName { get; set; }
        public string PartRevision { get; set; }
        public string PartDescription { get; set; }
        public string CustomerPONumber { get; set; }
        public DateTime ShippingDate { get; set; }
        public int Quantity { get; set; }
        public string TrackingNumber { get; set; }
        public string CarrierName { get; set; }
        public string CustomerName { get; set; }      
        public int Term { get; set; }     
        public string Attention { get; set; }
        public int VendorId { get; set; }
        public string CustomerPhoneNumber { get; set; }

        public string CustomerAddress_AddressLine1 { get; set; }
        public string CustomerAddress_AddressLine2 { get; set; }
        public string CustomerAddress_City { get; set; }
        public string CustomerAddress_State { get; set; }
        public string CustomerAddress_CountryName { get; set; }
        public int? CustomerAddress_StateProvinceId { get; set; }
        public string CustomerAddress_ZipCode { get; set; }
        public string CustomerAddress_PostalCode { get; set; }

        public string CustomerShippingAddress_AddressLine1 { get; set; }
        public string CustomerShippingAddress_AddressLine2 { get; set; }
        public string CustomerShippingAddress_City { get; set; }
        public string CustomerShippingAddress_State { get; set; }
        public int CustomerShippingAddress_CountryId { get; set; }
        public string CustomerShippingAddress_CountryName { get; set; }
        public int? CustomerShippingAddress_StateProvinceId { get; set; }
        public string CustomerShippingAddress_ZipCode { get; set; }
        public string CustomerShippingAddress_PostalCode { get; set; }

        public string AdminName { get; set; }
        public string AdminEmailAddress { get; set; }

        public string AdminAddress_AddressLine1 { get; set; }
        public string AdminAddress_AddressLine2 { get; set; }
        public string AdminAddress_City { get; set; }
        public string AdminAddress_State { get; set; }
        public int AdminAddress_CountryId { get; set; }
        public string AdminAddress_CountryName { get; set; }
        public int? AdminAddress_StateProvinceId { get; set; }
        public string AdminAddress_ZipCode { get; set; }
        public string AdminAddress_PostalCode { get; set; }

        public string Buyer { get; set; }
        public string EstimateNumber { get; set; }
        public DateTime OrderDate { get; set; }

        public string CompanyLogoUri { get; set; }
        public bool isEnterprise { get; set; }

    }
}