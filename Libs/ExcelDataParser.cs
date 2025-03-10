using ExcelDataReader;
using Omnae.Libs.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Data.Linq;
using System.Text.RegularExpressions;

namespace Omnae.Libs
{
    public class ExcelDataParser
    {
        //Old UI OnBoarding
        public static List<FileDataViewModel> GetFileStream(string filePath)
        {
            using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                IExcelDataReader excelReader = ExcelReaderFactory.CreateReader(stream);

                DataSet result = excelReader.AsDataSet();
                excelReader.Close();
                DataTable dt = result.Tables["Data"];

                List<FileDataViewModel> fdList = new List<FileDataViewModel>();
                foreach (DataRow row in dt.AsEnumerable().Skip(3))
                {
                    if (string.IsNullOrWhiteSpace(row[0].ToString()) ||
                        string.IsNullOrWhiteSpace(row[1].ToString()) ||
                        string.IsNullOrWhiteSpace(row[2].ToString()) ||
                        string.IsNullOrWhiteSpace(row[3].ToString()) ||
                        string.IsNullOrWhiteSpace(row[4].ToString()) ||
                        string.IsNullOrWhiteSpace(row[5].ToString()) ||
                        string.IsNullOrWhiteSpace(row[6].ToString()) ||
                        string.IsNullOrWhiteSpace(row[8].ToString()) ||
                        string.IsNullOrWhiteSpace(row[11].ToString()) ||
                        string.IsNullOrWhiteSpace(row[12].ToString()) ||
                        string.IsNullOrWhiteSpace(row[25].ToString()) ||
                        string.IsNullOrWhiteSpace(row[26].ToString()) ||
                        string.IsNullOrWhiteSpace(row[41].ToString()) ||
                        string.IsNullOrWhiteSpace(row[42].ToString()) ||
                        string.IsNullOrWhiteSpace(row[43].ToString()) ||
                        string.IsNullOrWhiteSpace(row[57].ToString()))
                    {
                        throw new Exception("Required column(s) is missing.");
                    }

                    var entity = new FileDataViewModel();
                    entity.Name = row[0].ToString();
                    entity.Description = row[1].ToString();
                    entity.PartNumber = row[2].ToString();
                    entity.PartNumberRevision = row[3].ToString();
                    entity.StateId = Convert.ToInt32(row[4]);
                    entity.AvatarFilePath = row[5].ToString();
                    entity.Doc2D = row[6].ToString();
                    entity.Doc3D = row[7] != DBNull.Value ? row[7].ToString() : null;
                    entity.DocProof = row[8].ToString();
                    entity.AdminId = ConfigurationManager.AppSettings["AdminId"];
                    entity.VendorId = row[9] != DBNull.Value ? Convert.ToInt32(row[9]) : (int?)null;
                    entity.PartVolume = row[10] != DBNull.Value ? Convert.ToInt32(row[10]) : (int?)null;
                    entity.PriceBreak_Amount1 = Convert.ToInt32(row[11]);
                    entity.PriceBreak_UnitPrice1 = Convert.ToDecimal(row[12]);
                    entity.PriceBreak_Amount2 = row[13] != DBNull.Value ? Convert.ToInt32(row[13]) : (int?)null;
                    entity.PriceBreak_UnitPrice2 = row[14] != DBNull.Value ? (string.IsNullOrEmpty(row[14].ToString()) ? (Decimal?)null : Convert.ToDecimal(row[14])) : (Decimal?)null;
                    entity.PriceBreak_Amount3 = row[15] != DBNull.Value ? Convert.ToInt32(row[15]) : (int?)null;
                    entity.PriceBreak_UnitPrice3 = row[16] != DBNull.Value ? (string.IsNullOrEmpty(row[16].ToString()) ? (Decimal?)null : Convert.ToDecimal(row[16])) : (Decimal?)null;
                    entity.PriceBreak_Amount4 = row[17] != DBNull.Value ? Convert.ToInt32(row[17]) : (int?)null;
                    entity.PriceBreak_UnitPrice4 = row[18] != DBNull.Value ? (string.IsNullOrEmpty(row[18].ToString()) ? (Decimal?)null : Convert.ToDecimal(row[18])) : (Decimal?)null;
                    entity.PriceBreak_Amount5 = row[19] != DBNull.Value ? Convert.ToInt32(row[19]) : (int?)null;
                    entity.PriceBreak_UnitPrice5 = row[20] != DBNull.Value ? (string.IsNullOrEmpty(row[20].ToString()) ? (Decimal?)null : Convert.ToDecimal(row[20])) : (Decimal?)null;
                    entity.PriceBreak_Amount6 = row[21] != DBNull.Value ? Convert.ToInt32(row[21]) : (int?)null;
                    entity.PriceBreak_UnitPrice6 = row[22] != DBNull.Value ? (string.IsNullOrEmpty(row[22].ToString()) ? (Decimal?)null : Convert.ToDecimal(row[22])) : (Decimal?)null;
                    entity.PriceBreak_Amount7 = row[23] != DBNull.Value ? Convert.ToInt32(row[23]) : (int?)null;
                    entity.PriceBreak_UnitPrice7 = row[24] != DBNull.Value ? (string.IsNullOrEmpty(row[24].ToString()) ? (Decimal?)null : Convert.ToDecimal(row[24])) : (Decimal?)null;
                    entity.BuildType = Convert.ToInt32(row[25]);
                    entity.Material = Convert.ToInt32(row[26]);
                    entity.PrecisionMetal = row[27] != DBNull.Value ? Convert.ToInt32(row[27]) : (int?)null;
                    entity.MetalsProcesses = row[28] != DBNull.Value ? Convert.ToInt32(row[28]) : (int?)null;
                    entity.MetalType = row[29] != DBNull.Value ? Convert.ToInt32(row[29]) : (int?)null;
                    entity.MetalsSurfaceFinish = row[30] != DBNull.Value ? Convert.ToInt32(row[30]) : (int?)null;
                    entity.PrecisionPlastics = row[31] != DBNull.Value ? Convert.ToInt32(row[31]) : (int?)null;
                    entity.PlasticsProcesses = row[32] != DBNull.Value ? Convert.ToInt32(row[32]) : (int?)null;
                    entity.MembraneSwitches = row[33] != DBNull.Value ? Convert.ToInt32(row[33]) : (int?)null;
                    entity.Elastomers = row[34] != DBNull.Value ? row[34].ToString() : null;
                    entity.Labels = row[35] != DBNull.Value ? row[35].ToString() : null;
                    entity.Others = row[36] != DBNull.Value ? row[36].ToString() : null;
                    entity.MetalType_FreeText = row[37] != DBNull.Value ? row[37].ToString() : null;
                    entity.SurfaceFinish_FreeText = row[38] != DBNull.Value ? row[38].ToString() : null;
                    entity.PlasticType_FreeText = row[39] != DBNull.Value ? row[39].ToString() : null;
                    entity.ToolingLeadTime = row[40] != DBNull.Value ? Convert.ToInt32(row[40]) : (int?)null;
                    entity.SampleLeadTime = Convert.ToInt32(row[41]);
                    entity.ProductionLeadTime = Convert.ToInt32(row[42]);
                    entity.ToolingSetupCharges = Convert.ToDecimal(row[43]);
                    entity.HarmonizedCode = row[44] != DBNull.Value ? row[44].ToString() : null;
                    entity.MembraneSwitchesAttributesWaterproof = row[45] != DBNull.Value ? Convert.ToInt32(row[45]) : (int?)null;
                    entity.MembraneSwitchesAttributesEmbossing = row[46] != DBNull.Value ? Convert.ToInt32(row[46]) : (int?)null;
                    entity.MembraneSwitchesAttributesLEDLighting = row[47] != DBNull.Value ? Convert.ToInt32(row[47]) : (int?)null;
                    entity.MembraneSwitchesAttributesLED_EL_Backlighting = row[48] != DBNull.Value ? Convert.ToInt32(row[48]) : (int?)null;
                    entity.GraphicOverlaysAttributesEmbossing = row[49] != DBNull.Value ? Convert.ToInt32(row[49]) : (int?)null;
                    entity.GraphicOverlaysAttributesSelectiveTexture = row[50] != DBNull.Value ? Convert.ToInt32(row[50]) : (int?)null;
                    entity.MembraneSwitchesAttributes = row[51] != DBNull.Value ? Convert.ToInt32(row[51]) : (int?)null;
                    entity.GraphicOverlaysAttributes = row[52] != DBNull.Value ? Convert.ToInt32(row[52]) : (int?)null;
                    entity.MilledStone = row[53] != DBNull.Value ? row[53].ToString() : null;
                    entity.MilledWood = row[54] != DBNull.Value ? row[54].ToString() : null;
                    entity.FlexCircuits = row[55] != DBNull.Value ? row[55].ToString() : null;
                    entity.CableAssemblies = row[56] != DBNull.Value ? row[56].ToString() : null;
                    entity.Vendor_PriceBreak_UnitPrice1 = Convert.ToDecimal(row[57]);
                    entity.Vendor_PriceBreak_UnitPrice2 = row[58] != DBNull.Value ? (string.IsNullOrEmpty(row[58].ToString()) ? (Decimal?)null : Convert.ToDecimal(row[58])) : (Decimal?)null;
                    entity.Vendor_PriceBreak_UnitPrice3 = row[59] != DBNull.Value ? (string.IsNullOrEmpty(row[59].ToString()) ? (Decimal?)null : Convert.ToDecimal(row[59])) : (Decimal?)null;
                    entity.Vendor_PriceBreak_UnitPrice4 = row[60] != DBNull.Value ? (string.IsNullOrEmpty(row[60].ToString()) ? (Decimal?)null : Convert.ToDecimal(row[60])) : (Decimal?)null;
                    entity.Vendor_PriceBreak_UnitPrice5 = row[61] != DBNull.Value ? (string.IsNullOrEmpty(row[61].ToString()) ? (Decimal?)null : Convert.ToDecimal(row[61])) : (Decimal?)null;
                    entity.Vendor_PriceBreak_UnitPrice6 = row[62] != DBNull.Value ? (string.IsNullOrEmpty(row[62].ToString()) ? (Decimal?)null : Convert.ToDecimal(row[62])) : (Decimal?)null;
                    entity.Vendor_PriceBreak_UnitPrice7 = row[63] != DBNull.Value ? (string.IsNullOrEmpty(row[63].ToString()) ? (Decimal?)null : Convert.ToDecimal(row[63])) : (Decimal?)null;

                    fdList.Add(entity);
                }
                return fdList;
            }
        }

