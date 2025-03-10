using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;
using Omnae.WebApi.Util;
using Omnae.Common;

namespace Omnae.WebApi.Controllers
{
    /// <summary>
    /// Core Api for looking up Enums 
    /// </summary>
    [EnableCors("*", "*", "*")]
    [RoutePrefix("api/Lookups")]
    public class LookupsController : ApiController
    {
        //// GET: Lookups
        //public ActionResult Index()
        //{
        //    return View();
        //}

        /// <summary>
        /// Get all States Lookup
        /// </summary>
        [HttpGet]
        [Route("States")]
        public Dictionary<int, string> GetStates()
        {
            var dict = EnumExtentions.ToDictionary<States>();
            return dict;
        }

        /// <summary>
        /// Get all BuildType Lookup
        /// </summary>
        [HttpGet]
        [Route("BuildType")]
        public Dictionary<int, string> GetBuildType()
        {
            var dict = EnumExtentions.ToDictionary<BUILD_TYPE>();
            return dict;
        }

        /// <summary>
        /// Get all MaterialsType Lookup
        /// </summary>
        [HttpGet]
        [Route("MaterialsType")]
        public Dictionary<int, string> GetMaterialsType()
        {
            var dict = EnumExtentions.ToDictionary<MATERIALS_TYPE>();
            return dict;
        }

        /// <summary>
        /// Get all PrecisionMetal Lookup
        /// </summary>
        [HttpGet]
        [Route("PrecisionMetal")]
        public Dictionary<int, string> GetPrecisionMetal()
        {
            var dict = EnumExtentions.ToDictionary<Precision_Metal>();
            return dict;
        }

        /// <summary>
        /// Get all MetalsProcesses Lookup
        /// </summary>
        [HttpGet]
        [Route("MetalsProcesses")]
        public Dictionary<int, string> GetMetalsProcesses()
        {
            var dict = EnumExtentions.ToDictionary<Metals_Processes>();
            return dict;
        }

        /// <summary>
        /// Get all MetalType Lookup
        /// </summary>
        [HttpGet]
        [Route("MetalType")]
        public Dictionary<int, string> GetMetalType()
        {
            var dict = EnumExtentions.ToDictionary<Metal_Type>();
            return dict;
        }

        /// <summary>
        /// Get all MetalsSurfaceFinish Lookup
        /// </summary>
        [HttpGet]
        [Route("MetalsSurfaceFinish")]
        public Dictionary<int, string> GetMetalsSurfaceFinish()
        {
            var dict = EnumExtentions.ToDictionary<Metals_Surface_Finish>();
            return dict;
        }

        /// <summary>
        /// Get all PrecisionPlastics Lookup
        /// </summary>
        [HttpGet]
        [Route("PrecisionPlastics")]
        public Dictionary<int, string> GetPrecisionPlastics()
        {
            var dict = EnumExtentions.ToDictionary<Precision_Plastics>();
            return dict;
        }


        /// <summary>
        /// Get all PlasticsProcesses Lookup
        /// </summary>
        [HttpGet]
        [Route("PlasticsProcesses")]
        public Dictionary<int, string> GetPlasticsProcesses()
        {
            var dict = EnumExtentions.ToDictionary<Plastics_Processes>();
            return dict;
        }


        /// <summary>
        /// Get all SwitchesType Lookup
        /// </summary>
        [HttpGet]
        [Route("SwitchesType")]
        public Dictionary<int, string> GetSwitchesType()
        {
            var dict = EnumExtentions.ToDictionary<Switches_Type>();
            return dict;
        }


        /// <summary>
        /// Get all PrintType Lookup
        /// </summary>
        [HttpGet]
        [Route("PrintType")]
        public Dictionary<int, string> GetPrintType()
        {
            var dict = EnumExtentions.ToDictionary<Print_Type>();
            return dict;
        }

        /// <summary>
        /// Get all MembraneSwitchesAttributesWaterproof Lookup
        /// </summary>
        [HttpGet]
        [Route("MembraneSwitchesAttributesWaterproof")]
        public Dictionary<int, string> GetMembraneSwitchesAttributesWaterproof()
        {
            var dict = EnumExtentions.ToDictionary<Membrane_Switches_Attributes_Waterproof>();
            return dict;
        }

