using Omnae.Common;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.QuickBooks.ViewModels
{
    public class PurchaseOrderViewModel : QboBaseViewModel
    {
        public decimal CustomerToolingCharge { get; set; }
        public decimal VendorToolingCharge { get; set; }
        public decimal VendorUnitPrice { get; set; }
        public string Attention { get; set; }
        

        public string CompanyName { get; set; }

        public string VendorAddress_AddressLine1 { get; set; }
        public string VendorAddress_AddressLine2 { get; set; }
        public string VendorAddress_City { get; set; }
        public string VendorAddress_State { get; set; }
        public int VendorAddress_CountryId { get; set; }
        public string VendorAddress_CountryName { get; set; }
        public int? VendorAddress_StateProvinceId { get; set; }
        public string VendorAddress_ZipCode { get; set; }
        public string VendorAddress_PostalCode { get; set; }

        public string AdminAddress_AddressLine1 { get; set; }
        public string AdminAddress_AddressLine2 { get; set; }
        public string AdminAddress_City { get; set; }
        public string AdminAddress_State { get; set; }
        public int AdminAddress_CountryId { get; set; }
        public string AdminAddress_CountryName { get; set; }
        public int? AdminAddress_StateProvinceId { get; set; }
        public string AdminAddress_ZipCode { get; set; }
        public string AdminAddress_PostalCode { get; set; }

        // Bill Address
        public string BillAddress_AddressLine1 { get; set; }
        public string BillAddress_AddressLine2 { get; set; }
        public string BillAddress_City { get; set; }
        public string BillAddress_State { get; set; }
        public int BillAddress_CountryId { get; set; }
        public string BillAddress_CountryName { get; set; }
        public int? BillAddress_StateProvinceId { get; set; }
        public string BillAddress_ZipCode { get; set; }
        public string BillAddress_PostalCode { get; set; }


        // Shipping Address
        public string ShipAddress_AddressLine1 { get; set; }
        public string ShipAddress_AddressLine2 { get; set; }
        public string ShipAddress_City { get; set; }
        public string ShipAddress_State { get; set; }
        public int ShipAddress_CountryId { get; set; }
        public string ShipAddress_CountryName { get; set; }
        public int? ShipAddress_StateProvinceId { get; set; }
        public string ShipAddress_ZipCode { get; set; }
        public string ShipAddress_PostalCode { get; set; }

        public string EstimateId { get; set; }
        public string EstimateNumber { get; set; }
        public string PONumber { get; set; }
        public string CustomerPONumber { get; set; }
        public DateTime ShipDate { get; set; }
        public DateTime? DesireShippingDate { get; set; }
        public DateTime? EarliestShippingDate { get; set; }
        public int NumberSampleIncluded { get; set; }
        public int OrderId { get; set; }
        public string CompanyLogoUri { get; set; }
    }
}