        //New UI Company OnBoarding
        public static List<VendorDataViewModel> GetCompanyDataStream(string filePath)
        {
            DataSet result;

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var excelReader = ExcelReaderFactory.CreateReader(stream))
            {
                result = excelReader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });

                excelReader.Close();
                stream.Close();
            }

            if (result.Tables.Count == 0)
                throw new ValidationException("Invalid Import File.");

            DataTable dt = result.Tables.Contains("Active") ? result.Tables["Active"] :
                           result.Tables.Contains("Vendors") ? result.Tables["Vendors"] :
                           result.Tables.Contains("Partner") ? result.Tables["Partner"] :
                           result.Tables[0];

            var da = dt.AsEnumerable().FirstOrDefault(r => r[0].ToString().ToLowerInvariant().Contains("VendorName".ToLowerInvariant()) || r[0].ToString().ToLowerInvariant().Contains("Vendor Name".ToLowerInvariant())
                                                || r[0].ToString().ToLowerInvariant().Contains("PartnerCompanyName".ToLowerInvariant()) || r[0].ToString().ToLowerInvariant().Contains("Partner Company Name".ToLowerInvariant()) );

            string[] columnNames = da?.ItemArray.Select(x => x.ToString()).ToArray() 
                                   ?? dt.Columns.Cast<DataColumn>().Where(c => !string.IsNullOrEmpty(c.ColumnName)).Select(x => x.ColumnName).ToArray();

            List<VendorDataViewModel> fdList = new List<VendorDataViewModel>();
            foreach (DataRow row in ((da == null) ? dt.AsEnumerable() : dt.AsEnumerable().SkipWhile(r => r != da)))
            {
                if (row == null || row == da) //Skip the Header row
                    continue;

                if (string.IsNullOrWhiteSpace(row[0].ToString()) && string.IsNullOrWhiteSpace(row[1].ToString())
                                                                 && string.IsNullOrWhiteSpace(row[3].ToString())
                                                                 && string.IsNullOrWhiteSpace(row[4].ToString())) //Check many cool that this line have some data entry. If I don't check all lines will be returned and invalid.
                    continue;

                var entity = new VendorDataViewModel();                  
                for (var i = 0; i < columnNames.Length; i++)
                {
                    var colName = columnNames[i].ToUpperInvariant();


                    if (colName.Contains("VendorName".ToUpperInvariant()) || colName.Contains("Vendor Name".ToUpperInvariant()) || colName.Contains("Partner Company Name".ToUpperInvariant()))
                    {
                        entity.VendorName = row[i].ToString()?.Trim();
                    }
                    else if (colName.Contains("FirstName".ToUpperInvariant()) || colName.Contains("Contact First Name".ToUpperInvariant()))
                    {
                        entity.ContactFirstName = row[i]?.ToString()?.Trim();
                    }
                    else if (colName.Contains("LastName".ToUpperInvariant()) || colName.Contains("Contact Last Name".ToUpperInvariant()))
                    {
                        entity.ContactLastName = row[i]?.ToString()?.Trim();
                    }
                    else if (colName.Contains("VendorAddressLine1".ToUpperInvariant()) || colName.Contains("Vendor AddressLine1".ToUpperInvariant())
                                                                                       || colName.Contains("Partner AddressLine1".ToUpperInvariant()))
                    {
                        entity.AddressLine1 = row[i].ToString()?.Trim();
                    }
                    else if (colName.Contains("VendorAddressLine2".ToUpperInvariant()) || colName.Contains("Vendor AddressLine2".ToUpperInvariant())
                                                                                       || colName.Contains("Partner AddressLine2".ToUpperInvariant()))
                    {
                        entity.AddressLine2 = row[i]?.ToString()?.Trim();
                    }
                    else if (colName.Contains("VendorCity".ToUpperInvariant()) || colName.Contains("Vendor City".ToUpperInvariant())
                                                                               || colName.Contains("Partner City".ToUpperInvariant()))
                    {
                        entity.City = row[i].ToString()?.Trim()?.Trim();
                    }
                    else if (colName.Contains("VendorCountry".ToUpperInvariant()) || colName.Contains("Vendor Country".ToUpperInvariant())
                                                                                  || colName.Contains("Partner Country".ToUpperInvariant()))
                    {
                        entity.Country = row[i].ToString()?.Trim();
                    }
                    else if (colName.Contains("StateOrProvince".ToUpperInvariant()) || colName.Contains("State or Province".ToUpperInvariant()))
                    {
                        entity.StateOrProvince = row[i]?.ToString()?.Trim();
                    }
                    else if (colName.Contains("VendorZipCode".ToUpperInvariant()) || colName.Contains("Vendor Zip Code".ToUpperInvariant())
                                                                                  || colName.Contains("Partner Zip Code".ToUpperInvariant()))
                    {
                        entity.ZipCode = row[i]?.ToString()?.Trim();
                    }
                    else if (colName.Contains("VendorPostalCode".ToUpperInvariant()) || colName.Contains("Vendor Postal Code".ToUpperInvariant())
                                                                                     || colName.Contains("Partner Postal Code".ToUpperInvariant()))
                    {
                        entity.PostalCode = row[i]?.ToString()?.Trim();
                    }
                    else if (colName.Contains("VendorContactEmail".ToUpperInvariant()) || colName.Contains("Vendor Contact Email".ToUpperInvariant())
                                                                                       || colName.Contains("Partner Contact Email".ToUpperInvariant()))
                    {
                        entity.Email = row[i].ToString()?.Trim()?.ToLowerInvariant();
                    }
                    else if (colName.Contains("Phone Country Code".ToUpperInvariant()))
                    {
                        entity.PhoneCountryCode = row[i].ToString().Trim();
                    }
                    else if (colName.Contains("VendorPhoneNumber".ToUpperInvariant()) || colName.Contains("Vendor Phone Number".ToUpperInvariant())
                                                                                      || colName.Contains("Vendor Local Phone Number".ToUpperInvariant())
                                                                                      || colName.Contains("Partner Phone Number".ToUpperInvariant())
                                                                                      || colName.Contains("Partner Local Phone Number".ToUpperInvariant())
                                                                                      || colName.Contains("Phone Number".ToUpperInvariant()))
                    {
                        entity.Phone = row[i].ToString();
                        entity.OriginalPhoneEntry = row[i].ToString();
                    }

                    else if (colName.Contains("Currency ISO Code".ToUpperInvariant()))
                    {
                        entity.Currency = row[i].ToString().Trim().ToUpperInvariant();
                    }

                    else if (colName.Contains("IsTerm".ToUpperInvariant()) || colName.Contains("Do you have credit terms with this vendor?".ToUpperInvariant())
                                                                           || colName.Contains("Do you have credit terms with this partner?".ToUpperInvariant()))
                    {
                        var term = row[i]?.ToString().Trim().ToUpperInvariant();
                        entity.IsTerm = string.IsNullOrWhiteSpace(term) ? (bool?)null : string.Equals(term, "YES", StringComparison.InvariantCultureIgnoreCase);
                    }
                    else if (colName.Contains("TermDays".ToUpperInvariant()) || colName.Contains("How many days are your terms for?".ToUpperInvariant()))
                    {
                        entity.TermDays = row[i].ToString();
                    }
                    else if (colName.Contains("CreditLimit".ToUpperInvariant()) || colName.Contains("What is your credit limit with this vendor?".ToUpperInvariant())
                                                                                || colName.Contains("What is your credit limit with this partner?".ToUpperInvariant()))
                    {
                        entity.CreditLimit = row[i]?.ToString();
                    }

                    else if (colName.Contains("EarlyPaymentDiscountDays".ToUpperInvariant()) || colName.Contains("How many days does this vendor's early payment discount apply for".ToUpperInvariant())
                                                                                             || colName.Contains("How many days does this partner's early payment discount apply for".ToUpperInvariant()))
                    {
                        entity.EarlyPaymentDiscountDays = row[i]?.ToString();
                    }
                    else if (colName.Contains("EarlyPaymentDiscountPercentage".ToUpperInvariant()) || colName.Contains("What percentage early payment discount does this vendor provide?".ToUpperInvariant())
                                                                                                   || colName.Contains("What percentage early payment discount does this partner provide?".ToUpperInvariant()))
                    {
                        entity.EarlyPaymentDiscountPercentage = row[i]?.ToString();
                    }
                    else if (colName.Contains("DepositePercentage".ToUpperInvariant()) || colName.Contains("If deposits required, what percentage?".ToUpperInvariant()))
                    {
                        entity.DepositePercentage = row[i]?.ToString();
                    }

                    else if (colName.Contains("ToolingDepositePercentage".ToUpperInvariant()) || colName.Contains("If tooling deposit required, what percentage?".ToUpperInvariant()))
                    {
                        entity.ToolingDepositePercentage = row[i]?.ToString();
                    }
                }

                //FixPhone
                if (entity.Phone?.Contains("-") == true)
                {
                    entity.Phone = entity.Phone.Replace("-", "");
                }
                if (!string.IsNullOrWhiteSpace(entity.Phone) && !string.IsNullOrWhiteSpace(entity.PhoneCountryCode) &&
                    (!entity.Phone.StartsWith(entity.PhoneCountryCode) || (entity.Phone.StartsWith(entity.PhoneCountryCode) && !Regex.IsMatch(entity.Phone, @"^\s*\+?([0-9\-\.\s\(\)]{3,22})\s*$"))) )
                {
                    entity.Phone = $"{entity.PhoneCountryCode}{entity.Phone}"?.Trim();
                }

                fdList.Add(entity);
            }
            return fdList;
        }

        //New UI Part OnBoarding
        public static IEnumerable<VendorProductDataViewModel> GetProductsStream(string filePath)
        {
            foreach (var data in GetVendorProductsStream(filePath))
            {
                yield return data;
            }
            foreach (var data in GetCustomerProductsStream(filePath))
            {
                yield return data;
            }
        }

        private static IEnumerable<VendorProductDataViewModel> GetVendorProductsStream(string filePath)
        {
            foreach (var data in GetProductsStreamInternal(filePath, new[] {"Active", "Products", "Vendor", "Vendors" }, true))
            {
                data.ThisProductIsMadeByAVendor = true;
                yield return data;
            }
        }

        private static IEnumerable<VendorProductDataViewModel> GetCustomerProductsStream(string filePath)
        {
            foreach (var data in GetProductsStreamInternal(filePath, new[] { "Customers", "Customer" }, false))
            {
                data.ThisProductIsMadeByAVendor = false;
                yield return data;
            }
        }

        private static List<VendorProductDataViewModel> GetProductsStreamInternal(string filePath, string[] tableNames, bool useFirstTable = true)
        {
            DataSet result;

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var excelReader = ExcelReaderFactory.CreateReader(stream))
            {
                result = excelReader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });

                excelReader.Close();
                stream.Close();
            }

            if (result.Tables.Count == 0)
                throw new ApplicationException("Invalid Import File.");

            DataTable dt = null;
            foreach (var tableName in tableNames)
            {
                var table = result.Tables.Cast<DataTable>().FirstOrDefault(t => t.TableName.ToLowerInvariant().Contains(tableName.ToLowerInvariant()));
                if (table != null)
                {
                    dt = table;
                    break;
                }
            }
            if (dt == null && useFirstTable)
            {
                dt = result.Tables[0];
            }

            if(dt == null)
                return new List<VendorProductDataViewModel>();

            var da = dt.AsEnumerable().FirstOrDefault(r => r[0].ToString().ToLowerInvariant().Contains("VendorName".ToLowerInvariant())   || r[0].ToString().ToLowerInvariant().Contains("Vendor Name".ToLowerInvariant()) 
                                                        || r[0].ToString().ToLowerInvariant().Contains("CustomerName".ToLowerInvariant()) || r[0].ToString().ToLowerInvariant().Contains("Customer Name".ToLowerInvariant()));

            string[] columnNames = da?.ItemArray.Select(x => x.ToString()).ToArray()
                                   ?? dt.Columns.Cast<DataColumn>().Where(c => !string.IsNullOrEmpty(c.ColumnName)).Select(x => x.ColumnName).ToArray();

            var fdList = new List<VendorProductDataViewModel>();
            foreach (DataRow row in ((da == null) ? dt.AsEnumerable() : dt.AsEnumerable().SkipWhile(r => r != da)))
            {
                if (row == null || row == da)
                    continue;

                if (string.IsNullOrWhiteSpace(row[0].ToString()) || string.IsNullOrWhiteSpace(row[1].ToString())
                                                                 || string.IsNullOrWhiteSpace(row[2].ToString())
                                                                 || string.IsNullOrWhiteSpace(row[4].ToString())
                                                                 || string.IsNullOrWhiteSpace(row[5].ToString()))
                    continue;

                var entity = new VendorProductDataViewModel();

                for (var i = 0; i < columnNames.Length; i++)
                {
                    var colName = columnNames[i].ToUpperInvariant();

                    if (colName.Contains("VENDORNAME") || colName.Contains("VENDOR NAME")
                                                       || colName.Contains("CustomerName".ToUpperInvariant())
                                                       || colName.Contains("Customer Name".ToUpperInvariant()))
                    {
                        entity.CompanyName = row[i].ToString();
                    }
                    else if (colName.Contains("PRODUCT ID") || colName.Contains("PART ID"))
                    {
                        entity.PartId = row[i] != DBNull.Value ? row[i].ToString() : null;
                    }
                    else if (colName.Contains("PART NAME"))
                    {
                        entity.PartName = row[i] != DBNull.Value ? row[i].ToString() : null;
                    }
                    else if (colName.Contains("DESCRIPTION"))
                    {
                        entity.Description = row[i] != DBNull.Value ? row[i].ToString() : null;
                    }
                    else if (colName.Contains("PRODUCT NUMBER") || colName.Contains("PART NUMBER"))
                    {
                        entity.PartNumber = row[i] != DBNull.Value ? row[i].ToString() : null;
                    }
                    else if (colName.Contains("REVISION") || colName.Contains("REV"))
                    {
                        entity.PartRevision = row[i] != DBNull.Value ? row[i].ToString() : null;
                    }

                    else if (colName.Equals("QUANTITY"))
                    {
                        entity.Quantity1 = row[i] != DBNull.Value ? row[i].ToString() : null;
                    }
                    else if (colName.Equals("UNIT PRICE") || colName.Equals("UNIT") || colName.Equals("PRICE"))
                    {
                        entity.UnitPrice1 = row[i] != DBNull.Value ? row[i].ToString() : null;
                    }
                    else if (colName.Contains("QUANTITY 1"))
                    {
                        entity.Quantity1 = row[i] != DBNull.Value ? row[i].ToString() : null;
                    }
                    else if (colName.Contains("UNIT PRICE 1") || colName.Contains("UNIT 1") || colName.Contains("PRICE 1"))
                    {
                        entity.UnitPrice1 = row[i] != DBNull.Value ? row[i].ToString() : null;
                    }
                    else if (colName.Contains("QUANTITY 2"))
                    {
                        entity.Quantity2 = row[i] != DBNull.Value ? row[i].ToString() : null;
                    }
                    else if (colName.Contains("UNIT PRICE 2") || colName.Contains("UNIT 2") || colName.Contains("PRICE 2"))
                    {
                        entity.UnitPrice2 = row[i] != DBNull.Value ? row[i].ToString() : null;
                    }
                    else if (colName.Contains("QUANTITY 3"))
                    {
                        entity.Quantity3 = row[i] != DBNull.Value ? row[i].ToString() : null;
                    }
                    else if (colName.Contains("UNIT PRICE 3") || colName.Contains("UNIT 3") || colName.Contains("PRICE 3"))
                    {
                        entity.UnitPrice3 = row[i] != DBNull.Value ? row[i].ToString() : null;
                    }
                    else if (colName.Contains("QUANTITY 4"))
                    {
                        entity.Quantity4 = row[i] != DBNull.Value ? row[i].ToString() : null;
                    }
                    else if (colName.Contains("UNIT PRICE 4") || colName.Contains("UNIT 4") || colName.Contains("PRICE 4"))
                    {
                        entity.UnitPrice4 = row[i] != DBNull.Value ? row[i].ToString() : null;
                    }
                    else if (colName.Contains("QUANTITY 5"))
                    {
                        entity.Quantity5 = row[i] != DBNull.Value ? row[i].ToString() : null;
                    }
                    else if (colName.Contains("UNIT PRICE 5") || colName.Contains("UNIT 5") || colName.Contains("PRICE 5"))
                    {
                        entity.UnitPrice5 = row[i] != DBNull.Value ? row[i].ToString() : null;
                    }
                    else if (colName.Contains("QUANTITY 6"))
                    {
                        entity.Quantity6 = row[i] != DBNull.Value ? row[i].ToString() : null;
                    }
                    else if (colName.Contains("UNIT PRICE 6") || colName.Contains("UNIT 6") || colName.Contains("PRICE 6"))
                    {
                        entity.UnitPrice6 = row[i] != DBNull.Value ? row[i].ToString() : null;
                    }
                    else if (colName.Contains("QUANTITY 7"))
                    {
                        entity.Quantity7 = row[i] != DBNull.Value ? row[i].ToString() : null;
                    }
                    else if (colName.Contains("UNIT PRICE 7") || colName.Contains("UNIT 7") || colName.Contains("PRICE 7"))
                    {
                        entity.UnitPrice7 = row[i] != DBNull.Value ? row[i].ToString() : null;
                    }
                    else if (colName.Contains("UNITOFMEASUREMENT"))
                    {
                        entity.UnitOfMeasurement = row[i] != DBNull.Value ? row[i].ToString() : null;
                    }
                 }

                fdList.Add(entity);
            }

            return fdList;
        }
    }
}