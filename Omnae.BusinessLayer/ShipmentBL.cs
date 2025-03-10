using System.Configuration;
using System.IO;
using Omnae.Common;
using Omnae.ShippingAPI.DHL.Libs;
using Omnae.ShippingAPI.DHL.Models;

namespace Omnae.BusinessLayer
{
    public class ShipmentBL
    {

        public double GetSalesTax(Model.Models.Address addr)
        {
            if (addr.CountryId != (int)COUNTRY.CANADA)
                return TAX_RATE.EXEMPT_TAX;

            switch (addr.StateProvinceId)
            {
                case (int)CANADA_PROVINCES.Alberta:
                case (int)CANADA_PROVINCES.British_Columbia:
                case (int)CANADA_PROVINCES.Nunavut:
                case (int)CANADA_PROVINCES.Northwest_Territories:
                case (int)CANADA_PROVINCES.Yukon_Territory:
                    return TAX_RATE.BC_TAX;
                case (int)CANADA_PROVINCES.Ontario:
                    return TAX_RATE.ON_TAX;
                case (int)CANADA_PROVINCES.New_Brunswick:
                    return TAX_RATE.NB_TAX;
                case (int)CANADA_PROVINCES.Newfoundland_and_Labrador:
                    return TAX_RATE.NL_TAX;
                case (int)CANADA_PROVINCES.Nova_Scotia:
                    return TAX_RATE.NS_TAX;
                case (int)CANADA_PROVINCES.Prince_Edward_Island:
                    return TAX_RATE.PE_TAX;
                case (int)CANADA_PROVINCES.Manitoba:
                    return TAX_RATE.MB_TAX;
                case (int)CANADA_PROVINCES.Québec:
                    return TAX_RATE.QC_TAX;
                case (int)CANADA_PROVINCES.Saskatchewan:
                    return TAX_RATE.SK_TAX;

                default:
                    return TAX_RATE.EXEMPT_TAX;
            }
        }

        public DHLResponse ShipmentTracking(string id)
        {
            string xmlFile = ConfigurationManager.AppSettings["TrackingRequest"];
            string xmlWriteToFile = ConfigurationManager.AppSettings["WriteToTrackingRequest"];
            string filePath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath(@"~/DHLShippingXml/"), xmlFile);
            string fileWriteToPath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath(@"~/DHLShippingXml/"), xmlWriteToFile);
            string url = ConfigurationManager.AppSettings["DHLWebApi"];

            var requestTracking = SetupTrackingRequest(id);
            DHLApi.SetupRequest(filePath, fileWriteToPath, requestTracking);
            string responseString = DHLApi.XmlRequest(url, fileWriteToPath).Result;
            DHLResponse resp = DHLApi.XmlResponse(responseString, REQUESTS.TRACKING);
            return resp;
        }

        private RequestBase SetupTrackingRequest(string trackingNumber)
        {
            RequestBase requestTracking = new RequestTracking()
            {
                RequestType = REQUESTS.TRACKING,
                LanguageCode = "en",
                Waybill = trackingNumber,
                //LPNumber = "JD0144549751510007712",               
                LevelOfDetails = "ALL_CHECK_POINTS",
                PiecesEnabled = "B"
            };

            return requestTracking;
        }
    }
}