        /// <summary>
        /// Get all MembraneSwitchesAttributesEmbossing Lookup
        /// </summary>
        [HttpGet]
        [Route("MembraneSwitchesAttributesEmbossing")]
        public Dictionary<int, string> GetMembraneSwitchesAttributesEmbossing()
        {
            var dict = EnumExtentions.ToDictionary<Membrane_Switches_Attributes_Embossing>();
            return dict;
        }

        /// <summary>
        /// Get all MembraneSwitchesAttributesLedlighting Lookup
        /// </summary>
        [HttpGet]
        [Route("MembraneSwitchesAttributesLedlighting")]
        public Dictionary<int, string> GetMembraneSwitchesAttributesLedlighting()
        {
            var dict = EnumExtentions.ToDictionary<Membrane_Switches_Attributes_LEDLighting>();
            return dict;
        }

        /// <summary>
        /// Get all MembraneSwitchesAttributesLedElBacklighting Lookup
        /// </summary>
        [HttpGet]
        [Route("MembraneSwitchesAttributesLedElBacklighting")]
        public Dictionary<int, string> GetMembraneSwitchesAttributesLedElBacklighting()
        {
            var dict = EnumExtentions.ToDictionary<Membrane_Switches_Attributes_LED_EL_Backlighting>();
            return dict;
        }

        /// <summary>
        /// Get all GraphicOverlaysAttributesEmbossing Lookup
        /// </summary>
        [HttpGet]
        [Route("GraphicOverlaysAttributesEmbossing")]
        public Dictionary<int, string> GetGraphicOverlaysAttributesEmbossing()
        {
            var dict = EnumExtentions.ToDictionary<Graphic_Overlays_Attributes_Embossing>();
            return dict;
        }

        /// <summary>
        /// Get all GraphicOverlaysAttributesSelectivetexture Lookup
        /// </summary>
        [HttpGet]
        [Route("GraphicOverlaysAttributesSelectivetexture")]
        public Dictionary<int, string> GetGraphicOverlaysAttributesSelectivetexture()
        {
            var dict = EnumExtentions.ToDictionary<Graphic_Overlays_Attributes_SelectiveTexture>();
            return dict;
        }


        /// <summary>
        /// Get all Triggers Lookup
        /// </summary>
        [HttpGet]
        [Route("Triggers")]
        public Dictionary<int, string> GetTriggers()
        {
            var dict = EnumExtentions.ToDictionary<Triggers>();
            return dict;
        }

        /// <summary>
        /// Get all Country Lookup
        /// </summary>
        [HttpGet]
        [Route("Country")]
        public Dictionary<int, string> GetCountry()
        {
            var dict = EnumExtentions.ToDictionary<COUNTRY>();
            return dict;
        }

        /// <summary>
        /// Get all CanadaProvinces Lookup
        /// </summary>
        [HttpGet]
        [Route("CanadaProvinces")]
        public Dictionary<int, string> GetCanadaProvinces()
        {
            var dict = EnumExtentions.ToDictionary<CANADA_PROVINCES>();
            return dict;
        }

        /// <summary>
        /// Get all DistanceUnit Lookup
        /// </summary>
        [HttpGet]
        [Route("DistanceUnit")]
        public Dictionary<int, string> GetDistanceUnit()
        {
            var dict = EnumExtentions.ToDictionary<DISTANCE_UNIT>();
            return dict;
        }

        /// <summary>
        /// Get all MassUnit Lookup
        /// </summary>
        [HttpGet]
        [Route("MassUnit")]
        public Dictionary<int, string> GetMassUnit()
        {
            var dict = EnumExtentions.ToDictionary<MASS_UNIT>();
            return dict;
        }

        /// <summary>
        /// Get all PaymentMethods Lookup
        /// </summary>
        [HttpGet]
        [Route("PaymentMethods")]
        public Dictionary<int, string> GetPaymentMethods()
        {
            var dict = EnumExtentions.ToDictionary<PAYMENT_METHODS>();
            return dict;
        }

        /// <summary>
        /// Get all DocumentType Lookup
        /// </summary>
        [HttpGet]
        [Route("DocumentType")]
        public Dictionary<int, string> GetDocumentType()
        {
            var dict = EnumExtentions.ToDictionary<DOCUMENT_TYPE>();
            return dict;
        }

