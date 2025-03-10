using Common;
using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.QueryFilter;
using Intuit.Ipp.ReportService;
//using Microsoft.Extensions.Logging;
using Omnae.Common;
using Omnae.QuickBooks.ViewModels;
using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceProcess;
using System.Web.UI.WebControls;

namespace Omnae.QuickBooks.QBO
{

    public class QboAccessor
    {
        internal static readonly Dictionary<int, int> DicTerms = new Dictionary<int, int>()
        {
            { 0, 6 },
            { 7, 9 },
            { 15, 2 },
            { 20, 7 },
            { 25, 8 },
            { 30, 3 },
            { 45, 5 },
            { 60, 4 },
        };

        internal static ExchangeRate GetExchangeRate(ServiceContext context, DateTime date, ILogger log = null)
        {
            QueryService<ExchangeRate> exchangeRateQueryService = new QueryService<ExchangeRate>(context);
            string query = $"select * from exchangerate where sourcecurrencycode = 'CAD' and asofdate >= '{date.AddDays(-1).ToString("yyyy-MM-dd")}'";
            var exchangeRate = exchangeRateQueryService.ExecuteIdsQuery(query).LastOrDefault();

#if false
            if (log != null)
            {
                
                if (exchangeRate == null)
                {
                    log.Error($"exchangeRate is null");
                    throw new Exception("Exchang Rate is null");
                }
                else
                {
                    log.Information($"exchangeRate.Rate: {exchangeRate.Rate}");
                }
            }
#endif
            return exchangeRate;
        }

        internal static async System.Threading.Tasks.Task<ExchangeRate> GetExchangeRate(ServiceContext context, DateTime date, string sourceCurrencyCode, ILogger log = null)
        {
            QueryService<ExchangeRate> exchangeRateQueryService = new QueryService<ExchangeRate>(context);
            string query = $"select * from exchangerate where sourcecurrencycode = '{sourceCurrencyCode}' and asofdate >= '{date.AddDays(-1).ToString("yyyy-MM-dd")}'";
            var exchangeRate = await System.Threading.Tasks.Task.FromResult(exchangeRateQueryService.ExecuteIdsQuery(query).LastOrDefault());
#if false
            if (log != null)
            {
                log.Information($"date = {date}");
                log.Information($"date (yyyy-MM-dd) = {date.ToString("yyyy-MM-dd")}");
                if (exchangeRate == null)
                {
                    log.Error($"exchangeRate is null");
                    throw new Exception("Exchang Rate is null");
                }
                else
                {
                    log.Information($"exchangeRate.Rate: {exchangeRate.Rate}");
                }
            }
#endif
            return exchangeRate;
        }



        public static TaxRate GetTaxRate(ServiceContext context, string country, string countrySubDivisionCode)
        {
            TaxCode taxCode = GetTaxCode(context, country, countrySubDivisionCode);
            if (taxCode != null && taxCode.SalesTaxRateList != null &&
                taxCode.SalesTaxRateList.TaxRateDetail != null && taxCode.SalesTaxRateList.TaxRateDetail.Any())
            {
                var taxValue = taxCode.SalesTaxRateList.TaxRateDetail.First().TaxRateRef.Value;
                QueryService<TaxRate> taxRateQueryService = new QueryService<TaxRate>(context);
                string query = $"select * from TaxRate where Id = '{taxValue}'";
                TaxRate taxRate = taxRateQueryService.ExecuteIdsQuery(query).FirstOrDefault();
                return taxRate;
            }
            return null;
        }

        internal static TaxCode GetTaxCode(ServiceContext context, string country, string countrySubDivisionCode)
        {
            string taxName = string.Empty;
            if (country == COUNTRY_NAME.Canada)
            {
                if (countrySubDivisionCode == "ON")
                {
                    taxName = TAX_TYPE.HST_ON;
                }
                else if (countrySubDivisionCode == "NB")
                {
                    taxName = TAX_TYPE.HST_NB;
                }
                else if (countrySubDivisionCode == "NL")
                {
                    taxName = TAX_TYPE.HST_NL;
                }
                else if (countrySubDivisionCode == "NS")
                {
                    taxName = TAX_TYPE.HST_NS;
                }
                else if (countrySubDivisionCode == "PE")
                {
                    taxName = TAX_TYPE.HST_PE;
                }
                else if (countrySubDivisionCode == "BC" ||
                         countrySubDivisionCode == "AB" ||
                         countrySubDivisionCode == "NU" ||
                         countrySubDivisionCode == "NT" ||
                         countrySubDivisionCode == "YT")
                {
                    taxName = TAX_TYPE.GST;
                }
                else
                {
                    taxName = TAX_TYPE.Exempt;
                }
            }
            else
            {
                taxName = TAX_TYPE.Exempt;
            }
            QueryService<TaxCode> taxCodeQueryService = new QueryService<TaxCode>(context);
            string query = $"select * from TaxCode where Name = '{taxName}'";
            TaxCode taxCode = taxCodeQueryService.ExecuteIdsQuery(query).FirstOrDefault();
            return taxCode;
        }

        internal static Customer CreateCustomer(ServiceContext context, CustomerInfo customerInfo)
        {
            Customer customer = new Customer();
            customer.Taxable = customerInfo.Taxable;
            customer.TaxableSpecified = true;

            if (customerInfo.ShipAddr == null)
            {
                customerInfo.ShipAddr = customerInfo.BillAddr;
            }
            TaxCode taxcode = GetTaxCode(context, customerInfo.ShipAddr.Country, customerInfo.ShipAddr.CountrySubDivisionCode);
            if (taxcode != null)
            {
                customer.DefaultTaxCodeRef = new ReferenceType
                {
                    name = taxcode.Name,
                    Value = taxcode.Id,
                };
            }
            customer.BillAddr = customerInfo.BillAddr;
            customer.ShipAddr = customerInfo.ShipAddr;
            string query = string.Empty;

            if (customerInfo.Term != null)
            {
                string termName = string.Format("Net {0}", customerInfo.Term);
                QueryService<Term> termQueryService = new QueryService<Term>(context);
                query = $"select * from Term where Name = '{termName}'";
                Term term = termQueryService.ExecuteIdsQuery(query).FirstOrDefault();

                // create new Term in QBO
                if (term == null)
                {
                    term = new Term();
                    term.Name = string.Format("Net {0}", customerInfo.Term);
                    term.Active = true;
                    term.ActiveSpecified = true;
                    term.Type = "STANDARD";
                    term.AnyIntuitObjects = new Object[] { customerInfo.Term };
                    term.ItemsElementName = new ItemsChoiceType[] { ItemsChoiceType.DueDays };
                    Helper.Add<Term>(context, term);
                }
                customer.SalesTermRef = new ReferenceType
                {
                    name = term.Name,
                    Value = term.Id,
                };
            }

            QueryService<PaymentMethod> paymentService = new QueryService<PaymentMethod>(context);
            query = $"select * from PaymentMethod where Id = '{customerInfo.PaymentMethod.ToString()}'";
            PaymentMethod payment = paymentService.ExecuteIdsQuery(query).FirstOrDefault();
            if (payment == null)
            {
                query = "select * from PaymentMethod where Id = '2'";
                payment = paymentService.ExecuteIdsQuery(query).First();
            }
            customer.PaymentMethodRef = new ReferenceType
            {
                name = payment.Name,
                Value = payment.Id,
            };

            customer.Notes = customerInfo.Notes;

            customer.Balance = customerInfo.Balance;
            customer.BalanceSpecified = true;
            customer.CreditLimit = customerInfo.Balance;
            customer.CreditLimitSpecified = true;


            customer.BalanceWithJobs = customerInfo.BalanceWithJobs;
            customer.BalanceWithJobsSpecified = true;

            customer.PreferredDeliveryMethod = "Print";
            customer.ResaleNum = "ResaleNum";

            customer.Title = customerInfo.Title;
            customer.GivenName = customerInfo.GivenName;
            customer.MiddleName = customerInfo.MiddleName;
            customer.FamilyName = customerInfo.FamilyName;
            customer.Suffix = customerInfo.Suffix;
            customer.FullyQualifiedName = customerInfo.DisplayName;
            customer.CompanyName = customerInfo.CompanyName;
            customer.DisplayName = customerInfo.DisplayName;
            customer.PrintOnCheckName = customer.DisplayName;

            customer.Active = true;
            customer.ActiveSpecified = true;


            customer.PrimaryPhone = customerInfo.primaryPhone;
            customer.AlternatePhone = customerInfo.alternatePhone;
            customer.Mobile = customerInfo.mobile;
            customer.Fax = customerInfo.fax;
            customer.PrimaryEmailAddr = customerInfo.primaryEmailAddr;
            customer.WebAddr = customerInfo.webAddr;

            customer.CurrencyRef = new ReferenceType()
            {
                name = customerInfo.CurrencyText,
                Value = customerInfo.CurrencyText
            };

            //customer.Taxable TaxCodeRef = new ReferenceType() { name = taxcode.Name, Value = taxcode.Id },

            return customer;
        }

