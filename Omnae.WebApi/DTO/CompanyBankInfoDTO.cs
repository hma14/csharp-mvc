using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.WebApi.DTO
{
    /// <summary>
    /// CompanyBankInfo DTO
    /// </summary>
    public class CompanyBankInfoDTO
    {
        /// <summary>
        /// Bank Name
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// Bank address - AddressLine1
        /// </summary>
        public string AddressLine1 { get; set; }

        /// <summary>
        /// Bank address - AddressLine2
        /// </summary>
        public string AddressLine2 { get; set; }

        /// <summary>
        /// Bank address - ZipCode
        /// </summary>
        public string ZipCode { get; set; }

        /// <summary>
        /// Bank address - PostalCode
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Bank address - City
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Bank address - State Or Province
        /// </summary>
        public string StateProvince { get; set; }

        /// <summary>
        /// Bank address - Country
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Bank Transit #
        /// </summary>
        public string TransitNumber { get; set; }
        /// <summary>
        /// Bank Institution #
        /// </summary>
        public string InstitutionNumber { get; set; }
        /// <summary>
        /// Bank Account #
        /// </summary>
        public string AccountNumber { get; set; }

        /// <summary>
        /// Beneficiary Bank Swift # 
        /// </summary>
        public string BeneficiaryBankSwiftNumber { get; set; }

        /// <summary>
        /// Intermediary Bank
        /// </summary>
        public string IntermediaryBank { get; set; }

        /// <summary>
        /// Intermediary Bank Swift #
        /// </summary>
        public string IntermediaryBankSwiftNumber { get; set; }
    }
}