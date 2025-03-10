using System;

namespace Omnae.Libs.ViewModels
{
    public class FileDataViewModel
    {
        
        public string Name { get; set; }
        public string Description { get; set; }
        public string PartNumber { get; set; }
        public string PartNumberRevision { get; set; }
        public int StateId { get; set; }
        public string AvatarFilePath { get; set; }
        public string Doc2D { get; set; }
        public string Doc3D { get; set; }
        public string DocProof { get; set; }
        public string AdminId { get; set; }
        public int? VendorId { get; set; }
        public int? PartVolume { get; set; }
        public int PriceBreak_Amount1 { get; set; }
        public decimal PriceBreak_UnitPrice1 { get; set; }
        public decimal Vendor_PriceBreak_UnitPrice1 { get; set; }
        public int? PriceBreak_Amount2 { get; set; }
        public decimal? PriceBreak_UnitPrice2 { get; set; }
        public decimal? Vendor_PriceBreak_UnitPrice2 { get; set; }
        public int? PriceBreak_Amount3 { get; set; }
        public decimal? PriceBreak_UnitPrice3 { get; set; }
        public decimal? Vendor_PriceBreak_UnitPrice3 { get; set; }
        public int? PriceBreak_Amount4 { get; set; }
        public decimal? PriceBreak_UnitPrice4 { get; set; }
        public decimal? Vendor_PriceBreak_UnitPrice4 { get; set; }
        public int? PriceBreak_Amount5 { get; set; }
        public decimal? PriceBreak_UnitPrice5 { get; set; }
        public decimal? Vendor_PriceBreak_UnitPrice5 { get; set; }
        public int? PriceBreak_Amount6 { get; set; }
        public decimal? PriceBreak_UnitPrice6 { get; set; }
        public decimal? Vendor_PriceBreak_UnitPrice6 { get; set; }
        public int? PriceBreak_Amount7 { get; set; }
        public decimal? PriceBreak_UnitPrice7 { get; set; }
        public decimal? Vendor_PriceBreak_UnitPrice7 { get; set; }
        public int BuildType { get; set; }
        public int Material { get; set; }
        public int? PrecisionMetal { get; set; }
        public int? MetalsProcesses { get; set; }
        public int? MetalType { get; set; }
        public int? MetalsSurfaceFinish { get; set; }
        public int? PrecisionPlastics { get; set; }
        public int? PlasticsProcesses { get; set; }
        public int? MembraneSwitches { get; set; }
        public string Elastomers { get; set; }
        public string Labels { get; set; }
        public string Others { get; set; }
        public string MetalType_FreeText { get; set; }
        public string SurfaceFinish_FreeText { get; set; }
        public string PlasticType_FreeText { get; set; }

        public int? QuoteId { get; set; }
        public int? ToolingLeadTime { get; set; }
        public int SampleLeadTime { get; set; }
        public int ProductionLeadTime { get; set; }
        public decimal ToolingSetupCharges { get; set; }
        public int? Status { get; set; }

        public string HarmonizedCode { get; set; }
        public int? MembraneSwitchesAttributesWaterproof { get; set; }
        public int? MembraneSwitchesAttributesEmbossing { get; set; }
        public int? MembraneSwitchesAttributesLEDLighting { get; set; }
        public int? MembraneSwitchesAttributesLED_EL_Backlighting { get; set; }
        public int? GraphicOverlaysAttributesEmbossing { get; set; }
        public int? GraphicOverlaysAttributesSelectiveTexture { get; set; }
        public int? MembraneSwitchesAttributes { get; set; }
        public int? GraphicOverlaysAttributes { get; set; }
        public string MilledStone { get; set; }
        public string MilledWood { get; set; }
        public string FlexCircuits { get; set; }
        public string CableAssemblies { get; set; }
      
    }
}