        /// <summary>
        /// Get all Filters Lookup
        /// </summary>
        [HttpGet]
        [Route("Filters")]
        public Dictionary<int, string> GetFilters()
        {
            var dict = EnumExtentions.ToDictionary<FILTERS>();
            return dict;
        }

        /// <summary>
        /// Get all NcrFilters Lookup
        /// </summary>
        [HttpGet]
        [Route("NcrFilters")]
        public Dictionary<int, string> GetNcrFilters()
        {
            var dict = EnumExtentions.ToDictionary<NCR_FILTERS>();
            return dict;
        }

        /// <summary>
        /// Get all InvoiceFilters Lookup
        /// </summary>
        [HttpGet]
        [Route("InvoiceFilters")]
        public Dictionary<int, string> GetInvoiceFilters()
        {
            var dict = EnumExtentions.ToDictionary<INVOICE_FILTERS>();
            return dict;
        }

        /// <summary>
        /// Get all NcDetectedBy Lookup
        /// </summary>
        [HttpGet]
        [Route("NcDetectedBy")]
        public Dictionary<int, string> GetNcDetectedBy()
        {
            var dict = EnumExtentions.ToDictionary<NC_DETECTED_BY>();
            return dict;
        }

        /// <summary>
        /// Get all NcRootCause Lookup
        /// </summary>
        [HttpGet]
        [Route("NcRootCause")]
        public Dictionary<int, string> GetNcRootCause()
        {
            var dict = EnumExtentions.ToDictionary<NC_ROOT_CAUSE>();
            return dict;
        }

        /// <summary>
        /// Get all NcDisposition Lookup
        /// </summary>
        [HttpGet]
        [Route("NcDisposition")]
        public Dictionary<int, string> GetNcDisposition()
        {
            var dict = EnumExtentions.ToDictionary<NC_DISPOSITION>();
            return dict;
        }

        /// <summary>
        /// Get all CountryId Lookup
        /// </summary>
        [HttpGet]
        [Route("CountryId")]
        public Dictionary<int, string> GetCountryId()
        {
            var dict = EnumExtentions.ToDictionary<COUNTRY_ID>();
            return dict;
        }

        /// <summary>
        /// Get all StateProvinceCode Lookup
        /// </summary>
        [HttpGet]
        [Route("StateProvinceCode")]
        public Dictionary<int, string> GetStateProvinceCode()
        {
            var dict = EnumExtentions.ToDictionary<STATE_PROVINCE_CODE>();
            return dict;
        }

        /// <summary>
        /// Get all CustomerPriority Lookup
        /// </summary>
        [HttpGet]
        [Route("CustomerPriority")]
        public Dictionary<int, string> GetCustomerPriority()
        {
            var dict = EnumExtentions.ToDictionary<CUSTOMER_PRIORITY>();
            return dict;
        }

        /// <summary>
        /// Get all NcrImageType Lookup
        /// </summary>
        [HttpGet]
        [Route("NcrImageType")]
        public Dictionary<int, string> GetNcrImageType()
        {
            var dict = EnumExtentions.ToDictionary<NCR_IMAGE_TYPE>();
            return dict;
        }

        /// <summary>
        /// Get all ShippingCarrierType Lookup
        /// </summary>
        [HttpGet]
        [Route("ShippingCarrierType")]
        public Dictionary<int, string> GetShippingCarrierType()
        {
            var dict = EnumExtentions.ToDictionary<SHIPPING_CARRIER_TYPE>();
            return dict;
        }

        /// <summary>
        /// Get all ShippingCarrier Lookup
        /// </summary>
        [HttpGet]
        [Route("ShippingCarrier")]
        public Dictionary<int, string> GetShippingCarrier()
        {
            var dict = EnumExtentions.ToDictionary<SHIPPING_CARRIER>();
            return dict;
        }

        /// <summary>
        /// Get all CustomerType Lookup
        /// </summary>
        [HttpGet]
        [Route("CustomerType")]
        public Dictionary<int, string> GetCustomerType()
        {
            var dict = EnumExtentions.ToDictionary<CUSTOMER_TYPE>();
            return dict;
        }

        /// <summary>
        /// Get all CurrencyCodes Lookup
        /// </summary>
        [HttpGet]
        [Route("CurrencyCodes")]
        public Dictionary<int, string> GetCurrencyCodes()
        {
            var dict = EnumExtentions.ToDictionary<CurrencyCodes>();
            return dict;
        }
    }
}