        internal static Vendor CreateVendor(ServiceContext context, CustomerInfo vendorInfo)
        {
            Vendor vendor = new Vendor();
            if (vendorInfo.ShipAddr == null)
            {
                vendorInfo.ShipAddr = vendorInfo.BillAddr;
            }
            TaxCode taxcode = GetTaxCode(context, vendorInfo.ShipAddr.Country, vendorInfo.ShipAddr.CountrySubDivisionCode);
            if (taxcode != null)
            {
                vendor.DefaultTaxCodeRef = new ReferenceType
                {
                    name = taxcode.Name,
                    Value = taxcode.Id,
                };
            }
            vendor.BillAddr = vendorInfo.BillAddr;
            vendor.ShipAddr = vendorInfo.ShipAddr;

            vendor.Title = vendorInfo.Title;
            vendor.GivenName = vendorInfo.GivenName;
            vendor.MiddleName = vendorInfo.MiddleName;
            vendor.FamilyName = vendorInfo.FamilyName;
            vendor.Suffix = vendorInfo.Suffix;
            vendor.CompanyName = vendorInfo.CompanyName;
            vendor.DisplayName = vendorInfo.DisplayName;
            //vendor.PrintOnCheckName = "PrintOnCheckName";

            vendor.Active = true;
            vendor.ActiveSpecified = true;
            vendor.PrimaryPhone = vendorInfo.primaryPhone;
            vendor.AlternatePhone = vendorInfo.primaryPhone;
            vendor.Mobile = vendorInfo.mobile;
            vendor.Fax = vendorInfo.fax;
            vendor.PrimaryEmailAddr = vendorInfo.primaryEmailAddr;
            vendor.WebAddr = vendorInfo.webAddr;

            if (vendorInfo.Term != null)
            {
                string termName = string.Format("Net {0}", vendorInfo.Term);
                QueryService<Term> termQueryService = new QueryService<Term>(context);
                string query = $"select * from Term where Name = '{termName}'";
                Term term = termQueryService.ExecuteIdsQuery(query).FirstOrDefault();


                // create new Term in QBO
                if (term == null)
                {
                    term = new Term();
                    term.Name = string.Format("Net {0}", vendorInfo.Term);
                    term.Active = true;
                    term.ActiveSpecified = true;
                    term.Type = "STANDARD";

                    //term.DiscountPercent = new Decimal(50.00);
                    //term.DiscountPercentSpecified = true;
                    term.AnyIntuitObjects = new Object[] { vendorInfo.Term };
                    term.ItemsElementName = new ItemsChoiceType[] { ItemsChoiceType.DueDays };
                    Helper.Add<Term>(context, term);
                }
                vendor.TermRef = new ReferenceType
                {
                    name = term.Name,
                    Value = term.Id,
                };
            }

            vendor.CurrencyRef = new ReferenceType()
            {
                name = vendorInfo.CurrencyText,
                Value = vendorInfo.CurrencyText
            };

            return vendor;
        }

