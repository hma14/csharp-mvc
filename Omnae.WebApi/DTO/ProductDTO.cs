using Humanizer;
using Omnae.BusinessLayer.Models;
using Omnae.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Omnae.WebApi.DTO
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public States State { get; set; }

        [Display(Name = "Product Name")]
        public string  Name { get; set; }
        public string  Description { get; set; }

        public string AvatarUri { get; set; }

        [Display(Name = "Admin Id")]
        public string AdminId { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; }


        [Display(Name = "Vendor Id")]
        public int? VendorId { get; set; }
        public string VendorName { get; set; }

        [Display(Name = "Price Break")]
        public int? PriceBreakId { get; set; }

        public string PartNumber { get; set; }

        public string PartNumberRevision { get; set; }

        [Display(Name = "Parent Revision")]
        [StringLength(50)]
        public string ParentPartNumberRevision { get; set; }

        public int? PartRevisionId { get; set; }
        public int? ParentPartRevisionId { get; set; }

        [Display(Name = "Build Type")]
        public BUILD_TYPE BuildType { get; set; }

        [Display(Name = "Material")]
        public MATERIALS_TYPE Material { get; set; }

        // Metal
        [Display(Name = "Precision Metal")]
        public Precision_Metal? PrecisionMetal { get; set; }

        [Display(Name = "Processes")]
        public Metals_Processes? MetalsProcesses { get; set; }

        [Display(Name = "Metal Type")]
        public Metal_Type? MetalType { get; set; }

        [Display(Name = "Surface Finish")]
        public Metals_Surface_Finish? MetalsSurfaceFinish { get; set; }


        // Plastics
        [Display(Name = "Precision Plastics")]
        public Precision_Plastics? PrecisionPlastics { get; set; }

        [Display(Name = "Plastics Processes")]
        public Plastics_Processes? PlasticsProcesses { get; set; }


        // Membrane
        [Display(Name = "Switches Type")]
        public Switches_Type? MembraneSwitches { get; set; }

        [Display(Name = "Print Type")]
        public Print_Type? MembraneSwitchesAttributes { get; set; }

        [Display(Name = "Waterproof")]
        public Membrane_Switches_Attributes_Waterproof? MembraneSwitchesAttributesWaterproof { get; set; }

        [Display(Name = "Embossing")]
        public Membrane_Switches_Attributes_Embossing? MembraneSwitchesAttributesEmbossing { get; set; }

        [Display(Name = "LEDLighting")]
        public Membrane_Switches_Attributes_LEDLighting? MembraneSwitchesAttributesLEDLighting { get; set; }

        [Display(Name = "LED EL Backlighting")]
        public Membrane_Switches_Attributes_LED_EL_Backlighting? MembraneSwitchesAttributesLED_EL_Backlighting { get; set; }


        // Graphic_Overlays
        [Display(Name = "Print Type")]
        public Print_Type? GraphicOverlaysAttributes { get; set; }

        [Display(Name = "Graphic Overlays Attr. Embossing")]
        public Graphic_Overlays_Attributes_Embossing? GraphicOverlaysAttributesEmbossing { get; set; }

        [Display(Name = "Graphic Overlays Attr. SelectiveTexture")]
        public Graphic_Overlays_Attributes_SelectiveTexture? GraphicOverlaysAttributesSelectiveTexture { get; set; }

        // Elastomers
        [Display(Name = "Elastomers")]
        [StringLength(250)]
        public string Elastomers { get; set; }

        // Labels
        [Display(Name = "Labels")]
        [StringLength(250)]
        public string Labels { get; set; }

        // Others
        [Display(Name = "Milled Stone")]
        public string MilledStone { get; set; }

        [Display(Name = "Milled Wood")]
        [StringLength(250)]
        public string MilledWood { get; set; }

        [Display(Name = "Flex Circuits")]
        [StringLength(250)]
        public string FlexCircuits { get; set; }

        [Display(Name = "Cable Assemblies")]
        [StringLength(250)]
        public string CableAssemblies { get; set; }

        public string Others { get; set; }

        [Display(Name = "Metal Type")]
        [DataType(DataType.Text)]
        public string MetalType_FreeText { get; set; }

        [Display(Name = "Surface Finish")]
        [DataType(DataType.Text)]
        public string SurfaceFinish_FreeText { get; set; }

        [Display(Name = "Plastic Type")]
        [DataType(DataType.Text)]
        public string PlasticType_FreeText { get; set; }

        public int? QuoteId { get; set; }

        [Display(Name = "Tooling Lead Time")]
        public int? ToolingLeadTime { get; set; }

        [Display(Name = "Sample Lead Time")]
        public int? SampleLeadTime { get; set; }

        [Display(Name = "Prod. Lead Time")]
        public int? ProductionLeadTime { get; set; }

        [Display(Name = "Tooling Charges")]
        public decimal? ToolingSetupCharges { get; set; }
        public decimal? CustomerToolingSetupCharges { get; set; }
        
        public int Status { get; set; }

        [Display(Name = "Harmonized Code")]
        public string HarmonizedCode { get; set; }

        public int? RFQQuantityId { get; set; }
        public int? ExtraQuantityId { get; set; }
        public DateTime? CreatedDate { get; set; }

        //public int? PartVolume { get; set; }
        public string CustomerPriority { get; set; }

        public decimal? UnitPrice { get; set; }

        public Quantities Quantities { get; set; }
        public string[] Product2DDocUri { get; set; }
        public string[] Product3DDocUri { get; set; }
        public DateTime ModifiedUtc { get; set; }

        [Display(Name = "Processe Type")]
        public Process_Type? ProcessType { get; set; }

        [Display(Name = "Anodizing Type")]
        public Anodizing_Type? AnodizingType { get; set; }
        public bool? ActionRequired { get; set; }

        public int? OriginProductId { get; set; }
        /// <summary>
        /// The CurrencyCodes ISO Number
        /// </summary>
        public CurrencyCodes? PreferredCurrency { get; set; }

        /// <summary>
        /// The CurrencyCodes ISO 3 letter Code
        /// </summary>
        public string PreferredCurrencyText
        {
            get => PreferredCurrency?.Humanize();
            set => PreferredCurrency = (CurrencyCodes?)(string.IsNullOrEmpty(value) ? null : Enum.Parse(typeof(CurrencyCodes), value, true));
        }

        // Unit of Quantity
        public int UnitOfMeasurement { get; set; }
    }

    //public class Quantities    {
    //    public int? Qty1 { get; set; }

    //    public int? Qty2 { get; set; }

    //    public int? Qty3 { get; set; }

    //    public int? Qty4 { get; set; }

    //    public int? Qty5 { get; set; }

    //    public int? Qty6 { get; set; }

    //    public int? Qty7 { get; set; }
    //}
}
