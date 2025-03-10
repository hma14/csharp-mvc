using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Omnae.Common;

namespace Omnae.Libs.ViewModels
{
    public class AddExistingProductViewModel : IValidatableObject
    {
        [Required]
        public BUILD_TYPE BuildType { get; set; }
        public MATERIALS_TYPE Material { get; set; }

        public int CustomerId { get; set; }

        [Required]
        public int VendorId { get; set; }

      
        
        [CanBeNull]
        public string PartId { get; set; }
        [CanBeNull]
        public string PartName { get; set; }
        [CanBeNull]
        public string Description { get; set; }

        [Required]
        public string PartNumber { get; set; }
        [Required]
        public string PartRevision { get; set; }
        
        [Required]
        public string Quantity1 { get; set; }
        [Required]
        [Range(0.0, Double.MaxValue)]
        public decimal? UnitPrice1 { get; set; }

        public string Quantity2 { get; set; } = null;
        [Range(0.0, 999_999_999d)]
        public decimal? UnitPrice2 { get; set; }
        public string Quantity3 { get; set; } = null;
        [Range(0.0, 999_999_999d)]
        public decimal? UnitPrice3 { get; set; }
        public string Quantity4 { get; set; } = null;
        [Range(0.0, 999_999_999d)]
        public decimal? UnitPrice4 { get; set; }
        public string Quantity5 { get; set; } = null;
        [Range(0.0, 999_999_999d)]
        public decimal? UnitPrice5 { get; set; }
        public string Quantity6 { get; set; } = null;
        [Range(0.0, 999_999_999d)]
        public decimal? UnitPrice6 { get; set; }
        public string Quantity7 { get; set; } = null;
        [Range(0.0, 999_999_999d)]
        public decimal? UnitPrice7 { get; set; }
        public int? SampleLeadTime { get; set; }
        public int? ProductionLeadTime { get; set; }
        public string HarmonizedCode { get; set; }
        public DateTime ExpireDate { get; set; } = DateTime.UtcNow.AddMonths(12);

        public string UnitOfMeasurement { get; set; }

        public CurrencyCodes? Currency { get; set; }

        /////////////////////////////////

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(!(int.TryParse(Quantity1, out var q1) && q1 >= 0))
                yield return new ValidationResult($"Invalid Quantity: {Quantity1}");

            if (!string.IsNullOrWhiteSpace(Quantity2) && !(int.TryParse(Quantity2, out var q2) && q2 >= 0))
                yield return new ValidationResult($"Invalid Quantity: {Quantity2}");
            if (!string.IsNullOrWhiteSpace(Quantity3) && !(int.TryParse(Quantity3, out var q3) && q3 >= 0))
                yield return new ValidationResult($"Invalid Quantity: {Quantity3}");
            if (!string.IsNullOrWhiteSpace(Quantity4) && !(int.TryParse(Quantity4, out var q4) && q4 >= 0))
                yield return new ValidationResult($"Invalid Quantity: {Quantity4}");
            if (!string.IsNullOrWhiteSpace(Quantity5) && !(int.TryParse(Quantity5, out var q5) && q5 >= 0))
                yield return new ValidationResult($"Invalid Quantity: {Quantity5}");
            if (!string.IsNullOrWhiteSpace(Quantity6) && !(int.TryParse(Quantity6, out var q6) && q6 >= 0))
                yield return new ValidationResult($"Invalid Quantity: {Quantity6}");
            if (!string.IsNullOrWhiteSpace(Quantity7) && !(int.TryParse(Quantity7, out var q7) && q7 >= 0))
                yield return new ValidationResult($"Invalid Quantity: {Quantity7}");

            if (!string.IsNullOrWhiteSpace(Quantity2) && UnitPrice2 == null)
                yield return new ValidationResult($"Invalid UnitPrice2: You must set the UnitPrice2 for the Quantity2.");
            if (!string.IsNullOrWhiteSpace(Quantity3) && UnitPrice3 == null)
                yield return new ValidationResult($"Invalid UnitPrice3: You must set the UnitPrice3 for the Quantity3.");
            if (!string.IsNullOrWhiteSpace(Quantity4) && UnitPrice4 == null)
                yield return new ValidationResult($"Invalid UnitPrice4: You must set the UnitPrice4 for the Quantity4.");
            if (!string.IsNullOrWhiteSpace(Quantity5) && UnitPrice5 == null)
                yield return new ValidationResult($"Invalid UnitPrice5: You must set the UnitPrice5 for the Quantity5.");
            if (!string.IsNullOrWhiteSpace(Quantity6) && UnitPrice6 == null)
                yield return new ValidationResult($"Invalid UnitPrice6: You must set the UnitPrice6 for the Quantity6.");
            if (!string.IsNullOrWhiteSpace(Quantity7) && UnitPrice7 == null)
                yield return new ValidationResult($"Invalid UnitPrice7: You must set the UnitPrice7 for the Quantity7.");


            yield break;
        }
    }
}