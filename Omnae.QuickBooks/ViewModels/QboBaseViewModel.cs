using Omnae.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.QuickBooks.ViewModels
{
    public class QboBaseViewModel
    {
        public string QboId { get; set; }
        public string CustomerQboId { get; set; }
        

        public int TaskId { get; set; }
        public int ProductId { get; set; }
        public string PartNumber { get; set; }
        public string PartRevision { get; set; }
        public int? VendorId { get; set; }
        public string VendorName { get; set; }

        public int? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int? Term { get; set; }
        public decimal Total { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal ToolingCharges { get; set; }
        public decimal Quantity { get; set; }
        public decimal SalesTax { get; set; }

        public string ProductName { get; set; }
        public string ProductDescription { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime DueDate { get; set; }

        public string PhoneNumber { get; set; }

        public string EmailAddress { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }

        public int? StateProvinceId { get; set; }

        public string ZipCode { get; set; }

        public string PostalCode { get; set; }

        public bool isBilling { get; set; }

        public bool isShipping { get; set; }

        public string CarrierName { get; set; }

        public string TrackingNumber { get; set; }

        public string HarmonizedCode { get; set; }

        public string AttachFileName { get; set; }
        public string AttachFileAccessUri { get; set; }
        public string AttachTempDownloadUri { get; set; }
        public int AttachFileSize { get; set; }
        public string AttachFileContentType { get; set; }
        public string AttachedReferenceId { get; set; }

        public MATERIALS_TYPE ProductCategory { get; set; }

        public int? UnitOfMeasurement { get; set; }

    }
}