        internal static async System.Threading.Tasks.Task<Estimate> CreateEstimate(ServiceContext context, PurchaseOrderViewModel model, ILogger log = null)
        {
            Estimate estimate = new Estimate();

            estimate.AutoDocNumber = true;
            estimate.AutoDocNumberSpecified = true;

            estimate.ExpirationDate = DateTime.UtcNow.Date.AddDays(model.Term != null ? model.Term.Value : 30);
            estimate.ExpirationDateSpecified = true;
            estimate.TxnDate = DateTime.UtcNow.Date;
            estimate.TxnDateSpecified = true;
            estimate.TrackingNum = model.TrackingNumber;
            estimate.ShipMethodRef = new ReferenceType
            {
                name = model.CarrierName,
                Value = model.CarrierName,
            };


            QueryService<Customer> customerQueryService = new QueryService<Customer>(context);
            Customer customer = null;
            string query = string.Empty;
            try
            {
                if (model.QboId != null)
                {
                    query = $"select * from Customer where Id = '{model.QboId}'";
                    customer = customerQueryService.ExecuteIdsQuery(query).FirstOrDefault();
                }
                else
                {
                    query = $"select * from Customer where DisplayName = '{model.CustomerName}'";
                    customer = customerQueryService.ExecuteIdsQuery(query).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.RetrieveErrorMessage());
            }

            if (customer == null)
            {
                throw new Exception("Could not find customer in QBO");
            }

            estimate.CustomerRef = new ReferenceType()
            {
                name = customer.CompanyName,
                Value = customer.Id,
            };

            var addr = new PhysicalAddress()
            {
                Line1 = model.AddressLine1,
                Line2 = model.AddressLine2,
                City = model.City,
                Country = model.CountryName,
                CountrySubDivisionCode = model.State,
                PostalCode = model.PostalCode ?? model.ZipCode,

            };

            estimate.BillAddr = customer.BillAddr;
            estimate.ShipAddr = addr;

            EmailAddress billEmail = new EmailAddress();
            billEmail.Address = model.EmailAddress;
            billEmail.Default = true;
            billEmail.DefaultSpecified = true;
            billEmail.Tag = "Tag";
            estimate.BillEmail = billEmail;

            // Term
            var term = FindQboTerm(context, model.Term);
            estimate.SalesTermRef = new ReferenceType
            {
                name = term == null ? "Due on receipt" : term.Name,
                Value = term == null ? "1" : term.Id,
            };


            estimate.ShipMethodRef = new ReferenceType()
            {
                name = model.CarrierName,
                Value = model.CarrierName,
            };

            estimate.ShipDate = model.ShipDate;
            estimate.ShipDateSpecified = true;


            Account depositAccount = CreateAccount(context, AccountTypeEnum.Bank, AccountClassificationEnum.Asset, "PadtechAccount");
            estimate.DepositToAccountRef = new ReferenceType()
            {
                name = depositAccount.Name,
                Value = depositAccount.Id
            };


            QueryService<Item> itemQueryService = new QueryService<Item>(context);
            query = $"select * from Item where Name = '{model.ProductName}'";
            Item item = itemQueryService.ExecuteIdsQuery(query).FirstOrDefault<Item>();
            if (item == null)
            {
                try
                {
                    item = CreateNewItem(context, model);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }

            estimate.TotalAmt = model.Total; //model.UnitPrice * model.Quantity + model.CustomerToolingCharge;

            // GST
            if (customer.ShipAddr == null)
            {
                customer.ShipAddr = customer.BillAddr;
            }

            TaxCode taxcode = GetTaxCode(context, customer.ShipAddr.Country, customer.ShipAddr.CountrySubDivisionCode);
            if (taxcode != null && taxcode.SalesTaxRateList != null && taxcode.SalesTaxRateList.TaxRateDetail.Count() > 0)
            {
                TaxRate taxRateToFind = new TaxRate();
                taxRateToFind.Id = taxcode.SalesTaxRateList.TaxRateDetail[0].TaxRateRef.Value;
                TaxRate taxRate = Helper.FindById<TaxRate>(context, taxRateToFind);
                model.SalesTax = estimate.TotalAmt * (taxRate.RateValue / 100);
                estimate.TotalAmt += model.SalesTax;
            }


            List<Line> lineList = new List<Line>();
            Line line = new Line();
            if (model.Quantity > model.NumberSampleIncluded || model.UnitPrice > 0)
            {
                if (string.IsNullOrEmpty(model.ProductDescription) == false)
                {
                    line.Description = string.Format("Part Id: {0}, Part No: {1}, Rev: {2}, Part Description: {3}",
                                                      model.ProductId, model.PartNumber, model.PartRevision, model.ProductDescription);
                }
                else
                {
                    line.Description = string.Format("Part Id: {0}, Part No: {1}, Rev: {2}",
                                                      model.ProductId, model.PartNumber, model.PartRevision);
                }

                line.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                line.DetailTypeSpecified = true;
                line.AnyIntuitObject = new SalesItemLineDetail()
                {
                    ServiceDateSpecified = true,
                    ServiceDate = DateTime.UtcNow,
                    ItemRef = new ReferenceType() { name = item.Name, Value = item.Id },
                    TaxCodeRef = taxcode != null ? new ReferenceType() { name = taxcode.Name, Value = taxcode.Id } : null,
                    Qty = model.Quantity,
                    QtySpecified = true,

                    // UnitPrice
                    AnyIntuitObject = model.UnitPrice,
                    ItemElementName = ItemChoiceType.UnitPrice,
                };
                line.Amount = model.UnitPrice * model.Quantity;
                line.AmountSpecified = true;


            }
            else
            {
                line.Description = "Tooling Setup Charges";
                
                line.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                line.DetailTypeSpecified = true;
                line.AnyIntuitObject = new SalesItemLineDetail()
                {
                    ServiceDateSpecified = true,
                    ServiceDate = DateTime.UtcNow,
                    ItemRef = new ReferenceType() { name = item.Name, Value = item.Id },
                    TaxCodeRef = taxcode != null ? new ReferenceType() { name = taxcode.Name, Value = taxcode.Id } : null,
                    Qty = model.NumberSampleIncluded,
                    QtySpecified = true,
                };
                line.Amount = model.Total;
                line.AmountSpecified = true;
            }

            lineList.Add(line);
            estimate.Line = lineList.ToArray();
#if false
            // Add your customized item name as a CustomField
            var unitOfMeasurement = Enum.GetName(typeof(MEASUREMENT_UNITS), model.UnitOfMeasurement ?? 0);
            CustomField unitOfQty = new CustomField { DefinitionId = "3", Name = "Unit of Qty", Type = CustomFieldTypeEnum.StringType, AnyIntuitObject = unitOfMeasurement };
            estimate.CustomField = new CustomField[] { unitOfQty };

#endif
            ExchangeRate exchangeRate = await GetExchangeRate(context, DateTime.Now, customer.CurrencyRef.Value, log);
            if (exchangeRate != null)
            {
                estimate.ExchangeRate = exchangeRate.Rate;
                estimate.ExchangeRateSpecified = true;
            }

            estimate.CurrencyRef = customer.CurrencyRef;
            return estimate;
        }

        private readonly Dictionary<string, string> IncomeMap = new Dictionary<string, string>
        {
            { "1601", "Elastomers" },
            { "1602", "Graphic Overlays" },
            { "1603", "Labels" },
            { "1604", "Membrane Switches" },
            { "1605", "Other" },
            { "1606", "Precision Plastics" },
            { "1607", "Precision Metals" },
        };

        private readonly Dictionary<string, string> CostMap = new Dictionary<string, string>
        {
            { "1701", "Elastomers" },
            { "1702", "Graphic Overlays" },
            { "1703", "Labels" },
            { "1704", "Membrane Switches" },
            { "1705", "Other" },
            { "1706", "Precision Plastics" },
            { "1707", "Precision Metals" },
        };

        internal static Account CreateAccount(ServiceContext context, AccountTypeEnum accountType, AccountClassificationEnum classification, string accountName)
        {
            Account account = new Account();

            if (accountType == AccountTypeEnum.CostofGoodsSold)
            {
                account.Name = string.Format("01 {0}", accountName);
            }
            else if (accountType == AccountTypeEnum.Income)
            {
                account.Name = string.Format("02 {0}", accountName);
            }
            account.FullyQualifiedName = accountName;
            account.Classification = classification;
            account.ClassificationSpecified = true;
            account.AccountType = accountType;
            account.AccountTypeSpecified = true;

            account.CurrencyRef = new ReferenceType()
            {
                name = "United States Dollar",
                Value = "USD"
            };

            return account;
        }



        internal static Account FindAccount(ServiceContext context, AccountTypeEnum accountType, AccountClassificationEnum classification, string accountName)
        {
            Account account = null;
            QueryService<Account> accountQueryService = new QueryService<Account>(context);
            string temp = string.Empty;
            if (accountType == AccountTypeEnum.CostofGoodsSold)
            {
                temp = string.Format("01 {0}", accountName);
            }
            else if (accountType == AccountTypeEnum.Income)
            {
                temp = string.Format("02 {0}", accountName);
            }
            string query = $"select * from Account where Name = '{temp}'";
            account = accountQueryService.ExecuteIdsQuery(query).FirstOrDefault<Account>();
            return account;
        }



        internal static async System.Threading.Tasks.Task<Invoice> CreateInvoice(ServiceContext context, InvoiceViewModel model, ILogger log)
        {

            //Global invoice
            Invoice invoice = new Invoice();

            QueryService<Customer> customerQueryService = new QueryService<Customer>(context);
            Customer customer = null;
            string query = string.Empty;
            if (model.QboId != null)
            {
                query = $"select * from Customer where Id = '{model.QboId}'";
                customer = customerQueryService.ExecuteIdsQuery(query).FirstOrDefault();
            }
            else
            {
                query = $"select * from Customer where DisplayName = '{model.CompanyName}'";
                customer = customerQueryService.ExecuteIdsQuery(query).FirstOrDefault();
            }
            if (customer == null)
            {
                throw new Exception("Could not find customer in QBO");
            }

            invoice.CustomerRef = new ReferenceType()
            {
                name = customer.DisplayName,
                Value = customer.Id,
            };

            var addr = new PhysicalAddress()
            {
                Line1 = model.AddressLine1,
                Line2 = model.AddressLine2,
                City = model.City,
                Country = model.CountryName,
                CountrySubDivisionCode = model.State,
                PostalCode = model.PostalCode ?? model.ZipCode,

            };

            invoice.BillAddr = customer.BillAddr;
            invoice.ShipAddr = addr;

            invoice.DueDate = model.DueDate;
            invoice.DueDateSpecified = true;

            invoice.ShipDate = model.ShipDate;
            invoice.ShipDateSpecified = true;

            invoice.ShipMethodRef = new ReferenceType
            {
                name = model.CarrierName,
                Value = model.CarrierName
            };
            invoice.TrackingNum = model.TrackingNumber;

            QueryService<Item> itemQueryService = new QueryService<Item>(context);
            query = $"select * from Item where Name = '{model.ProductName}'";
            Item item = itemQueryService.ExecuteIdsQuery(query).FirstOrDefault<Item>();
            if (item == null)
            {
                item = CreateNewItem(context, model);
            }


            invoice.AutoDocNumber = true;
            invoice.AutoDocNumberSpecified = true;

            // Term
            var term = FindQboTerm(context, model.Term);
            invoice.SalesTermRef = new ReferenceType
            {
                name = term == null ? "Due on receipt" : term.Name,
                Value = term == null ? "1" : term.Id,
            };


            invoice.ApplyTaxAfterDiscount = false;
            invoice.ApplyTaxAfterDiscountSpecified = true;

            invoice.PrintStatus = PrintStatusEnum.NotSet;
            invoice.PrintStatusSpecified = true;
            invoice.EmailStatus = EmailStatusEnum.NotSet;
            invoice.EmailStatusSpecified = true;

            EmailAddress billEmail = new EmailAddress();
            billEmail.Address = model.EmailAddress;
            billEmail.Default = true;
            billEmail.DefaultSpecified = true;
            billEmail.Tag = "Tag";
            invoice.BillEmail = billEmail;

            EmailAddress billEmailcc = new EmailAddress();
            billEmailcc.Address = model.EmailAddress;
            billEmailcc.Default = true;
            billEmailcc.DefaultSpecified = true;
            billEmailcc.Tag = "Tag";
            invoice.BillEmailCc = billEmailcc;

            EmailAddress billEmailbcc = new EmailAddress();
            billEmailbcc.Address = model.EmailAddress;
            billEmailbcc.Default = true;
            billEmailbcc.DefaultSpecified = true;
            billEmailbcc.Tag = "Tag";
            invoice.BillEmailBcc = billEmailbcc;

            invoice.Balance = model.Total;
            invoice.BalanceSpecified = true;
            invoice.TxnDate = DateTime.UtcNow.Date;
            invoice.TxnDateSpecified = true;

            List<LinkedTxn> LinkedTxnList = new List<LinkedTxn>();
            LinkedTxn linkedTxn = new LinkedTxn();
            linkedTxn.TxnId = model.EstimateId;
            linkedTxn.TxnType = TxnTypeEnum.Estimate.ToString();
            LinkedTxnList.Add(linkedTxn);

            invoice.LinkedTxn = LinkedTxnList.ToArray();



            invoice.PONumber = model.PONumber;

            List<CustomField> customFields = new List<CustomField>();

            var customField1 = new CustomField
            {
                DefinitionId = "1",
                Name = "P.O. No",
                Type = CustomFieldTypeEnum.StringType,
                AnyIntuitObject = model.PONumber,
            };
            var customField2 = new CustomField
            {
                DefinitionId = "2",
                Name = "S.O.NO.",
                Type = CustomFieldTypeEnum.StringType,
                AnyIntuitObject = model.EstimateNumber,
            };
            var customField3 = new CustomField
            {
                DefinitionId = "3",
                Name = "Buyer",
                Type = CustomFieldTypeEnum.StringType,
                AnyIntuitObject = model.Buyer,
            };

            customFields.Add(customField1);
            customFields.Add(customField2);
            customFields.Add(customField3);
            invoice.CustomField = customFields.ToArray();

            invoice.CustomerMemo = new MemoRef
            {
                Value = "Thank you for your business and have a great day!",
            };

            invoice.TotalAmt = model.Total;

            // GST
            if (customer.ShipAddr == null)
            {
                customer.ShipAddr = customer.BillAddr;
            }

            TaxCode taxcode = GetTaxCode(context, customer.ShipAddr.Country, customer.ShipAddr.CountrySubDivisionCode);
            if (taxcode != null && taxcode.SalesTaxRateList != null && taxcode.SalesTaxRateList.TaxRateDetail.Count() > 0)
            {
                TaxRate taxRateToFind = new TaxRate();
                taxRateToFind.Id = taxcode.SalesTaxRateList.TaxRateDetail[0].TaxRateRef.Value;
                TaxRate taxRate = Helper.FindById<TaxRate>(context, taxRateToFind);
                model.SalesTax = invoice.TotalAmt * (taxRate.RateValue / 100);
                invoice.TotalAmt += model.SalesTax;
            }

            List<Line> lineList = new List<Line>();
            Line line = new Line();
            if (model.Quantity > model.NumberSampleIncluded || model.UnitPrice > 0)
            {
                if (string.IsNullOrEmpty(model.ProductDescription) == false)
                {
                    line.Description = string.Format("Part Id: {0}, Part No: {1}, Rev: {2}, Part Description: {3}",
                                                      model.ProductId, model.PartNumber, model.PartRevision, model.ProductDescription);
                }
                else
                {
                    line.Description = string.Format("Part Id: {0}, Part No: {1}, Rev: {2}",
                                                      model.ProductId, model.PartNumber, model.PartRevision);
                }


                line.LinkedTxn = LinkedTxnList.ToArray();
                line.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                line.DetailTypeSpecified = true;
                line.AnyIntuitObject = new SalesItemLineDetail()
                {
                    ServiceDateSpecified = true,
                    ServiceDate = DateTime.UtcNow,
                    ItemRef = new ReferenceType() { name = item.Name, Value = item.Id },
                    TaxCodeRef = taxcode != null ? new ReferenceType() { name = taxcode.Name, Value = taxcode.Id } : null,
                    Qty = model.Quantity,
                    QtySpecified = true,

                    // UnitPrice
                    AnyIntuitObject = model.UnitPrice,
                    ItemElementName = ItemChoiceType.UnitPrice,
                };
                line.Amount = model.Quantity * model.UnitPrice;
            }
            else
            {
                line.Description = "Tooling Setup Charges";
                line.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                line.DetailTypeSpecified = true;
                line.AnyIntuitObject = new SalesItemLineDetail()
                {
                    ServiceDateSpecified = true,
                    ServiceDate = DateTime.UtcNow,
                    ItemRef = new ReferenceType() { name = item.Name, Value = item.Id },
                    TaxCodeRef = taxcode != null ? new ReferenceType() { name = taxcode.Name, Value = taxcode.Id } : null,
                    Qty = model.NumberSampleIncluded,
                    QtySpecified = true,
                };

                line.Amount = model.Total;

            }
            line.AmountSpecified = true;
            lineList.Add(line);
            invoice.Line = lineList.ToArray();

            invoice.AllowOnlineCreditCardPayment = false;

            ExchangeRate exchangeRate = await GetExchangeRate(context, DateTime.Now, customer.CurrencyRef.Value, log);
            if (exchangeRate != null)
            {
                invoice.ExchangeRate = exchangeRate.Rate;
                invoice.ExchangeRateSpecified = true;
            }

            invoice.CurrencyRef = customer.CurrencyRef;


            return invoice;
        }

        internal static Invoice CreateStripeInvoice(ServiceContext context, StripeInvoiceViewModel model, ILogger log)
        {

            //Global invoice
            Invoice invoice = new Invoice();

            QueryService<Customer> customerQueryService = new QueryService<Customer>(context);
            Customer customer = null;
            string query = string.Empty;
            if (!string.IsNullOrEmpty(model.QboId))
            {
                query = $"select * from Customer where Id = '{model.QboId}'";
                customer = customerQueryService.ExecuteIdsQuery(query).FirstOrDefault();
            }
            else
            {
                query = $"select * from Customer where DisplayName = '{model.CompanyName}'";
                customer = customerQueryService.ExecuteIdsQuery(query).FirstOrDefault();
            }
            if (customer == null)
            {
                throw new Exception("Could not find customer in QBO");
            }

            invoice.CustomerRef = new ReferenceType()
            {
                name = customer.DisplayName,
                Value = customer.Id,
            };

            var addr = new PhysicalAddress()
            {
                Line1 = model.AddressLine1,
                Line2 = model.AddressLine2,
                City = model.City,
                Country = model.CountryName,
                CountrySubDivisionCode = model.State,
                PostalCode = model.PostalCode ?? model.ZipCode,

            };

            invoice.BillAddr = customer.BillAddr;
            invoice.ShipAddr = addr;

            if (model.DueDate != null)
            {
                invoice.DueDate = model.DueDate.Value;
            }

            invoice.DueDateSpecified = true;
            invoice.AutoDocNumber = true;
            invoice.AutoDocNumberSpecified = true;

            invoice.ApplyTaxAfterDiscount = false;
            invoice.ApplyTaxAfterDiscountSpecified = true;

            invoice.PrintStatus = PrintStatusEnum.NotSet;
            invoice.PrintStatusSpecified = true;
            invoice.EmailStatus = EmailStatusEnum.NotSet;
            invoice.EmailStatusSpecified = true;

            EmailAddress billEmail = new EmailAddress();
            billEmail.Address = model.EmailAddress;
            billEmail.Default = true;
            billEmail.DefaultSpecified = true;
            billEmail.Tag = "Tag";
            invoice.BillEmail = billEmail;

            EmailAddress billEmailcc = new EmailAddress();
            billEmailcc.Address = model.EmailAddress;
            billEmailcc.Default = true;
            billEmailcc.DefaultSpecified = true;
            billEmailcc.Tag = "Tag";
            invoice.BillEmailCc = billEmailcc;

            EmailAddress billEmailbcc = new EmailAddress();
            billEmailbcc.Address = model.EmailAddress;
            billEmailbcc.Default = true;
            billEmailbcc.DefaultSpecified = true;
            billEmailbcc.Tag = "Tag";
            invoice.BillEmailBcc = billEmailbcc;

            invoice.Balance = model.Total;
            invoice.BalanceSpecified = true;
            invoice.TxnDate = DateTime.UtcNow.Date;
            invoice.TxnDateSpecified = true;

            invoice.CustomerMemo = new MemoRef
            {
                //Value = "Thank you for your business and have a great day!",
                Value = $"Stripe Invoice Id      :   {model.InvoiceId}, \n" +
                        $"Stripe Invoice Number  :   {model.InvoiceNumber}",
            };

            invoice.TotalAmt = model.Total;

            // GST
            if (customer.ShipAddr == null)
            {
                customer.ShipAddr = customer.BillAddr;
            }

            TaxCode taxcode = GetTaxCode(context, customer.ShipAddr.Country, customer.ShipAddr.CountrySubDivisionCode);
            if (taxcode != null && taxcode.SalesTaxRateList != null && taxcode.SalesTaxRateList.TaxRateDetail.Count() > 0)
            {
                TaxRate taxRateToFind = new TaxRate();
                taxRateToFind.Id = taxcode.SalesTaxRateList.TaxRateDetail[0].TaxRateRef.Value;
                TaxRate taxRate = Helper.FindById<TaxRate>(context, taxRateToFind);
                invoice.TotalAmt += invoice.TotalAmt * (taxRate.RateValue / 100);
            }

            List<Line> lineList = new List<Line>();
            Line line = new Line();
            line.Description = "Subscription Fee";
            line.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
            line.DetailTypeSpecified = true;
            line.AnyIntuitObject = new SalesItemLineDetail()
            {
                ServiceDateSpecified = true,
                ServiceDate = DateTime.UtcNow,
                //ItemRef = new ReferenceType() { name = item.Name, Value = item.Id },
                TaxCodeRef = taxcode != null ? new ReferenceType() { name = taxcode.Name, Value = taxcode.Id } : null,
                TaxInclusiveAmt = invoice.TotalAmt,
                TaxInclusiveAmtSpecified = true,
                Qty = model.Quantity,
                QtySpecified = true,

                // UnitPrice
                AnyIntuitObject = model.Total,
                ItemElementName = ItemChoiceType.UnitPrice,
            };

            line.Amount = invoice.TotalAmt;

            line.AmountSpecified = true;
            lineList.Add(line);
            invoice.Line = lineList.ToArray();

            invoice.AllowOnlineCreditCardPayment = true;
            invoice.AllowOnlinePayment = true;

            ExchangeRate exchangeRate = GetExchangeRate(context, DateTime.Now, log);
            if (exchangeRate != null)
            {
                invoice.ExchangeRate = exchangeRate.Rate;
                invoice.ExchangeRateSpecified = true;
            }

            invoice.CurrencyRef = new ReferenceType
            {
                name = "United States Dollar",
                Value = "USD"
            };



            return invoice;
        }


        internal static Payment CreatePayment(ServiceContext context, PaymentViewModel model)
        {
            Payment payment = new Payment();
            payment.TxnDate = DateTime.UtcNow.Date;
            payment.TxnDateSpecified = true;

            payment.PaymentRefNum = model.PaymentRefNum;
            payment.PaymentTypeSpecified = true;
            payment.UnappliedAmt = 0;
            payment.UnappliedAmtSpecified = true;
            payment.TotalAmt = model.TotalAmt;
            payment.TotalAmtSpecified = true;
            payment.ProcessPayment = false;
            payment.ProcessPaymentSpecified = true;


            Account ARAccount = Helper.FindOrAddAccount(context, AccountTypeEnum.AccountsReceivable, AccountClassificationEnum.Asset);
            payment.ARAccountRef = new ReferenceType()
            {
                name = ARAccount.Name,
                Value = ARAccount.Id
            };

            Account depositAccount = Helper.FindOrAddAccount(context, AccountTypeEnum.Bank, AccountClassificationEnum.Asset);
            payment.DepositToAccountRef = new ReferenceType()
            {
                name = depositAccount.Name,
                Value = depositAccount.Id
            };
            PaymentMethod paymentMethod = Helper.FindOrAdd<PaymentMethod>(context, new PaymentMethod());
            payment.PaymentMethodRef = new ReferenceType()
            {
                name = paymentMethod.Name,
                Value = paymentMethod.Id
            };

            payment.PaymentType = PaymentTypeEnum.CreditCard;
            payment.CurrencyRef = new ReferenceType { name = "US Dollar", Value = "USD" };
            payment.Line = new Line[] { new Line
               {
                              Amount = model.TotalAmt,
                              AmountSpecified = true,
                              DetailType = LineDetailTypeEnum.PaymentLineDetail,
                              DetailTypeSpecified = true,
                              LinkedTxn = new LinkedTxn[] { new LinkedTxn { TxnId = model.QboInvoiceId, TxnType = "Invoice" } } }
               };

            payment.CustomerRef = new ReferenceType { Value = model.QboId, name = model.CustomerName };

            return payment;
        }


        internal static Item CreateNewItem(ServiceContext context, QboBaseViewModel model)
        {
            //ExchangeRate exchangeRate = GetExchangeRate(context, DateTime.Now);
            Item item = new Item();

            item.Type = ItemTypeEnum.NonInventory;
            item.TypeSpecified = true;

            item.Active = true;
            item.ActiveSpecified = true;


            item.Taxable = false;
            item.TaxableSpecified = true;


            item.UnitPrice = model.UnitPrice;
            item.UnitPriceSpecified = true;
            item.PurchaseCost = model.Total;
            item.PurchaseCostSpecified = true;

            item.Name = model.ProductName;
            item.Description = model.ProductDescription;
            item.FullyQualifiedName = model.ProductName;

            item.TrackQtyOnHand = false;

            //Account incomeAccount = Helper.FindOrAddAccount(context, AccountTypeEnum.Income, AccountClassificationEnum.Revenue);
            string categoryName = Enum.GetName(typeof(MATERIALS_TYPE), model.ProductCategory);
            Account incomeAccount = FindAccount(context, AccountTypeEnum.Income, AccountClassificationEnum.Revenue, categoryName);
            if (incomeAccount == null)
            {
                incomeAccount = CreateAccount(context, AccountTypeEnum.Income, AccountClassificationEnum.Revenue, categoryName);
                incomeAccount = Helper.Add<Account>(context, incomeAccount);
            }
            item.IncomeAccountRef = new ReferenceType()
            {
                name = incomeAccount.Name,
                Value = incomeAccount.Id
            };


            Account expenseAccount = FindAccount(context, AccountTypeEnum.CostofGoodsSold, AccountClassificationEnum.Expense, categoryName);
            if (expenseAccount == null)
            {
                expenseAccount = CreateAccount(context, AccountTypeEnum.CostofGoodsSold, AccountClassificationEnum.Expense, categoryName);
                expenseAccount = Helper.Add<Account>(context, expenseAccount);
            }
            item.ExpenseAccountRef = new ReferenceType()
            {
                name = expenseAccount.Name,
                Value = expenseAccount.Id
            };

            try
            {
                item = Helper.Add<Item>(context, item);
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Fatal error on creating a new Item in QBO, Exception details: {0}", ex.RetrieveErrorMessage()));
            }
        }



        internal static async System.Threading.Tasks.Task<Bill> CreateBill(ServiceContext context, BillViewModel model, ILogger log = null)
        {
            QueryService<Vendor> vendorQueryService = new QueryService<Vendor>(context);
            Vendor vendor = null;
            string query = string.Empty;
            if (model.QboId != null)
            {
                query = $"select * from Vendor where Id = '{model.QboId}'";
                vendor = vendorQueryService.ExecuteIdsQuery(query).FirstOrDefault();
            }
            else
            {
                query = $"select * from Vendor where DisplayName = '{model.VendorName}'";
                vendor = vendorQueryService.ExecuteIdsQuery(query).FirstOrDefault();
            }
            if (vendor == null)
            {
                throw new Exception("Could not find vendor in QBO");
            }

            QueryService<Customer> customerQueryService = new QueryService<Customer>(context);
            Customer customer = null;
            if (model.CustomerQboId != null)
            {
                query = $"select * from Customer where Id = '{model.CustomerQboId}'";
                customer = customerQueryService.ExecuteIdsQuery(query).FirstOrDefault();
            }
            else
            {
                query = $"select * from Customer where DisplayName = '{model.CustomerName}'";
                customer = customerQueryService.ExecuteIdsQuery(query).FirstOrDefault();
            }

            if (customer == null)
            {
                throw new Exception("Could not find customer in QBO");
            }

            QueryService<Item> itemQueryService = new QueryService<Item>(context);
            query = $"select * from Item where Name = '{model.ProductName}'";
            Item item = itemQueryService.ExecuteIdsQuery(query).FirstOrDefault<Item>();
            if (item == null)
            {
                item = CreateNewItem(context, model);
            }

            Bill bill = new Bill();

            bill.DocNumber = model.BillNumber;

            bill.TotalAmt = model.Total; // model.UnitPrice * model.Quantity + model.ToolingCharges;

            // GST
            if (vendor.ShipAddr == null)
            {
                vendor.ShipAddr = vendor.BillAddr;
            }

            TaxCode taxcode = GetTaxCode(context, vendor.ShipAddr.Country, vendor.ShipAddr.CountrySubDivisionCode);
            if (taxcode != null && taxcode.SalesTaxRateList != null)
            {
                TaxRate taxRateToFind = new TaxRate();
                taxRateToFind.Id = taxcode.SalesTaxRateList.TaxRateDetail[0].TaxRateRef.Value;
                TaxRate taxRate = Helper.FindById<TaxRate>(context, taxRateToFind);
                model.SalesTax = bill.TotalAmt * (taxRate.RateValue / 100);
                bill.TotalAmt += model.SalesTax;
            }

            // Add your customized item name as a CustomField
            var unitOfMeasurement = Enum.GetName(typeof(MEASUREMENT_UNITS), model.UnitOfMeasurement);

            List<Line> lineList = new List<Line>();
            Line line = new Line();
            if (model.Quantity > model.NumberSampleIncluded || model.UnitPrice > 0)
            {
                // Item details               
                if (string.IsNullOrEmpty(model.ProductDescription) == false)
                {
                    line.Description = $"{model.PartNumber} / {model.PartRevision} (Part # / Part Revision), Qty Unit: {unitOfMeasurement}";
                }
                else
                {
                    line.Description = string.Format("Part Id: {0}, Part No: {1}, Rev: {2}",
                                                      model.ProductId, model.PartNumber, model.PartRevision);
                }
                line.DetailType = LineDetailTypeEnum.ItemBasedExpenseLineDetail;
                line.DetailTypeSpecified = true;
                line.Amount = model.Total;
                line.AmountSpecified = true;

                ItemBasedExpenseLineDetail detail = new ItemBasedExpenseLineDetail();

                detail.ItemRef = new ReferenceType
                {
                    name = item.Name,
                    Value = item.Id,
                };

                detail.AnyIntuitObject = model.UnitPrice;
                detail.ItemElementName = ItemChoiceType.UnitPrice;
                detail.BillableStatus = BillableStatusEnum.NotBillable;
                detail.BillableStatusSpecified = true;
                detail.CustomerRef = new ReferenceType
                {
                    name = customer.DisplayName,
                    Value = customer.Id,
                };
                detail.Qty = model.Quantity;
                detail.QtySpecified = true;
                detail.TaxCodeRef = taxcode != null ? new ReferenceType() { name = taxcode.Name, Value = taxcode.Id } : null;

                line.AnyIntuitObject = detail;

            }
            else
            {
                line.Description = "Tooling Setup Charges";

                line.DetailType = LineDetailTypeEnum.ItemBasedExpenseLineDetail;
                line.DetailTypeSpecified = true;
                line.AnyIntuitObject = new ItemBasedExpenseLineDetail()
                {
                    CustomerRef = new ReferenceType()
                    {
                        name = customer.CompanyName,
                        Value = customer.Id,
                    },
                    ItemRef = new ReferenceType() { name = item.Name, Value = item.Id },
                    Qty = model.NumberSampleIncluded,
                    QtySpecified = true,
                };

                line.Amount = model.Total;
                line.AmountSpecified = true;
            }
            lineList.Add(line);
            bill.Line = lineList.ToArray();



            var term = FindQboTerm(context, model.Term);
            bill.SalesTermRef = new ReferenceType
            {
                name = term == null ? "Due on receipt" : term.Name,
                Value = term == null ? "1" : term.Id,
            };


            bill.DueDate = model.DueDate.Date;
            bill.DueDateSpecified = true;
            bill.VendorRef = new ReferenceType()
            {
                name = vendor.DisplayName,
                Value = vendor.Id,
            };

            ExchangeRate exchangeRate = await GetExchangeRate(context, DateTime.Now, vendor.CurrencyRef.Value, log);

            bill.ExchangeRate = exchangeRate.Rate;
            bill.ExchangeRateSpecified = true;


            bill.CurrencyRef = vendor.CurrencyRef;

            return bill;
        }

        internal static Class CreateClass(ServiceContext context, string name)
        {
            Class class1 = new Class();
            class1.Name = name;
            class1.SubClass = true;
            class1.SubClassSpecified = true;

            class1.FullyQualifiedName = class1.Name;
            class1.Active = true;
            class1.ActiveSpecified = true;

            return class1;
        }

        internal static Class UpdateClass(ServiceContext context, Class entity, string newName)
        {
            entity.Name = newName;
            entity.FullyQualifiedName = entity.Name;
            return entity;
        }

        internal static Attachable CreateAttachable(ServiceContext context, BillViewModel model)
        {
            Attachable attachable = new Attachable();

            attachable.Note = model.AttachFileName;
            attachable.Tag = model.AttachFileName;
            attachable.FileName = model.AttachFileName;
            attachable.FileAccessUri = model.AttachFileAccessUri;
            attachable.TempDownloadUri = model.AttachTempDownloadUri;
            attachable.ContentType = model.AttachFileContentType;
            attachable.Size = model.AttachFileSize;
            attachable.SizeSpecified = true;

            AttachableRef attachableRef = new AttachableRef
            {
                EntityRef = new ReferenceType
                {
                    name = objectNameEnumType.Bill.ToString(),
                    Value = model.AttachedReferenceId,
                    type = objectNameEnumType.Bill.ToString(),
                },
                IncludeOnSend = true
            };

            List<AttachableRef> list = new List<AttachableRef>();
            list.Add(attachableRef);
            attachable.AttachableRef = list.ToArray();

            return attachable;
        }



        internal static Report GetReport(ServiceContext context, string reportName)
        {
            ReportService reportService = new ReportService(context);

            reportService.start_date = DateTime.Now.AddMonths(-12).ToString("yyyy-MM-dd");
            reportService.end_date = DateTime.Now.ToString("yyyy-MM-dd");
            reportService.accounting_method = "Accrual";

            //reportService.date_macro = "This Fiscal Year-to-date";
            //reportService.summarize_column_by = "Month";
            //Check query parameters section of each report for querying on specific cols
            //List<String> columndata = new List<String>();
            //columndata.Add("tx_date");
            //columndata.Add("tx_type");//The value odf this col has the txnId
            //string coldata = String.Join(",", columndata);
            //reportService.columns = coldata;

            Report report = reportService.ExecuteReport(reportName);

            return report;
        }

        internal static Invoice GetInvoice(ServiceContext context, string invoiceId)
        {
            QueryService<Invoice> invoiceQueryservice = new QueryService<Invoice>(context);
            try
            {
                string query = $"select * from Invoice where Id = '{invoiceId}'";
                return invoiceQueryservice.ExecuteIdsQuery(query).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal static Bill GetBill(ServiceContext context, string billId)
        {
            QueryService<Bill> billQueryservice = new QueryService<Bill>(context);
            try
            {
                string query = $"select * from Bill where Id = '{billId}'";
                return billQueryservice.ExecuteIdsQuery(query).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        internal static Payment GetPayment(ServiceContext context, string paymentId)
        {
            QueryService<Payment> paymentQueryservice = new QueryService<Payment>(context);
            string query = $"select * from Payment where Id = '{paymentId}'";
            Payment payment = paymentQueryservice.ExecuteIdsQuery(query).FirstOrDefault();
            return payment;
        }

        internal static PaymentMethod GetPaymentMethod(ServiceContext context, string paymentMethodId)
        {
            QueryService<PaymentMethod> paymentMethodQueryservice = new QueryService<PaymentMethod>(context);
            string query = $"select * from PaymentMethod where Id = '{paymentMethodId}'";
            PaymentMethod paymentMethod = paymentMethodQueryservice.ExecuteIdsQuery(query)?.FirstOrDefault();
            return paymentMethod;
        }

        internal static BillPayment GetBillPayment(ServiceContext context, string billPaymentId)
        {
            QueryService<BillPayment> billPaymentQueryservice = new QueryService<BillPayment>(context);
            string query = $"select * from BillPayment where Id = '{billPaymentId}'";
            BillPayment billPayment = billPaymentQueryservice.ExecuteIdsQuery(query).FirstOrDefault();
            return billPayment;
        }

        internal static List<string> FindQboCustomerEmails(ServiceContext context, string custId)
        {
            QueryService<Customer> customerQueryService = new QueryService<Customer>(context);

            List<string> emails = null;
            string query = $"select * from Customer where Id = '{custId}'";
            var customer = customerQueryService.ExecuteIdsQuery(query).FirstOrDefault();
            if (customer != null && customer.PrimaryEmailAddr != null)
            {
                emails = customer.PrimaryEmailAddr.Address.Split(',').ToList();
            }
            return emails;
        }

        internal static Term FindQboTerm(ServiceContext context, int? Id)
        {
            QueryService<Term> termService = new QueryService<Term>(context);
            string query = string.Empty;
            if (Id == null)
            {
                query = $"select * from Term where Id = '10'";  // id='10' mapping to name = 'Advance'
            }
            else
            {
                var termId = DicTerms[Id.Value];
                query = $"select * from Term where Id = '{termId}'";
            }

            return termService.ExecuteIdsQuery(query).FirstOrDefault();
        }

#if false

        internal static List<Invoice> GetCustomerInvoices(ServiceContext context, string customerName)
        {

            var service = new DataService(context);
            var tQuery = service.FindAll(new Invoice()).Where(q => q.CustomerRef.name == customerName).ToList();
            return tQuery;

        }

        internal static List<PurchaseOrder> GetCustomerPurchaseOrders(ServiceContext context, string displayname)
        {
            var service = new DataService(context);
            var tQuery = service.FindAll(new PurchaseOrder()).Where(q => q.VendorRef.name == displayname).ToList();
            return tQuery;

        }

        internal static decimal UpdateVendorBalance(ServiceContext context, string displayname, decimal balance)
        {
            QueryService<Vendor> vendorQueryService = new QueryService<Vendor>(context);
            var vendor = vendorQueryService.Where(x => x.DisplayName == displayname).FirstOrDefault();
            vendor.Balance += balance;
            Vendor changed = QBOHelper.UpdateVendor(context, vendor);

            //Update the returned entity data
            Vendor updated = Helper.Update<Vendor>(context, changed);

            return updated.Balance;
        }

        internal static decimal UpdateCustomerBalance(ServiceContext context, string displayname, decimal balance)
        {
            QueryService<Customer> customerQueryService = new QueryService<Customer>(context);

            var customer = customerQueryService.Where(x => x.DisplayName == displayname).FirstOrDefault();
            customer.Balance -= balance;

            Customer changed = QBOHelper.UpdateCustomer(context, customer);

            //Update the returned entity data
            Customer updated = Helper.Update<Customer>(context, changed);

            return updated.Balance;

        }


        internal static BillPayment CreateBillPaymentCheck(ServiceContext context, PaymentViewModel model)
        {
            if (model.BillId == null)
                return null;
            BillPayment billPayment = new BillPayment();

            billPayment.TxnDate = DateTime.UtcNow.Date;
            billPayment.TxnDateSpecified = true;

            billPayment.PrivateNote = "PrivateNote";

            QueryService<Vendor> vendorQueryService = new QueryService<Vendor>(context);
            Vendor vendor = null;

            if (model.CompanyId != null)
            {
                vendor = vendorQueryService.Where(x => x.Id == model.CompanyId).FirstOrDefault();
            }
            else
            {
                vendor = vendorQueryService.Where(x => x.DisplayName == model.VendorName).FirstOrDefault();
            }


            var vendorRef = new ReferenceType()
            {
                name = vendor.DisplayName,
                type = "Vendor",
                Value = vendor.Id
            };

            billPayment.VendorRef = vendorRef;
            QueryService<VendorCredit> vendorCreditService = new QueryService<VendorCredit>(context);


            billPayment.PayType = BillPaymentTypeEnum.Check;
            billPayment.PayTypeSpecified = true;

            decimal total = model.Quantity * model.UnitPrice + model.ToolingCharges + model.SalesTax;

            //VendorCredit vendorCredit = vendorCreditService.Where(x => x.VendorRef.Value == vendorRef.Value).FirstOrDefault(); //Helper.Add(context, QBOHelper.CreateVendorCredit(context));
            billPayment.TotalAmt = total;
            billPayment.TotalAmtSpecified = true;

            Account bankAccount = Helper.FindOrAddAccount(context, AccountTypeEnum.Bank, AccountClassificationEnum.Asset);
            BillPaymentCheck billPaymentCheck = new BillPaymentCheck();
            billPaymentCheck.BankAccountRef = new ReferenceType()
            {
                name = bankAccount.Name,
                Value = bankAccount.Id
            };

            CheckPayment checkPayment = new CheckPayment();
            checkPayment.AcctNum = "AcctNum" + Helper.GetGuid().Substring(0, 5);
            checkPayment.BankName = "BankName" + Helper.GetGuid().Substring(0, 5);
            checkPayment.CheckNum = "CheckNum" + Helper.GetGuid().Substring(0, 5);
            checkPayment.NameOnAcct = "Name" + Helper.GetGuid().Substring(0, 5);
            checkPayment.Status = "Status" + Helper.GetGuid().Substring(0, 5);
            billPaymentCheck.CheckDetail = checkPayment;

            PhysicalAddress payeeAddr = new PhysicalAddress();
            payeeAddr.Line1 = model.AddressLine1;
            payeeAddr.Line2 = model.AddressLine2;
            payeeAddr.City = model.City;
            payeeAddr.CountrySubDivisionCode = model.State;
            payeeAddr.PostalCode = model.PostalCode;
            billPaymentCheck.PayeeAddr = payeeAddr;
            billPaymentCheck.PrintStatus = PrintStatusEnum.NeedToPrint;
            billPaymentCheck.PrintStatusSpecified = true;
            billPayment.AnyIntuitObject = billPaymentCheck;

            List<Line> lineList = new List<Line>();

            Line line1 = new Line();

            QueryService<Bill> billQueryservice = new QueryService<Bill>(context);
            Bill bill = billQueryservice.Where(x => x.Id == model.BillId).FirstOrDefault();
            if (bill == null)
                return null;

            line1.Amount = bill.TotalAmt;
            line1.AmountSpecified = true;
            List<LinkedTxn> LinkedTxnList1 = new List<LinkedTxn>();
            LinkedTxn linkedTxn1 = new LinkedTxn();
            linkedTxn1.TxnId = bill.Id;
            linkedTxn1.TxnType = TxnTypeEnum.Bill.ToString();
            LinkedTxnList1.Add(linkedTxn1);
            line1.LinkedTxn = LinkedTxnList1.ToArray();

            lineList.Add(line1);

            //Line line = new Line();


            //line.Amount = total; // vendorCredit.TotalAmt;
            //line.AmountSpecified = true;

            List<LinkedTxn> LinkedTxnList = new List<LinkedTxn>();
            //LinkedTxn linkedTxn = new LinkedTxn();
            //linkedTxn.TxnId = vendorCredit.Id;
            //linkedTxn.TxnType = TxnTypeEnum.VendorCredit.ToString();
            //LinkedTxnList.Add(linkedTxn);
            //line.LinkedTxn = LinkedTxnList.ToArray();

            //lineList.Add(line);

            billPayment.Line = lineList.ToArray();

            return billPayment;
        }

        internal static BillPayment CreateBillPaymentCreditCard(ServiceContext context)
        {
            BillPayment billPayment = new BillPayment();
            VendorCredit vendorCredit = Helper.Add(context, QBOHelper.CreateVendorCredit(context));
            billPayment.PayType = BillPaymentTypeEnum.Check;
            billPayment.PayTypeSpecified = true;

            billPayment.TotalAmt = vendorCredit.TotalAmt;
            billPayment.TotalAmtSpecified = true;

            billPayment.TxnDate = DateTime.UtcNow.Date;
            billPayment.TxnDateSpecified = true;

            billPayment.PrivateNote = "This is my PrivateNote";

            Vendor vendor = Helper.FindOrAdd<Vendor>(context, new Vendor());
            billPayment.VendorRef = new ReferenceType()
            {
                name = vendor.DisplayName,
                type = "Vendor",
                Value = vendor.Id
            };

            Account bankAccount = Helper.FindOrAddAccount(context, AccountTypeEnum.CreditCard, AccountClassificationEnum.Expense);
            BillPaymentCreditCard billPaymentCreditCard = new BillPaymentCreditCard();
            billPaymentCreditCard.CCAccountRef = new ReferenceType()
            {
                name = bankAccount.Name,
                Value = bankAccount.Id
            };

            CreditCardPayment creditCardPayment = new CreditCardPayment();
            creditCardPayment.CreditChargeInfo = new CreditChargeInfo()
            {
                Amount = new Decimal(10.00),
                AmountSpecified = true,
                Number = "124124124",
                NameOnAcct = bankAccount.Name,
                CcExpiryMonth = 10,
                CcExpiryMonthSpecified = true,
                CcExpiryYear = 2015,
                CcExpiryYearSpecified = true,
                BillAddrStreet = "BillAddrStreetba7cca47",
                PostalCode = "560045",
                CommercialCardCode = "CardCodeba7cca47",
                CCTxnMode = CCTxnModeEnum.CardPresent,
                CCTxnType = CCTxnTypeEnum.Charge
            };

            billPaymentCreditCard.CCDetail = creditCardPayment;
            billPayment.AnyIntuitObject = billPaymentCreditCard;

            List<Line> lineList = new List<Line>();

            Line line1 = new Line();

            Bill bill = Helper.Add<Bill>(context, QBOHelper.CreateBill(context));
            line1.Amount = bill.TotalAmt;
            line1.AmountSpecified = true;
            List<LinkedTxn> LinkedTxnList1 = new List<LinkedTxn>();
            LinkedTxn linkedTxn1 = new LinkedTxn();
            linkedTxn1.TxnId = bill.Id;
            linkedTxn1.TxnType = TxnTypeEnum.Bill.ToString();
            LinkedTxnList1.Add(linkedTxn1);
            line1.LinkedTxn = LinkedTxnList1.ToArray();

            lineList.Add(line1);

            Line line = new Line();


            line.Amount = vendorCredit.TotalAmt;
            line.AmountSpecified = true;

            List<LinkedTxn> LinkedTxnList = new List<LinkedTxn>();
            LinkedTxn linkedTxn = new LinkedTxn();
            linkedTxn.TxnId = vendorCredit.Id;
            linkedTxn.TxnType = TxnTypeEnum.VendorCredit.ToString();
            LinkedTxnList.Add(linkedTxn);
            line.LinkedTxn = LinkedTxnList.ToArray();

            lineList.Add(line);

            billPayment.Line = lineList.ToArray();

            return billPayment;
        }

        internal static Payment CreatePayment(ServiceContext context, PaymentViewModel model)
        {
            Payment payment = new Payment();
            payment.TxnDate = DateTime.UtcNow.Date;
            payment.TxnDateSpecified = true;
            decimal total = model.Quantity * model.UnitPrice + model.ToolingCharges + model.SalesTax;

            Account ARAccount = Helper.FindOrAddAccount(context, AccountTypeEnum.AccountsReceivable, AccountClassificationEnum.Asset);
            payment.ARAccountRef = new ReferenceType()
            {
                name = ARAccount.Name,
                Value = ARAccount.Id
            };

            Account depositAccount = Helper.FindOrAddAccount(context, AccountTypeEnum.Bank, AccountClassificationEnum.Asset);
            payment.DepositToAccountRef = new ReferenceType()
            {
                name = depositAccount.Name,
                Value = depositAccount.Id
            };
            PaymentMethod paymentMethod = Helper.FindOrAdd<PaymentMethod>(context, new PaymentMethod());
            payment.PaymentMethodRef = new ReferenceType()
            {
                name = paymentMethod.Name,
                Value = paymentMethod.Id
            };
            QueryService<Customer> customerQueryService = new QueryService<Customer>(context);
            QueryService<Vendor> vendorQueryService = new QueryService<Vendor>(context);
            Customer customer = null;
            Vendor vendor = null;

            if (model.UserType == USER_TYPE.Customer)
            {
                if (model.CompanyId != null)
                {
                    customer = customerQueryService.Where(x => x.Id == model.CompanyId).FirstOrDefault();
                }
                else
                {
                    customer = customerQueryService.Where(x => x.DisplayName == model.CustomerName).FirstOrDefault();
                }

                payment.CustomerRef = new ReferenceType()
                {
                    name = customer.DisplayName,
                    Value = customer.Id
                };
            }
            else if (model.UserType == USER_TYPE.Vendor)
            {
                if (model.CustomerId != null)
                {
                    vendor = vendorQueryService.Where(x => x.Id == model.CompanyId).FirstOrDefault();
                }
                else
                {
                    vendor = vendorQueryService.Where(x => x.DisplayName == model.VendorName).FirstOrDefault();
                }

                payment.CustomerRef = new ReferenceType()
                {
                    name = vendor.DisplayName,
                    Value = vendor.Id
                };
            }



            payment.PaymentType = PaymentTypeEnum.Check;
            CheckPayment checkPayment = new CheckPayment();
            checkPayment.AcctNum = "Acctnum" + Helper.GetGuid().Substring(0, 5);
            checkPayment.BankName = "BankName" + Helper.GetGuid().Substring(0, 5);
            checkPayment.CheckNum = "CheckNum" + Helper.GetGuid().Substring(0, 5);
            checkPayment.NameOnAcct = "Name" + Helper.GetGuid().Substring(0, 5);
            checkPayment.Status = "Status" + Helper.GetGuid().Substring(0, 5);
            checkPayment.CheckPaymentEx = new IntuitAnyType();
            payment.AnyIntuitObject = checkPayment;

            payment.TotalAmt = total;
            payment.TotalAmtSpecified = true;
            //payment.UnappliedAmt = new Decimal(100.00);
            //payment.UnappliedAmtSpecified = true;

            List<LinkedTxn> LinkedTxnList = new List<LinkedTxn>();
            LinkedTxn linkedTxn = new LinkedTxn();
            linkedTxn.TxnId = model.InvoiceId;
            linkedTxn.TxnType = TxnTypeEnum.Invoice.ToString();
            LinkedTxnList.Add(linkedTxn);
            payment.LinkedTxn = LinkedTxnList.ToArray();

            return payment;
        }
#endif


    }
}
