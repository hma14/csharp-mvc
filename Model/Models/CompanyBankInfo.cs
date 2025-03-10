using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Model.Models
{
    public class CompanyBankInfo
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string BankName { get; set; }

        [ForeignKey("BankAddress")]
        public int BankAddressId { get; set; }
        public string TransitNumber { get; set; }
        public string InstitutionNumber { get; set; }
        public string AccountNumber { get; set; }
        public string BeneficiaryBankSwiftNumber { get; set; }
        public string IntermediaryBank { get; set; }
        public string IntermediaryBankSwiftNumber { get; set; }

        // Navigation Properties
        public virtual Address BankAddress { get; set; }
    }
}
