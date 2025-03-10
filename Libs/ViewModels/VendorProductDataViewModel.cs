using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Omnae.Common;

namespace Omnae.Libs.ViewModels
{
    public class VendorProductDataViewModel : IValidatableObject
    {
        [Required]
        public string CompanyName { get; set; }

        [Required] 
        public bool ThisProductIsMadeByAVendor { get; set; } = true;

        public BUILD_TYPE BuildType { get; set; }
        public MATERIALS_TYPE Material { get; set; }

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
        public string UnitPrice1 { get; set; }
        public string Quantity2 { get; set; } = null;
        public string UnitPrice2 { get; set; }
        public string Quantity3 { get; set; } = null;
        public string UnitPrice3 { get; set; }
        public string Quantity4 { get; set; } = null;
        public string UnitPrice4 { get; set; }
        public string Quantity5 { get; set; } = null;
        public string UnitPrice5 { get; set; }
        public string Quantity6 { get; set; } = null;
        public string UnitPrice6 { get; set; }
        public string Quantity7 { get; set; } = null;
        public string UnitPrice7 { get; set; }

        public int? SampleLeadTime { get; set; }
        public int? ProductionLeadTime { get; set; }
        public string HamonizedCode { get; set; }

        public string UnitOfMeasurement { get; set; }

        /////////////////////////////////

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(!(decimal.TryParse(Quantity1, out var q1) && q1 >= 0))
                yield return new ValidationResult($"Invalid Quantity: {Quantity1}");
            if (!(double.TryParse(UnitPrice1, out var up1) && up1 >= 0))
                yield return new ValidationResult($"Invalid UnitPrice1: {UnitPrice1}");

            if (!string.IsNullOrWhiteSpace(Quantity2) && !(decimal.TryParse(Quantity2, out var q2) && q2 >= 0))
                yield return new ValidationResult($"Invalid Quantity: {Quantity2}");
            if (!string.IsNullOrWhiteSpace(UnitPrice2) && !(double.TryParse(UnitPrice2, out var up2) && up2 >= 0))
                yield return new ValidationResult($"Invalid UnitPrice2: {UnitPrice2}");

            if (!string.IsNullOrWhiteSpace(Quantity3) && !(decimal.TryParse(Quantity3, out var q3) && q3 >= 0))
                yield return new ValidationResult($"Invalid Quantity: {Quantity3}");
            if (!string.IsNullOrWhiteSpace(UnitPrice3) && !(double.TryParse(UnitPrice3, out var up3) && up3 >= 0))
                yield return new ValidationResult($"Invalid UnitPrice3: {UnitPrice3}");
        
            if (!string.IsNullOrWhiteSpace(Quantity4) && !(decimal.TryParse(Quantity4, out var q4) && q4 >= 0))
                yield return new ValidationResult($"Invalid Quantity: {Quantity4}");
            if (!string.IsNullOrWhiteSpace(UnitPrice4) && !(double.TryParse(UnitPrice4, out var up4) && up4 >= 0))
                yield return new ValidationResult($"Invalid UnitPrice4: {UnitPrice4}");
            
            if (!string.IsNullOrWhiteSpace(Quantity5) && !(decimal.TryParse(Quantity5, out var q5) && q5 >= 0))
                yield return new ValidationResult($"Invalid Quantity: {Quantity5}");
            if (!string.IsNullOrWhiteSpace(UnitPrice5) && !(double.TryParse(UnitPrice5, out var up5) && up5 >= 0))
                yield return new ValidationResult($"Invalid UnitPrice5: {UnitPrice5}");
      
            if (!string.IsNullOrWhiteSpace(Quantity6) && !(decimal.TryParse(Quantity6, out var q6) && q6 >= 0))
                yield return new ValidationResult($"Invalid Quantity: {Quantity6}");
            if (!string.IsNullOrWhiteSpace(UnitPrice6) && !(double.TryParse(UnitPrice6, out var up6) && up6 >= 0))
                yield return new ValidationResult($"Invalid UnitPrice6: {UnitPrice6}");
       
            if (!string.IsNullOrWhiteSpace(Quantity7) && !(decimal.TryParse(Quantity7, out var q7) && q7 >= 0))
                yield return new ValidationResult($"Invalid Quantity: {Quantity7}");
            if (!string.IsNullOrWhiteSpace(UnitPrice7) && !(double.TryParse(UnitPrice7, out var up7) && up7 >= 0))
                yield return new ValidationResult($"Invalid UnitPrice7: {UnitPrice7}");

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