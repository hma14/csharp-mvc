using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Humanizer;
using Omnae.Common;

namespace Omnae.BusinessLayer.Models
{
    public class ProductViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Part name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Part description is required")]
        public string Description { get; set; }

        [Display(Name = "Avatar Uri")]
        public string AvatarUri { get; set; }
        public HttpPostedFileBase AvatarFile { get; set; }

        [Required(ErrorMessage = "Part Number is required")]
        [Display(Name = "Part Number")]
        public string PartNumber { get; set; }

        [Required(ErrorMessage = "Part Number Revision is required")]
        [Display(Name = "Part Number Revision")]
        public string PartNumberRevision { get; set; }


        [Display(Name = "Build Type")]
        public BUILD_TYPE? BuildType { get; set; }

        [Display(Name = "Select Material")]
        public MATERIALS_TYPE? Material { get; set; }

        // Metal
        [Display(Name = "Precision Metal")]
        public Precision_Metal? PrecisionMetal { get; set; }

        [Display(Name = "Metals Processes")]
        public Metals_Processes? MetalsProcesses { get; set; }

        [Display(Name = "Metal Type")]
        public Metal_Type? MetalType { get; set; }

        [Display(Name = "Metals Surf Finish")]
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

        [Display(Name = "Embossing")]
        public Graphic_Overlays_Attributes_Embossing? GraphicOverlaysAttributesEmbossing { get; set; }

        [Display(Name = "Selective Texture")]
        public Graphic_Overlays_Attributes_SelectiveTexture? GraphicOverlaysAttributesSelectiveTexture { get; set; }

        // Elastomers
        [Display(Name = "Elastomers")]
        public string Elastomers { get; set; }

        // Labels
        [Display(Name = "Labels")]
        public string Labels { get; set; }


        [Display(Name = "Milled Stone")]
        public string MilledStone { get; set; }

        [Display(Name = "Milled Wood")]
        public string MilledWood { get; set; }

        [Display(Name = "Flex Circuits")]
        public string FlexCircuits { get; set; }

        [Display(Name = "Cable Assemblies")]
        public string CableAssemblies { get; set; }

        public string Others { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Metal Type")]
        public string MetalType_FreeText { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Surface Finish")]
        public string SurfaceFinish_FreeText { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Plastic Type")]
        public string PlasticType_FreeText { get; set; }



        [Display(Name = "Desired Tooling Lead Time (business days)")]
        public int? ToolingLeadTime { get; set; }

        [Display(Name = "Sample Lead Time (business days)")]
        public int? SampleLeadTime { get; set; }

        [Display(Name = "Production Lead Time (business days)")]
        public int? ProductionLeadTime { get; set; }

        [Display(Name = "Risk Build")]
        public bool RiskBuild { get; set; }


        [Display(Name = "Customer Priority")]
        public int CustomerPriority { get; set; }

        public List<decimal?> QuantityList { get; set; }


        public IEnumerable<SelectListItem> CustomerPriorityDdl { get; set; }

        //public DocumentProdIdViewModel ProductDoc { get; set; }

        public List<HttpPostedFileBase> ProductDoc { get; set; }

        // Process Type

        [Display(Name = "Process Type")]
        public Process_Type? ProcessType { get; set; }

        [Display(Name = "Anodizing Type")]
        public Anodizing_Type? AnodizingType { get; set; }

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

        // Unit of Measurement

        public int UnitOfMeasurement { get; set; }
    }
}