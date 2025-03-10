using Humanizer;
using Omnae.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Omnae.BusinessLayer.Models
{
    public class ProductApiViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string AvatarUri { get; set; }
        public HttpPostedFileBase AvatarFile { get; set; }

        public string PartNumber { get; set; }

        public string PartNumberRevision { get; set; }


        public BUILD_TYPE? BuildType { get; set; }

        public MATERIALS_TYPE? Material { get; set; }

        // Metal
        public Precision_Metal? PrecisionMetal { get; set; }

        public Metals_Processes? MetalsProcesses { get; set; }

        public Metal_Type? MetalType { get; set; }

        public Metals_Surface_Finish? MetalsSurfaceFinish { get; set; }


        // Plastics
        public Precision_Plastics? PrecisionPlastics { get; set; }

        public Plastics_Processes? PlasticsProcesses { get; set; }


        // Membrane
        public Switches_Type? MembraneSwitches { get; set; }


        public Print_Type? MembraneSwitchesAttributes { get; set; }

        public Membrane_Switches_Attributes_Waterproof? MembraneSwitchesAttributesWaterproof { get; set; }

        public Membrane_Switches_Attributes_Embossing? MembraneSwitchesAttributesEmbossing { get; set; }

        public Membrane_Switches_Attributes_LEDLighting? MembraneSwitchesAttributesLEDLighting { get; set; }

        public Membrane_Switches_Attributes_LED_EL_Backlighting? MembraneSwitchesAttributesLED_EL_Backlighting { get; set; }


        // Graphic_Overlays

        public Print_Type? GraphicOverlaysAttributes { get; set; }

        public Graphic_Overlays_Attributes_Embossing? GraphicOverlaysAttributesEmbossing { get; set; }

        public Graphic_Overlays_Attributes_SelectiveTexture? GraphicOverlaysAttributesSelectiveTexture { get; set; }

        // Elastomers
        public string Elastomers { get; set; }

        // Labels
        public string Labels { get; set; }


        public string MilledStone { get; set; }

        public string MilledWood { get; set; }

        public string FlexCircuits { get; set; }

        public string CableAssemblies { get; set; }

        public string Others { get; set; }

        public string MetalType_FreeText { get; set; }

        public string SurfaceFinish_FreeText { get; set; }

        public string PlasticType_FreeText { get; set; }



        public int? ToolingLeadTime { get; set; }

        public int? SampleLeadTime { get; set; }

        public int? ProductionLeadTime { get; set; }

        public bool RiskBuild { get; set; }


        public int CustomerPriority { get; set; }

        public List<decimal?> QuantityList { get; set; }
        public decimal? Qty1 { get; set; }
        public decimal? Qty2 { get; set; }
        public decimal? Qty3 { get; set; }
        public decimal? Qty4 { get; set; }
        public decimal? Qty5 { get; set; }
        public decimal? Qty6 { get; set; }
        public decimal? Qty7 { get; set; }

        // Process Type 
        public Process_Type? ProcessType { get; set; }
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

        public int UnitOfMeasurement { get; set; }
    }
}
