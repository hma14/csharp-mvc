
using Omnae.ShippingAPI.DHL.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Omnae.ShippingAPI.DHL.Libs
{
    public enum REQUESTS
    {
        CAPABILITY = 1,
        IMAGE_UPLOAD,
        PICKUP,
        ROUTING,
        SHIPMENT,
        TRACKING
    }


    public class DHLApi
    {
        private static AutoResetEvent allDone = new AutoResetEvent(false);
        private static byte[] RequestBytes { get; set; }

        public static  Task<string> XmlRequest(string url, string filePath)
        {
            string xmlText = File.ReadAllText(filePath);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            RequestBytes = Encoding.GetEncoding("UTF-8").GetBytes(xmlText);
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = WebRequestMethods.Http.Post;
            request.Timeout = 20000;
            request.Proxy = null;

            request.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), request);

            // Keep the main thread from continuing while the asynchronous 
            // operation completes. A real world application 
            // could do something useful such as updating its user interface. 
            allDone.WaitOne(1000);

            Task<WebResponse> task = Task.Factory.FromAsync(
               request.BeginGetResponse,
               asyncResult => request.EndGetResponse(asyncResult),
               (object)null);

            return task.ContinueWith(t => ReadStreamFromResponse(t.Result));
        }


        private static void GetRequestStreamCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

            // End the operation
            //request.SendChunked = true;
            Stream postStream = request.EndGetRequestStream(asynchronousResult);

            // Write to the request stream.
            postStream.Write(RequestBytes, 0, RequestBytes.Length);
            postStream.Close();
        }



        private static string ReadStreamFromResponse(WebResponse response)
        {
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader sr = new StreamReader(responseStream))
            {
                //Need to return this response 
                string strContent = sr.ReadToEnd();
                return strContent;
            }
        }

        public static DHLResponse XmlResponse(string respString, REQUESTS reqType)
        {
            DHLResponse resp = new DHLResponse();
            switch (reqType)
            {
                case REQUESTS.CAPABILITY:
                    resp.Quote = ParseXmlCapability(respString);
                    break;
                case REQUESTS.IMAGE_UPLOAD:

                    break;
                case REQUESTS.PICKUP:

                    break;
                case REQUESTS.ROUTING:

                    break;
                case REQUESTS.SHIPMENT:

                    break;
                case REQUESTS.TRACKING:
                    resp.Trackings = ParseXmlTracking(respString);

                    break;
                default:
                    break;
            }
            resp.RequestType = reqType;
            return resp;
        }

        public static void SetupRequest(string filePath, string inputPath, RequestBase model)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            // Set ServiceHeader
            XmlNode messageTime = doc.SelectSingleNode("//MessageTime");
            XmlNode messageReference = doc.SelectSingleNode("//MessageReference");
            XmlNode siteID = doc.SelectSingleNode("//SiteID");
            XmlNode pwd = doc.SelectSingleNode("//Password");

            // Set values to elements in ServiceHeader pull out from above

            messageTime.InnerText = DateTime.Now.ToString("o");
            messageReference.InnerText = ConfigurationManager.AppSettings["MessageReference"];
            siteID.InnerText = ConfigurationManager.AppSettings["DHL_SiteID"];
            pwd.InnerText = ConfigurationManager.AppSettings["DHL_Password"];

            if (model.RequestType == REQUESTS.CAPABILITY)
            {
                SetupQuoteRequest(inputPath, ref doc, (RequestQuote)model);
            }
            else if (model.RequestType == REQUESTS.TRACKING)
            {
                SetupTrackingRequest(inputPath, ref doc, (RequestTracking)model);
            }
        }

        private static void SetupQuoteRequest(string filePath, ref XmlDocument doc, RequestQuote model)
        {

            XmlNode FromCountryCode = doc.SelectSingleNode("//From//CountryCode");
            FromCountryCode.InnerText = model.Origin.CountryCode;

            XmlNode FromPostalcode = doc.SelectSingleNode("//From//Postalcode");
            if (FromPostalcode != null)
            {
                FromPostalcode.InnerText = model.Origin.Postalcode;
            }
            XmlNode FromCity = doc.SelectSingleNode("//From//City");
            if (FromCity != null)
            {
                FromCity.InnerText = model.Origin.City;
            }
            XmlNode paymentAccountNumber = doc.SelectSingleNode("//BkgDetails//PaymentAccountNumber");
            if (paymentAccountNumber != null)
            {
                paymentAccountNumber.InnerText = model.BkgDetails.PaymentAccountNumber;
            }

            XmlNode paymentCountryCode = doc.SelectSingleNode("//BkgDetails//PaymentCountryCode");
            XmlNode date = doc.SelectSingleNode("//BkgDetails//Date");
            XmlNode readyTime = doc.SelectSingleNode("//BkgDetails//ReadyTime");
            XmlNode readyTimeGMTOffset = doc.SelectSingleNode("//BkgDetails//ReadyTimeGMTOffset");
            XmlNode dimensionUnit = doc.SelectSingleNode("//BkgDetails//DimensionUnit");
            XmlNode weightUnit = doc.SelectSingleNode("//BkgDetails//WeightUnit");
            XmlNode shipmentWeight = doc.SelectSingleNode("//BkgDetails//ShipmentWeight");
            XmlNode volume = doc.SelectSingleNode("//BkgDetails//Volume");

            XmlNode pieceID = doc.SelectSingleNode("//Pieces//Piece//PieceID");
            XmlNode height = doc.SelectSingleNode("//Pieces//Piece//Height");

            height.InnerText = model.BkgDetails.Pieces[0].Height.ToString();

            XmlNode depth = doc.SelectSingleNode("//Pieces//Piece//Depth");
            depth.InnerText = model.BkgDetails.Pieces[0].Depth.ToString();

            XmlNode width = doc.SelectSingleNode("//Pieces//Piece//Width");
            XmlNode weight = doc.SelectSingleNode("//Pieces//Piece//Weight");

            width.InnerText = model.BkgDetails.Pieces[0].Width.ToString();
            weight.InnerText = model.BkgDetails.Pieces[0].Weight.ToString();

            if (model.BkgDetails.IsDutiable.Equals("Y"))
            {
                XmlNode isDutiable = doc.SelectSingleNode("//BkgDetails//IsDutiable");
                isDutiable.InnerText = model.BkgDetails.IsDutiable;

                XmlNode networkTypeCode = doc.SelectSingleNode("//BkgDetails//NetworkTypeCode");
                if (networkTypeCode != null)
                {
                    networkTypeCode.InnerText = model.BkgDetails.NetworkTypeCode;
                }

                XmlNode declaredCurrency = doc.SelectSingleNode("//Dutiable//DeclaredCurrency");
                XmlNode declaredValue = doc.SelectSingleNode("//Dutiable//DeclaredValue");
                declaredCurrency.InnerText = model.Dutiable.DeclaredCurrency;
                declaredValue.InnerText = model.Dutiable.DeclaredValue.ToString();

                isDutiable.InnerText = model.BkgDetails.IsDutiable;

                if (networkTypeCode != null)
                {
                    networkTypeCode.InnerText = model.BkgDetails.NetworkTypeCode;
                }
            }

            XmlNode ToCountryCode = doc.SelectSingleNode("//To//CountryCode");
            ToCountryCode.InnerText = model.Destination?.CountryCode;

            XmlNode ToPostalcode = doc.SelectSingleNode("//To//Postalcode");
            XmlNode ToCity = doc.SelectSingleNode("//To//City");
            if (ToPostalcode != null)
            {
                ToPostalcode.InnerText = model.Destination.Postalcode;
            }
            if (ToCity != null)
            {
                ToCity.InnerText = model.Destination.City;
            }


            // Set values to elements pull out from above


            paymentCountryCode.InnerText = model.BkgDetails.PaymentCountryCode;
            date.InnerText = model.BkgDetails.Date;
            readyTime.InnerText = model.BkgDetails.ReadyTime;
            readyTimeGMTOffset.InnerText = model.BkgDetails.ReadyTimeGMTOffset;
            dimensionUnit.InnerText = model.BkgDetails.DimensionUnit;
            weightUnit.InnerText = model.BkgDetails.WeightUnit;
            shipmentWeight.InnerText = model.BkgDetails.ShipmentWeight != null ? model.BkgDetails.ShipmentWeight.Value.ToString() : null;
            volume.InnerText = model.BkgDetails.Volume.ToString();


            if (model.BkgDetails.Pieces.Count > 1)
            {
                XmlNode Pieces = doc.SelectSingleNode("//Pieces");
                foreach (var piece in model.BkgDetails.Pieces.Skip(1))
                {
                    if (piece.Height == 0)
                    {
                        break;
                    }
                    XmlNode Piece = doc.CreateNode(XmlNodeType.Element, "Piece", "");

                    XmlNode PieceID = doc.CreateNode(XmlNodeType.Element, "PieceID", "");
                    PieceID.InnerText = piece.PieceID.ToString();

                    XmlNode Height = doc.CreateNode(XmlNodeType.Element, "Height", "");
                    Height.InnerText = piece.Height.ToString();

                    XmlNode Depth = doc.CreateNode(XmlNodeType.Element, "Depth", "");
                    Depth.InnerText = piece.Depth.ToString();

                    XmlNode Width = doc.CreateNode(XmlNodeType.Element, "Width", "");
                    Width.InnerText = piece.Width.ToString();

                    XmlNode Weight = doc.CreateNode(XmlNodeType.Element, "Weight", "");
                    Weight.InnerText = piece.Weight.ToString();

                    Piece.AppendChild(PieceID);
                    Piece.AppendChild(Height);
                    Piece.AppendChild(Depth);
                    Piece.AppendChild(Width);
                    Piece.AppendChild(Weight);

                    Pieces.AppendChild(Piece);
                }
            }



            XmlNode globalProductCode = doc.SelectSingleNode("//BkgDetails//QtdShp//GlobalProductCode");
            if (model.BkgDetails.QtdShp != null && globalProductCode != null && model.BkgDetails.QtdShp.GlobalProductCode != null)
            {
                globalProductCode.InnerText = model.BkgDetails.QtdShp.GlobalProductCode;
            }
            XmlNode localProductCode = doc.SelectSingleNode("//BkgDetails//QtdShp//LocalProductCode");
            if (model.BkgDetails.QtdShp != null && localProductCode != null)
            {
                XmlNode specialServiceType = doc.SelectSingleNode("//BkgDetails//QtdShp//QtdShpExChrg//SpecialServiceType");
                if (specialServiceType != null)
                {
                    XmlNode localSpecialServiceType = doc.SelectSingleNode("//BkgDetails//QtdShp//QtdShpExChrg//LocalSpecialServiceType");
                    specialServiceType.InnerText = model.BkgDetails.QtdShp.QtdShpExChrg_SpecialServiceType;
                    localSpecialServiceType.InnerText = model.BkgDetails.QtdShp.QtdShpExChrg_LocalSpecialServiceType;
                }

                localProductCode.InnerText = model.BkgDetails.QtdShp.LocalProductCode;
            }
            XmlNode insuredValue = doc.SelectSingleNode("//BkgDetails//InsuredValue");
            XmlNode insuredCurrency = doc.SelectSingleNode("//BkgDetails//InsuredCurrency");
            if (insuredValue != null)
            {
                insuredValue.InnerText = model.BkgDetails.InsuredValue;
                insuredCurrency.InnerText = model.BkgDetails.InsuredCurrency;
            }

            doc.Save(filePath);

        }

        private static void SetupTrackingRequest(string filePath, ref XmlDocument doc, RequestTracking model)
        {

            XmlNode languageCode = doc.SelectSingleNode("//LanguageCode");
            languageCode.InnerText = model.LanguageCode;
            XmlNode waybill = doc.SelectSingleNode("//AWBNumber");
            if (waybill != null)
            {
                waybill.InnerText = model.Waybill;
            }
            XmlNode lPNumber = doc.SelectSingleNode("//LPNumber");
            if (lPNumber != null)
            {
                lPNumber.InnerText = model.LPNumber;
            }

            XmlNode levelOfDetails = doc.SelectSingleNode("//LevelOfDetails");
            levelOfDetails.InnerText = model.LevelOfDetails;
            XmlNode piecesEnabled = doc.SelectSingleNode("//PiecesEnabled");
            if (piecesEnabled != null)
            {
                piecesEnabled.InnerText = model.PiecesEnabled;
            }

            doc.Save(filePath);
        }

        public static void SetupCriteriaToRequestXml(string filePath)
        {
            // Create the XmlDocument.
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            XmlNode messageTime = doc.SelectSingleNode("//MessageTime");
            XmlNode messageReference = doc.SelectSingleNode("//MessageReference");
            XmlNode siteID = doc.SelectSingleNode("//SiteID");
            XmlNode pwd = doc.SelectSingleNode("//Password");
            XmlNode date = doc.SelectSingleNode("//Date");

            messageTime.InnerText = DateTime.Now.ToString("o");
            messageReference.InnerText = "0987654321098765432109876543";
            siteID.InnerText = ConfigurationManager.AppSettings["DHL_SiteID"];
            pwd.InnerText = ConfigurationManager.AppSettings["DHL_Password"];
            date.InnerText = DateTime.Now.ToString("yyyy-MM-dd");

            doc.Save(filePath);

        }

        public static ResponseQuote ParseXmlCapability(string respString)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(respString);

            ResponseQuote quote = new ResponseQuote();

            quote.BkgDetails = new BkgDetailsResp();
            quote.Srvs = new List<Service>();

            quote.BkgDetails.OriginServiceArea = new ServiceArea();

            XmlNode bkgDetails = doc.SelectSingleNode("//BkgDetails");
            XmlNode srvs = doc.SelectSingleNode("//Srvs");

            if (bkgDetails != null)
            {
                // <OriginServiceArea>
                XmlNode FacilityCode = bkgDetails.SelectSingleNode("OriginServiceArea//FacilityCode");
                if (FacilityCode != null)
                {
                    quote.BkgDetails.OriginServiceArea.FacilityCode = FacilityCode.InnerText;
                }

                XmlNode ServiceAreaCode = bkgDetails.SelectSingleNode("OriginServiceArea//ServiceAreaCode");
                if (ServiceAreaCode != null)
                {
                    quote.BkgDetails.OriginServiceArea.ServiceAreaCode = ServiceAreaCode.InnerText;
                }

                quote.BkgDetails.DestinationServiceArea = new ServiceArea();

                // <DestinationServiceArea>
                XmlNode DestFacilityCode = bkgDetails.SelectSingleNode("DestinationServiceArea//FacilityCode");
                if (DestFacilityCode != null)
                {
                    quote.BkgDetails.DestinationServiceArea.FacilityCode = DestFacilityCode.InnerText;
                }

                XmlNode DestServiceAreaCode = bkgDetails.SelectSingleNode("DestinationServiceArea//ServiceAreaCode");
                if (DestServiceAreaCode != null)
                {
                    quote.BkgDetails.DestinationServiceArea.ServiceAreaCode = DestServiceAreaCode.InnerText;
                }



                // <QtdShp>
                XmlNodeList qtdShpList = bkgDetails.SelectNodes("QtdShp");
                quote.BkgDetails.QtdShps = new List<QtdShpResp>();

                foreach (XmlNode qtdShp in qtdShpList)
                {
                    XmlNode GlobalProductCode = qtdShp.SelectSingleNode("GlobalProductCode");
                    XmlNode LocalProductCode = qtdShp.SelectSingleNode("LocalProductCode");
                    XmlNode ProductShortName = qtdShp.SelectSingleNode("ProductShortName");
                    XmlNode LocalProductName = qtdShp.SelectSingleNode("LocalProductName");
                    XmlNode NetworkTypeCode = qtdShp.SelectSingleNode("NetworkTypeCode");
                    XmlNode POfferedCustAgreement = qtdShp.SelectSingleNode("POfferedCustAgreement");
                    XmlNode TransInd = qtdShp.SelectSingleNode("TransInd");
                    XmlNode PickupDate = qtdShp.SelectSingleNode("PickupDate");
                    XmlNode PickupCutoffTime = qtdShp.SelectSingleNode("PickupCutoffTime");
                    XmlNode BookingTime = qtdShp.SelectSingleNode("BookingTime");
                    XmlNode CurrencyCode = qtdShp.SelectSingleNode("CurrencyCode");
                    XmlNode ExchangeRate = qtdShp.SelectSingleNode("ExchangeRate");
                    XmlNode WeightCharge = qtdShp.SelectSingleNode("WeightCharge");
                    XmlNode WeightChargeTax = qtdShp.SelectSingleNode("WeightChargeTax");
                    XmlNode TotalTransitDays = qtdShp.SelectSingleNode("TotalTransitDays");
                    XmlNode PickupPostalLocAddDays = qtdShp.SelectSingleNode("PickupPostalLocAddDays");
                    XmlNode DeliveryPostalLocAddDays = qtdShp.SelectSingleNode("DeliveryPostalLocAddDays");
                    XmlNode PickupNonDHLCourierCode = qtdShp.SelectSingleNode("PickupNonDHLCourierCode");
                    XmlNode DeliveryNonDHLCourierCode = qtdShp.SelectSingleNode("DeliveryNonDHLCourierCode");
                    XmlNode DeliveryDate = qtdShp.SelectSingleNode("DeliveryDate");
                    XmlNode DeliveryTime = qtdShp.SelectSingleNode("DeliveryTime");
                    XmlNode DimensionalWeight = qtdShp.SelectSingleNode("DimensionalWeight");
                    XmlNode WeightUnit = qtdShp.SelectSingleNode("WeightUnit");
                    XmlNode PickupDayOfWeekNum = qtdShp.SelectSingleNode("PickupDayOfWeekNum");
                    XmlNode DestinationDayOfWeekNum = qtdShp.SelectSingleNode("DestinationDayOfWeekNum");

                    // set to quote class
                    QtdShpResp qShp = new QtdShpResp();
                    qShp.GlobalProductCode = GlobalProductCode.InnerText;
                    qShp.LocalProductCode = LocalProductCode.InnerText;
                    qShp.ProductShortName = ProductShortName.InnerText;
                    qShp.LocalProductName = LocalProductName.InnerText;
                    qShp.NetworkTypeCode = NetworkTypeCode.InnerText;
                    qShp.POfferedCustAgreement = POfferedCustAgreement.InnerText;
                    qShp.TransInd = TransInd.InnerText;
                    qShp.PickupDate = PickupDate.InnerText;
                    qShp.PickupCutoffTime = PickupCutoffTime.InnerText;
                    qShp.BookingTime = BookingTime.InnerText;
                    qShp.CurrencyCode = CurrencyCode != null ? CurrencyCode.InnerText : null;

                    qShp.ExchangeRate = ExchangeRate != null ? Convert.ToDecimal(ExchangeRate.InnerText) : (Decimal?)null;
                    qShp.WeightCharge = WeightCharge != null ? Convert.ToDecimal(WeightCharge.InnerText) : (Decimal?)null;
                    qShp.WeightChargeTax = WeightChargeTax != null ? Convert.ToDecimal(WeightChargeTax.InnerText) : (Decimal?)null;
                    qShp.TotalTransitDays = TotalTransitDays.InnerText;
                    qShp.PickupPostalLocAddDays = PickupPostalLocAddDays.InnerText;
                    qShp.DeliveryPostalLocAddDays = DeliveryPostalLocAddDays.InnerText;
                    qShp.DeliveryDate = DeliveryDate.InnerText;
                    qShp.DeliveryTime = DeliveryTime.InnerText;
                    qShp.DimensionalWeight = DimensionalWeight.InnerText;
                    qShp.WeightUnit = WeightUnit.InnerText;
                    qShp.PickupDayOfWeekNum = PickupDayOfWeekNum.InnerText;
                    qShp.DestinationDayOfWeekNum = DestinationDayOfWeekNum.InnerText;

                    // <QtdShpExChrg>
                    qShp.QtdShpExChrgs = new List<QtdShpExChrg>();
                    XmlNodeList qtdShpChrgList = qtdShp.SelectNodes("QtdShpExChrg");
                    foreach (XmlNode qtdShpChrg in qtdShpChrgList)
                    {
                        QtdShpExChrg qtdShpExChrg = new QtdShpExChrg();

                        XmlNode SpecialServiceType = qtdShpChrg.SelectSingleNode("SpecialServiceType");
                        XmlNode LocalServiceType = qtdShpChrg.SelectSingleNode("LocalServiceType");
                        XmlNode GlobalServiceName = qtdShpChrg.SelectSingleNode("GlobalServiceName");
                        XmlNode LocalServiceTypeName = qtdShpChrg.SelectSingleNode("LocalServiceTypeName");
                        XmlNode ChargeCodeType = qtdShpChrg.SelectSingleNode("ChargeCodeType");
                        XmlNode CurrencyCode3 = qtdShpChrg.SelectSingleNode("CurrencyCode");
                        XmlNode ChargeValue = qtdShpChrg.SelectSingleNode("ChargeValue");
                        XmlNode ChargeTaxAmount = qtdShpChrg.SelectSingleNode("ChargeTaxAmount");

                        // set to quote class
                        qtdShpExChrg.SpecialServiceType = SpecialServiceType.InnerText;
                        qtdShpExChrg.LocalServiceType = LocalServiceType.InnerText;
                        qtdShpExChrg.GlobalServiceName = GlobalServiceName.InnerText;
                        qtdShpExChrg.LocalServiceTypeName = LocalServiceTypeName.InnerText;
                        qtdShpExChrg.ChargeCodeType = ChargeCodeType.InnerText;

                        qtdShpExChrg.CurrencyCode = CurrencyCode3 != null ? CurrencyCode3.InnerText : null;
                        qtdShpExChrg.ChargeValue = ChargeValue != null ? Convert.ToDecimal(ChargeValue.InnerText) : (decimal?)null;
                        qtdShpExChrg.ChargeTaxAmount = ChargeTaxAmount != null ? Convert.ToDecimal(ChargeTaxAmount.InnerText) : (decimal?)null;

                        // <QtdShpExChrg> <ChargeTaxAmountDet>

                        XmlNode chargeTaxAmountDet = qtdShpChrg.SelectSingleNode("ChargeTaxAmountDet");
                        if (chargeTaxAmountDet != null)
                        {
                            XmlNode TaxTypeRate3 = chargeTaxAmountDet.SelectSingleNode("TaxTypeRate");
                            XmlNode TaxTypeCode3 = chargeTaxAmountDet.SelectSingleNode("TaxTypeCode");
                            XmlNode TaxAmount = chargeTaxAmountDet.SelectSingleNode("TaxAmount");
                            XmlNode BaseAmount = chargeTaxAmountDet.SelectSingleNode("BaseAmount");

                            // set to quote class
                            qtdShpExChrg.ChargeTaxAmountDet = new ChargeTaxAmountDet();
                            qtdShpExChrg.ChargeTaxAmountDet.TaxTypeRate = Convert.ToDecimal(TaxTypeRate3.InnerText);
                            qtdShpExChrg.ChargeTaxAmountDet.TaxTypeCode = TaxTypeCode3.InnerText;
                            qtdShpExChrg.ChargeTaxAmountDet.TaxAmount = Convert.ToDecimal(TaxAmount.InnerText);
                            qtdShpExChrg.ChargeTaxAmountDet.BaseAmount = Convert.ToDecimal(BaseAmount.InnerText);
                        }



                        qtdShpExChrg.QtdSExtrChrgInAdCurList = new List<QtdSExtrChrgInAdCur>();

                        // <QtdShpExChrg> <QtdSExtrChrgInAdCur>
                        XmlNodeList qtdSExtrChrgInAdCurList = qtdShpChrg.SelectNodes("QtdSExtrChrgInAdCur");
                        foreach (XmlNode qtdSExtrChrgInAdCur in qtdSExtrChrgInAdCurList)
                        {
                            QtdSExtrChrgInAdCur adCur = new QtdSExtrChrgInAdCur();

                            XmlNode ChargeValue2 = qtdSExtrChrgInAdCur.SelectSingleNode("ChargeValue");
                            XmlNode ChargeTaxAmount2 = qtdSExtrChrgInAdCur.SelectSingleNode("ChargeTaxAmount");
                            XmlNode CurrencyCode4 = qtdSExtrChrgInAdCur.SelectSingleNode("CurrencyCode");
                            XmlNode CurrencyRoleTypeCode2 = qtdSExtrChrgInAdCur.SelectSingleNode("CurrencyRoleTypeCode");

                            adCur.ChargeValue = ChargeValue2 != null ? Convert.ToDecimal(ChargeValue2.InnerText) : (decimal?)null;
                            adCur.ChargeTaxAmount = ChargeTaxAmount2 != null ? Convert.ToDecimal(ChargeTaxAmount2.InnerText) : (decimal?)null;
                            adCur.CurrencyCode = CurrencyCode4 != null ? CurrencyCode4.InnerText : null;
                            adCur.CurrencyRoleTypeCode = CurrencyRoleTypeCode2 != null ? CurrencyRoleTypeCode2.InnerText : null;

                            XmlNode chargeTaxAmountDet2 = qtdSExtrChrgInAdCur.SelectSingleNode("ChargeTaxAmountDet");
                            if (chargeTaxAmountDet2 != null)
                            {
                                XmlNode TaxTypeRate4 = chargeTaxAmountDet2.SelectSingleNode("TaxTypeRate");
                                XmlNode TaxTypeCode4 = chargeTaxAmountDet2.SelectSingleNode("TaxTypeCode");
                                XmlNode TaxAmount2 = chargeTaxAmountDet2.SelectSingleNode("TaxAmount");
                                XmlNode BaseAmount2 = chargeTaxAmountDet2.SelectSingleNode("BaseAmount");

                                adCur.ChargeTaxAmountDet = new ChargeTaxAmountDet();
                                adCur.ChargeTaxAmountDet.TaxTypeRate = Convert.ToDecimal(TaxTypeRate4.InnerText);
                                adCur.ChargeTaxAmountDet.TaxTypeCode = TaxTypeCode4.InnerText;
                                adCur.ChargeTaxAmountDet.TaxAmount = Convert.ToDecimal(TaxAmount2.InnerText);
                                adCur.ChargeTaxAmountDet.BaseAmount = Convert.ToDecimal(BaseAmount2.InnerText);
                            }

                            qtdShpExChrg.QtdSExtrChrgInAdCurList.Add(adCur);
                        }
                        qShp.QtdShpExChrgs.Add(qtdShpExChrg);
                    }

                    XmlNode PricingDate = qtdShp.SelectSingleNode("PricingDate");
                    XmlNode ShippingCharge = qtdShp.SelectSingleNode("ShippingCharge");
                    XmlNode TotalTaxAmount = qtdShp.SelectSingleNode("TotalTaxAmount");

                    qShp.PricingDate = PricingDate != null ? PricingDate.InnerText : null;
                    qShp.ShippingCharge = ShippingCharge != null ? Convert.ToDecimal(ShippingCharge.InnerText) : (Decimal?)null;
                    qShp.TotalTaxAmount = TotalTaxAmount != null ? Convert.ToDecimal(TotalTaxAmount.InnerText) : (Decimal?)null;

                    // <QtdSInAdCur>
                    XmlNodeList qtdSInAdCurList = qtdShp.SelectNodes("QtdSInAdCur");
                    qShp.QtdSInAdCurList = new List<QtdSInAdCur>();
                    foreach (XmlNode qtdSInAdCur in qtdSInAdCurList)
                    {
                        QtdSInAdCur AdCur = new QtdSInAdCur();

                        XmlNode CurrencyCode2 = qtdSInAdCur.SelectSingleNode("CurrencyCode");
                        XmlNode CurrencyRoleTypeCode = qtdSInAdCur.SelectSingleNode("CurrencyRoleTypeCode");
                        XmlNode WeightCharge2 = qtdSInAdCur.SelectSingleNode("WeightCharge");
                        XmlNode TotalAmount = qtdSInAdCur.SelectSingleNode("TotalAmount");
                        XmlNode TotalTaxAmount2 = qtdSInAdCur.SelectSingleNode("TotalTaxAmount");
                        XmlNode WeightChargeTax2 = qtdSInAdCur.SelectSingleNode("WeightChargeTax");
                        if (CurrencyCode2 != null)
                        {
                            AdCur.CurrencyCode = CurrencyCode2.InnerText;
                            AdCur.CurrencyRoleTypeCode = CurrencyRoleTypeCode.InnerText;
                            AdCur.WeightCharge = Convert.ToDecimal(WeightCharge2.InnerText);
                            AdCur.TotalAmount = Convert.ToDecimal(TotalAmount.InnerText);
                            AdCur.TotalTaxAmount = Convert.ToDecimal(TotalTaxAmount2.InnerText);
                            AdCur.WeightChargeTax = Convert.ToDecimal(WeightChargeTax2.InnerText);
                        }

                        // <QtdSInAdCur> <WeightChargeTaxDet>
                        XmlNode weightChargeTaxDet = qtdSInAdCur.SelectSingleNode("WeightChargeTaxDet");
                        if (weightChargeTaxDet != null)
                        {
                            XmlNode TaxTypeRate = weightChargeTaxDet.SelectSingleNode("TaxTypeRate");
                            XmlNode TaxTypeCode = weightChargeTaxDet.SelectSingleNode("TaxTypeCode");
                            XmlNode WeightChargeTax3 = weightChargeTaxDet.SelectSingleNode("WeightChargeTax");
                            XmlNode BaseAmt = weightChargeTaxDet.SelectSingleNode("BaseAmt");

                            AdCur.WeightChargeTaxDet = new WeightChargeTaxDet();
                            AdCur.WeightChargeTaxDet.TaxTypeRate = Convert.ToDecimal(TaxTypeRate.InnerText);
                            AdCur.WeightChargeTaxDet.TaxTypeCode = TaxTypeCode.InnerText;
                            AdCur.WeightChargeTaxDet.WeightChargeTax = Convert.ToDecimal(WeightChargeTax3.InnerText);
                            AdCur.WeightChargeTaxDet.BaseAmt = Convert.ToDecimal(BaseAmt.InnerText);
                        }

                        qShp.QtdSInAdCurList.Add(AdCur);
                    }

                    XmlNode weightChargeTaxDet2 = qtdShp.SelectSingleNode("WeightChargeTaxDet");
                    if (weightChargeTaxDet2 != null)
                    {
                        XmlNode TaxTypeRate2 = weightChargeTaxDet2.SelectSingleNode("TaxTypeRate");
                        XmlNode TaxTypeCode2 = weightChargeTaxDet2.SelectSingleNode("TaxTypeCode");
                        XmlNode WeightChargeTax4 = weightChargeTaxDet2.SelectSingleNode("WeightChargeTax");
                        XmlNode BaseAmt2 = weightChargeTaxDet2.SelectSingleNode("BaseAmt");

                        qShp.WeightChargeTaxDet = new WeightChargeTaxDet();
                        qShp.WeightChargeTaxDet.TaxTypeRate = Convert.ToDecimal(TaxTypeRate2.InnerText);
                        qShp.WeightChargeTaxDet.TaxTypeCode = TaxTypeCode2.InnerText;
                        qShp.WeightChargeTaxDet.WeightChargeTax = Convert.ToDecimal(WeightChargeTax4.InnerText);
                        qShp.WeightChargeTaxDet.BaseAmt = Convert.ToDecimal(BaseAmt2.InnerText);
                    }

                    quote.BkgDetails.QtdShps.Add(qShp);
                }
            }

            //  <Srvs>


            if (srvs != null)
            {
                XmlNodeList srvList = srvs.SelectNodes("Srv");
                foreach (XmlNode srv in srvList)
                {
                    Service service = new Service();

                    XmlNode GlobalProductCode = srv.SelectSingleNode("GlobalProductCode");
                    XmlNodeList mrkSrvList = srv.SelectNodes("MrkSrv");

                    service.GlobalProductCode = GlobalProductCode.InnerText;
                    service.MrkSrvs = new List<MrkSrv>();

                    foreach (XmlNode mrkSrv in mrkSrvList)
                    {
                        MrkSrv mrkService = new MrkSrv();

                        XmlNode LocalProductCode = mrkSrv.SelectSingleNode("LocalProductCode");
                        XmlNode ProductShortName = mrkSrv.SelectSingleNode("ProductShortName");
                        XmlNode LocalProductName = mrkSrv.SelectSingleNode("LocalProductName");
                        XmlNode NetworkTypeCode = mrkSrv.SelectSingleNode("NetworkTypeCode");
                        XmlNode POfferedCustAgreement = mrkSrv.SelectSingleNode("POfferedCustAgreement");
                        XmlNode TransInd = mrkSrv.SelectSingleNode("TransInd");

                        XmlNode LocalServiceType = mrkSrv.SelectSingleNode("LocalServiceType");
                        XmlNode GlobalServiceName = mrkSrv.SelectSingleNode("GlobalServiceName");
                        XmlNode LocalServiceTypeName = mrkSrv.SelectSingleNode("LocalServiceTypeName");
                        XmlNode ChargeCodeType = mrkSrv.SelectSingleNode("ChargeCodeType");
                        XmlNode MrkSrvInd = mrkSrv.SelectSingleNode("MrkSrvInd");



                        mrkService.LocalProductCode = LocalProductCode != null ? LocalProductCode.InnerText : null;
                        mrkService.ProductShortName = ProductShortName != null ? ProductShortName.InnerText : null;
                        mrkService.LocalProductName = LocalProductName != null ? LocalProductName.InnerText : null;
                        mrkService.NetworkTypeCode = NetworkTypeCode != null ? NetworkTypeCode.InnerText : null;
                        mrkService.POfferedCustAgreement = POfferedCustAgreement != null ? POfferedCustAgreement.InnerText : null;
                        mrkService.TransInd = TransInd != null ? TransInd.InnerText : null;

                        mrkService.LocalServiceType = LocalServiceType != null ? LocalServiceType.InnerText : null;
                        mrkService.GlobalServiceName = GlobalServiceName != null ? GlobalServiceName.InnerText : null;
                        mrkService.LocalServiceTypeName = LocalServiceTypeName != null ? LocalServiceTypeName.InnerText : null;
                        mrkService.ChargeCodeType = ChargeCodeType != null ? ChargeCodeType.InnerText : null;
                        mrkService.MrkSrvInd = MrkSrvInd != null ? MrkSrvInd.InnerText : null;


                        service.MrkSrvs.Add(mrkService);
                    }
                    quote.Srvs.Add(service);
                }
            }

            return quote;
        }


        public void ParseXmlBookPickup(WebResponse response)
        {
            XmlDocument doc = new XmlDocument();
            var respStream = response.GetResponseStream();
            doc.Load(respStream);

            //string xmlText = File.ReadAllText(filePath);

            XmlNode ConfirmationNumber = doc.SelectSingleNode("//ConfirmationNumber");
            Console.WriteLine("ConfirmationNumber = {0}", ConfirmationNumber.InnerText);

            XmlNode NextPickupDate = doc.SelectSingleNode("//NextPickupDate");
            Console.WriteLine("NextPickupDate = {0}", NextPickupDate.InnerText);
        }

        public void ParseXmlShipment(WebResponse response)
        {
            XmlDocument doc = new XmlDocument();
            var respStream = response.GetResponseStream();
            doc.Load(respStream);

            //string xmlText = File.ReadAllText(filePath);

            XmlNode ShipperID = doc.SelectSingleNode("//ShipperID");
            Console.WriteLine("ShipperID = {0}", ShipperID.InnerText);

            XmlNode CompanyName = doc.SelectSingleNode("//CompanyName");
            Console.WriteLine("CompanyName = {0}", CompanyName.InnerText);

            XmlNodeList AddressLines = doc.SelectNodes("//Shipper//AddressLine");
            foreach (XmlNode address in AddressLines)
            {
                Console.WriteLine("AddressLines = {0}", address.InnerText);
            }

            XmlNode City = doc.SelectSingleNode("//Shipper//City");
            Console.WriteLine("City = {0}", City.InnerText);
            XmlNode DivisionCode = doc.SelectSingleNode("//Shipper//DivisionCode");
            Console.WriteLine("DivisionCode = {0}", DivisionCode.InnerText);
            XmlNode CountryCode = doc.SelectSingleNode("//Shipper//CountryCode");
            Console.WriteLine("CountryCode = {0}", CountryCode.InnerText);
            XmlNode CountryName = doc.SelectSingleNode("//Shipper//CountryName");
            Console.WriteLine("CountryName = {0}", CountryName.InnerText);
        }

        public static List<ResponseAWBInfo> ParseXmlTracking(string respString)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(respString);

            List<ResponseAWBInfo> AWBInfoList = new List<ResponseAWBInfo>();
            XmlNodeList AWBInfos = doc.SelectNodes("//AWBInfo");

            foreach (XmlNode awbInfo in AWBInfos)
            {
                ResponseAWBInfo AWBInfo = new ResponseAWBInfo();
                AWBInfo.ShipmentInfo = new ResponseShipmentInfo();

                XmlNode AWBNumber = awbInfo.SelectSingleNode("AWBNumber");
                AWBInfo.AWBNumber = AWBNumber.InnerText;

                XmlNode TrackedBy = awbInfo.SelectSingleNode("TrackedBy");
                if (TrackedBy != null)
                {
                    AWBInfo.TrackedBy_LPNumber = TrackedBy.InnerText;
                }

                XmlNode ActionStatus = awbInfo.SelectSingleNode("Status//ActionStatus");
                AWBInfo.Status_ActionStatus = ActionStatus.InnerText;

                XmlNodeList ShipmentInfos = awbInfo.SelectNodes("ShipmentInfo");
                //AWBInfo.ShipmentInfo.ShipmentEvents = new List<ResponseShipmentEvent>();
                foreach (XmlNode info in ShipmentInfos)
                {

                    XmlNode OriginServiceAreaCode = info.SelectSingleNode("OriginServiceArea//ServiceAreaCode");
                    XmlNode OriginDescription = info.SelectSingleNode("OriginServiceArea//Description");

                    XmlNode DestServiceAreaCode = info.SelectSingleNode("DestinationServiceArea//ServiceAreaCode");
                    XmlNode DestDescription = info.SelectSingleNode("DestinationServiceArea//Description");
                    if (OriginServiceAreaCode == null || DestServiceAreaCode == null)
                    {
                        return AWBInfoList;
                    }
                    XmlNode shipperName = info.SelectSingleNode("ShipperName");
                    XmlNode shipperAccountNumber = info.SelectSingleNode("ShipperAccountNumber");
                    XmlNode consigneeName = info.SelectSingleNode("ConsigneeName");
                    XmlNode shipmentDate = info.SelectSingleNode("ShipmentDate");

                    XmlNode pieces = info.SelectSingleNode("Pieces");
                    XmlNode weight = info.SelectSingleNode("Weight");
                    XmlNode weightUnit = info.SelectSingleNode("WeightUnit");
                    XmlNode globalProductCode = info.SelectSingleNode("GlobalProductCode");
                    XmlNode shipmentDesc = info.SelectSingleNode("ShipmentDesc");
                    XmlNode dlvyNotificationFlag = info.SelectSingleNode("DlvyNotificationFlag");

                    XmlNode city = info.SelectSingleNode("Shipper//City");
                    XmlNode postalCode = info.SelectSingleNode("Shipper//PostalCode");
                    XmlNode countryCode = info.SelectSingleNode("Shipper//CountryCode");
                    ResponseShipper shipper = new ResponseShipper
                    {
                        City = city.InnerText,
                        PostalCode = postalCode != null ? postalCode.InnerText : null,
                        CountryCode = countryCode != null ? countryCode.InnerText : null
                    };

                    city = info.SelectSingleNode("Consignee//City");
                    XmlNode divisionCode = info.SelectSingleNode("Consignee//DivisionCode");
                    postalCode = info.SelectSingleNode("Consignee//PostalCode");
                    countryCode = info.SelectSingleNode("Consignee//CountryCode");
                    ResponseConsignee consignee = new ResponseConsignee
                    {
                        City = city.InnerText,
                        DivisionCode = divisionCode != null ? divisionCode.InnerText : null,
                        PostalCode = postalCode != null ? postalCode.InnerText : null,
                        CountryCode = countryCode != null ? countryCode.InnerText : null
                    };

                    XmlNodeList ShipmentEvents = info.SelectNodes("ShipmentEvent");
                    List<ResponseShipmentEvent> events = new List<ResponseShipmentEvent>();
                    foreach (XmlNode evt in ShipmentEvents)
                    {
                        XmlNode date = evt.SelectSingleNode("Date");
                        XmlNode time = evt.SelectSingleNode("Time");
                        XmlNode eventCode = evt.SelectSingleNode("ServiceEvent//EventCode");
                        XmlNode description = evt.SelectSingleNode("ServiceEvent//Description");
                        XmlNode signatory = evt.SelectSingleNode("Signatory");
                        XmlNode serviceAreaCode = evt.SelectSingleNode("ServiceArea//ServiceAreaCode");
                        XmlNode description2 = evt.SelectSingleNode("ServiceArea//Description");

                        events.Add(new ResponseShipmentEvent
                        {
                            Date = date.InnerText,
                            Time = time.InnerText,
                            ServiceEvent_EventCode = eventCode.InnerText,
                            ServiceEvent_Description = description.InnerText,
                            Signatory = signatory.InnerText,
                            ServiceArea_ServiceAreaCode = serviceAreaCode.InnerText,
                            ServiceArea_Description = description2.InnerText
                        });

                    }
                    AWBInfo.ShipmentInfo = new ResponseShipmentInfo
                    {
                        OriginServiceArea_ServiceAreaCode = OriginServiceAreaCode != null ? OriginServiceAreaCode.InnerText : null,
                        OriginServiceArea_Description = OriginDescription != null ? OriginDescription.InnerText : null,
                        DestinationServiceArea_ServiceAreaCode = DestServiceAreaCode != null ? DestServiceAreaCode.InnerText : null,
                        DestinationServiceArea_Description = DestDescription != null ? DestDescription.InnerText : null,
                        ShipperName = shipperName.InnerText,
                        ShipperAccountNumber = shipperAccountNumber.InnerText,
                        ConsigneeName = consigneeName != null ? consigneeName.InnerText : null,
                        ShipmentDate = shipmentDate.InnerText != null ? DateTime.Parse(shipmentDate.InnerText) : DateTime.MinValue,
                        Pieces = pieces != null ? int.Parse(pieces.InnerText) : 0,
                        WeightUnit = weightUnit.InnerText,
                        GlobalProductCode = globalProductCode.InnerText,
                        ShipmentDesc = shipmentDesc.InnerText,
                        DlvyNotificationFlag = dlvyNotificationFlag.InnerText,

                        Shipper = shipper,
                        Consignee = consignee,
                        ShipmentEvents = events
                    };

                }
                XmlNode pieceDetails = awbInfo.SelectSingleNode("Pieces//PieceInfo//PieceDetails");
                if (pieceDetails != null)
                {
                    AWBInfo.PieceDetails = new ResponsePieceDetails
                    {
                        AWBNumber = pieceDetails.SelectSingleNode("AWBNumber").InnerText,
                        LicensePlate = pieceDetails.SelectSingleNode("LicensePlate").InnerText,
                        PieceNumber = int.Parse(pieceDetails.SelectSingleNode("PieceNumber").InnerText),
                        ActualDepth = float.Parse(pieceDetails.SelectSingleNode("ActualDepth").InnerText),
                        ActualWidth = pieceDetails.SelectSingleNode("ActualWidth").InnerText != null ? float.Parse(pieceDetails.SelectSingleNode("ActualWidth").InnerText) : 0f,
                        ActualHeight = pieceDetails.SelectSingleNode("ActualHeight").InnerText != null ? float.Parse(pieceDetails.SelectSingleNode("ActualHeight").InnerText) : 0f,
                        ActualWeight = pieceDetails.SelectSingleNode("ActualWeight").InnerText != null ? float.Parse(pieceDetails.SelectSingleNode("ActualWeight").InnerText) : 0f,
                        Depth = pieceDetails.SelectSingleNode("Depth").InnerText != null ? float.Parse(pieceDetails.SelectSingleNode("Depth").InnerText) : 0f,
                        Height = pieceDetails.SelectSingleNode("Height").InnerText != null ? float.Parse(pieceDetails.SelectSingleNode("Height").InnerText) : 0f,
                        Weight = pieceDetails.SelectSingleNode("Weight").InnerText != null ? float.Parse(pieceDetails.SelectSingleNode("Weight").InnerText) : 0f,
                        PackageType = pieceDetails.SelectSingleNode("PackageType") != null ? pieceDetails.SelectSingleNode("PackageType").InnerText : null,
                        DimWeight = pieceDetails.SelectSingleNode("DimWeight").InnerText != null ? float.Parse(pieceDetails.SelectSingleNode("DimWeight").InnerText) : 0f,
                        WeightUnit = pieceDetails.SelectSingleNode("WeightUnit").InnerText,
                    };
                }
                AWBInfoList.Add(AWBInfo);
            }
            return AWBInfoList;
        }



    }
}
