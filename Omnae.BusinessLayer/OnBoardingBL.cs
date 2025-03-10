using JetBrains.Annotations;
using Microsoft.Azure;
using Omnae.BusinessLayer.Identity;
using Omnae.BusinessLayer.Identity.Model;
using Omnae.BusinessLayer.Interface;
using Omnae.BusinessLayer.Util;
using Omnae.Common;
using Omnae.Common.Extensions;
using Omnae.Data;
using Omnae.Libs;
using Omnae.Libs.ViewModels;
using Omnae.Model.Context;
using Omnae.Model.Models;
using Omnae.Service.Service.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Omnae.BusinessLayer
{
    public class OnBoardingBL
    {
        private readonly IHomeBL HomeBL;
        private readonly TaskDatasBL TaskDatasBL;
        private readonly ChartBL chartBl;
        private readonly DocumentBL documentBL;
        private readonly ICompaniesCreditRelationshipService companiesCreditRelationshipService;

        private ICompanyService CompanyService { get; }
        private ITaskDataService TaskDataService { get; }
        private IPriceBreakService PriceBreakService { get; }
        private ILogedUserContext UserContext { get; }
        private IProductService ProductService { get; }
        private IShippingService ShippingService { get; }
        private ICountryService CountryService { get; }
        private IAddressService AddressService { get; }
        private IStateProvinceService StateProvinceService { get; }
        private IPartRevisionService PartRevisionService { get; }
        private IRFQQuantityService RfqQuantityService { get; }

        private IAuthZeroService AuthZeroService { get; }
        private ILogger Log { get; }

        private readonly ApplicationDbContext dbUser;
        private readonly ApplicationUserManager userManager;

        private readonly HttpClient sendInvitationHttpClient = new HttpClient();

        private static readonly int MaxCompaniesToOnboard = CloudConfigurationManager.GetSetting("OnBoarding.MaxCompaniesPerFile")?.ToInt() ?? 500;
        private static readonly int MaxPartsToOnboard = CloudConfigurationManager.GetSetting("OnBoarding.MaxPartsPerFile")?.ToInt() ?? 250;
        private readonly IProductPriceQuoteService productPriceQuoteService;

        public OnBoardingBL(IHomeBL homeBL, TaskDatasBL taskDatasBL, ChartBL chartBl, DocumentBL documentBL, ICompaniesCreditRelationshipService companiesCreditRelationshipService, ApplicationDbContext dbUser, ApplicationUserManager userManager, ICompanyService companyService, ITaskDataService taskDataService, IPriceBreakService priceBreakService, ILogedUserContext userContext, IProductService productService, IShippingService shippingService, ICountryService countryService, IAddressService addressService, IStateProvinceService stateProvinceService, IPartRevisionService partRevisionService, IAuthZeroService authZeroService, ILogger log, IRFQQuantityService rfqQuantityService, IProductPriceQuoteService productPriceQuoteService)
        {
            HomeBL = homeBL;
            TaskDatasBL = taskDatasBL;
            this.chartBl = chartBl;
            this.documentBL = documentBL;
            this.companiesCreditRelationshipService = companiesCreditRelationshipService;
            this.dbUser = dbUser;
            this.userManager = userManager;
            CompanyService = companyService;
            TaskDataService = taskDataService;
            PriceBreakService = priceBreakService;
            UserContext = userContext;
            ProductService = productService;
            ShippingService = shippingService;
            CountryService = countryService;
            AddressService = addressService;
            StateProvinceService = stateProvinceService;
            PartRevisionService = partRevisionService;
            AuthZeroService = authZeroService;
            Log = log;
            RfqQuantityService = rfqQuantityService;
            this.productPriceQuoteService = productPriceQuoteService;

            //Configure API
            var newSystemUrlDomain = CloudConfigurationManager.GetSetting("NewUiApiUrlDomain");
            sendInvitationHttpClient.BaseAddress = new Uri(newSystemUrlDomain);
            sendInvitationHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            sendInvitationHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", @"f5f45de7-91c6-4d55-b8fa-393c50b4a84b"); //TODO send to web.config
        }

        public async Task<IList<(int companyId, ApplicationUserOnBoardInfo userToInvite)>> ImportVendorAsync(string inputFilePath,
                                                                                                             bool isNewSystem,
                                                                                                             int? sourceCompanyId = null,
                                                                                                             bool? needCheckPartnerLimits = null)
        {
            if (sourceCompanyId == null)
            {
                sourceCompanyId = UserContext.Company.Id;
            }

            var inputList = ExcelDataParser.GetCompanyDataStream(inputFilePath);
            Log.Information("Found {Qnt} Entries in the Import File. {FileName}", inputList.Count, inputFilePath);

            if (needCheckPartnerLimits == true)
            {
                var partnersLimit = int.Parse(CloudConfigurationManager.GetSetting("PARTNERS_LIMIT_FOR_FREE_PLAN"));
                if (partnersLimit < inputList.Count)
                {
                    Log.Error("Error in OnBoarding:  Invited number of partners of {Count} exceeds the threshold of partner limit of {partnersLimit} for the free plan.", inputList.Count, partnersLimit);
                    throw new ValidationException("Invited number of partners exceeds the threshold of partner limit for the free plan.");
                }
            }

            if (inputList.Count > MaxCompaniesToOnboard)
                throw new ValidationException($"You can only import {MaxCompaniesToOnboard} Companies per file");

            Log.Debug("Validating...");
            foreach (var row in inputList)
            {
                //TODO better Validation Error. Add Line #
                Validator.ValidateObject(row, new ValidationContext(row), true);
            }

            var hasDupCompany = (from row in inputList
                                 group row by new { row.VendorName } into g
                                 where g.Count() > 1
                                 select g.Key).Any();
            if (hasDupCompany)
                throw new ValidationException("Duplicated Company Name found in the OnBoarding file");

            Log.Debug("Valid.");

            var index = 0;
            var result = new List<(int companyId, ApplicationUserOnBoardInfo userToInvite)>();
            foreach (var row in inputList)
            {
                Log.Verbose("Processing the Line {Line} of the OnBoarding file {Filename}. {@data}", inputFilePath, index, row);
                index++;

                if (string.IsNullOrWhiteSpace(row.VendorName)) //Invalid entry
                    break;

                FixCountry(row);
                Country country = CountryService.FindCountryByCountryCodeOrName(row.Country);
                if (country == null)
                {
                    Log.Warning("Invalid Country. Country:{CountryName}", row.Country);
                    throw new ValidationException($"Invalid Country. Country:{row.Country}");
                }

                //TODO: Do a better check to indentify that is the Same Company.
                var company = await CompanyService.FindAllCompanies(onlyActive: false)
                                                  .FirstOrDefaultAsync(c => c.Name.Equals(row.VendorName, StringComparison.CurrentCultureIgnoreCase));

                var discountDays = string.IsNullOrWhiteSpace(row.EarlyPaymentDiscountDays) == false
                                         ? int.Parse(row.EarlyPaymentDiscountDays)
                                         : (int?)null;
                var discount = string.IsNullOrWhiteSpace(row.EarlyPaymentDiscountPercentage) == false
                             ? int.Parse(row.EarlyPaymentDiscountPercentage)
                             : (int?)null;
                var deposit = string.IsNullOrWhiteSpace(row.DepositePercentage) == false
                            ? int.Parse(row.DepositePercentage)
                            : (int?)null;
                var toolingDeposit = string.IsNullOrWhiteSpace(row.ToolingDepositePercentage) == false
                                   ? int.Parse(row.ToolingDepositePercentage)
                                   : (int?)null;

                int? termDays = string.IsNullOrWhiteSpace(row.TermDays) == false ? int.Parse(row.TermDays) : (int?)null;
                decimal? creditLimit = string.IsNullOrWhiteSpace(row.CreditLimit) == false ? decimal.Parse(row.CreditLimit) : (decimal?)null;
                CurrencyCodes currency = CurrencyCodes.USD;

                if (company != null)//Company Exits
                {
                    Log.Warning("The company with name: {VendorName} exists in the database and is ignored by the importer.", row.VendorName);

                    var isToResendInvitations = (true); //TODO, Find a way to know if the invitation was accepect to check if we need to resend or not a invite.
                    if (isToResendInvitations)
                    {
                        var userToResend = GetUser(row, company.Id, country);
                        result.Add((company.Id, userToResend));
                    }
                    // check CompaniesCreditRelationship table to see if this customer has relation with this vendor
                    var relation = companiesCreditRelationshipService.FindCompaniesCreditRelationshipByCustomerIdVendorId((int)sourceCompanyId, company.Id);
                    if (relation == null)
                    {
                        var creditRelationship = new CompaniesCreditRelationship
                        {
                            CustomerId = (int)sourceCompanyId,
                            VendorId = company.Id,
                            isTerm = row.IsTerm ?? false,
                            TermDays = termDays,
                            CreditLimit = creditLimit,
                            DiscountDays = discountDays,
                            Discount = discount,
                            Deposit = deposit,
                            Currency = currency,
                            ToolingDepositPercentage = toolingDeposit,
                        };
                        companiesCreditRelationshipService.AddCompaniesCreditRelationship(creditRelationship);
                    }
                    continue;
                }

                var address = new Address
                {
                    AddressLine1 = row.AddressLine1,
                    AddressLine2 = row.AddressLine2,
                    StateProvince = (string.IsNullOrWhiteSpace(row.StateOrProvince) ? null : new StateProvince() { Name = row.StateOrProvince }),
                    City = row.City,
                    CountryId = country?.Id ?? 0,
                    ZipCode = row.ZipCode,
                    PostalCode = row.PostalCode,
                    isBilling = true,
                    isShipping = true,
                    isMailingAddress = true,
                    isMainAddress = true,
                };
                AddressService.FixIds(address);
                int addressId = AddressService.AddAddress(address);


                var hasCurrency = Enum.TryParse<CurrencyCodes>(row.Currency, out currency);
                if (hasCurrency == false)
                {
                    currency = CurrencyCodes.USD;
                }


                company = new Company
                {
                    Name = row.VendorName,
                    CompanyType = CompanyType.Vendor, //This is Obsolete.

                    AddressId = addressId,
                    BillAddressId = addressId,
                    MainCompanyAddress_Id = addressId,

                    Term = termDays, //This is Obsolete, The System will use CompaniesCreditRelationship
                    CreditLimit = creditLimit ?? 0m, //This is Obsolete, The System will use CompaniesCreditRelationship

                    isEnterprise = true, //Required to SAAS Model
                    WasOnboarded = true,
                    WasInvited = true,
                    InvitedByCompanyId = sourceCompanyId,
                    OnboardedByCompanyId = sourceCompanyId,
                    Currency = currency,
                };

                var companyId = CompanyService.AddCompany(company);



                //var wasFoundCuur = NMoneys.Currency.TryGet(row.Currency, out var curr);
                //CurrencyCodes currency = (CurrencyCodes)Enum.Parse(typeof(CurrencyCodes), curr.NumericCode.ToString());

                var companiesCreditRelationship = new CompaniesCreditRelationship
                {
                    CustomerId = (int)sourceCompanyId,
                    VendorId = companyId,
                    isTerm = row.IsTerm ?? false,
                    TermDays = termDays,
                    CreditLimit = creditLimit,
                    DiscountDays = discountDays,
                    Discount = discount,
                    Deposit = deposit,
                    Currency = currency,
                    ToolingDepositPercentage = toolingDeposit,
                };
                companiesCreditRelationshipService.AddCompaniesCreditRelationship(companiesCreditRelationship);

                // User
                var user = GetUser(row, companyId, country);

                // Shipping
                var shipping = new Shipping()
                {
                    Attention_FreeText = $"{row.ContactFirstName} {row.ContactLastName}",
                    CompanyId = companyId,
                    AddressId = addressId,
                    Address = address,
                    PhoneNumber = row.Phone,
                    EmailAddress = row.Email
                };

                // Save Shipping to DB
                var shippingId = ShippingService.AddShipping(shipping);

                // Insert shippingId to Companies table
                CompanyService.UpdateCompanyShippingId(companyId, shippingId);

                result.Add((companyId, user));
            }

            return result;
        }

        private static void FixCountry(VendorDataViewModel row)
        {
            row.Country = row.Country?.TrimAll();

            switch (row.Country)
            {
                case "United States":
                case "USA":
                case "U.S.A.":
                    row.Country = "United States of America";
                    break;
            }
        }

        private static ApplicationUserOnBoardInfo GetUser(VendorDataViewModel row, int companyId, Country country)
        {
            var user = new ApplicationUserOnBoardInfo
            {
                FirstName = row.ContactFirstName,
                LastName = row.ContactLastName,
                Email = row.Email,
                UserName = row.Email,
                UserType = USER_TYPE.Vendor,
                Country = country.CountryCode,
                PhoneNumber = row.OriginalPhoneEntry,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                CompanyId = companyId,

                CompanyName = row.VendorName,
                CompanyAddressRegion = row.StateOrProvince,
                CompanyAddressCity = row.City,
                CompanyAddressLine = $"{row.AddressLine1} {row.AddressLine2}".TrimAll(),
                CompanyAddressPostalCode = row.ZipCode?.ToNullIfEmpty() ?? row.PostalCode,
                Currency = row.Currency,
                HasCreditTerms = row.IsTerm ?? false,
            };

            return user;
        }

        //private async Task CreateUser(VendorDataViewModel row, int companyId, bool createInAuth0)
        //{
        //    var user = new ApplicationUser
        //    {
        //        FirstName = row.ContactFirstName,
        //        MiddleName = row.ContactMiddleName,
        //        LastName = row.ContactLastName,
        //        Email = row.Email,
        //        UserName = row.Email,
        //        UserType = USER_TYPE.Vendor,
        //        PhoneNumber = row.Phone,
        //        EmailConfirmed = true,
        //        PhoneNumberConfirmed = true,
        //        CompanyId = companyId,
        //    };
        //    var newPassword = ConfigurationManager.AppSettings["DefaultVendorPassword"];

        //    if (createInAuth0)
        //    {
        //        var newUser = await AuthZeroService.CreateOrGetUser(user, companyId, newPassword);

        //        if (companyId != newUser.CompanyId)
        //            throw new ApplicationException("User exists in another company.");
        //    }
        //    else
        //    {
        //        var result = await userManager.CreateAsync(user, newPassword);
        //        if (!result.Succeeded)
        //            throw new ApplicationException(result.Errors.FirstOrDefault());
        //    }
        //}

        public async Task SendInvitationToTheNewUserViaTheNewSystemAsync(Inviter inviter, ApplicationUserOnBoardInfo newUSerToInvite)
        {
            if (inviter == null) throw new ArgumentNullException(nameof(inviter));
            if (newUSerToInvite == null) throw new ArgumentNullException(nameof(newUSerToInvite));

            var data = new NewFrontEndInvitationNewUserModel
            {
                inviter = inviter,
                invitee = new Invitee()
                {
                    companyId = newUSerToInvite.CompanyId?.ToString(),
                    email = newUSerToInvite.Email,
                    firstName = newUSerToInvite.FirstName,
                    lastName = newUSerToInvite.LastName,
                    phone = new Phone()
                    {
                        country = newUSerToInvite.Country,
                        number = newUSerToInvite.PhoneNumber,
                    },
                    companyName = newUSerToInvite.CompanyName,
                    companyAddress = new CompanyAddress
                    {
                        country = newUSerToInvite.Country,
                        region = newUSerToInvite.CompanyAddressRegion,
                        city = newUSerToInvite.CompanyAddressCity,
                        address = newUSerToInvite.CompanyAddressLine,
                        postalCode = newUSerToInvite.CompanyAddressPostalCode,
                    },
                    currency = newUSerToInvite.Currency,
                    hasCreditTerms = newUSerToInvite.HasCreditTerms,
                }
            };

            var result = await sendInvitationHttpClient.PostAsJsonAsync(@"/api/invitations/companies", data);
            if (!result.IsSuccessStatusCode)
            {
                var body = await result.Content.ReadAsStringAsync();

                if (result.StatusCode == HttpStatusCode.Conflict || result.StatusCode == HttpStatusCode.BadRequest)
                {
                    Log.Warning("The new Front-End API send a BadRequest or a Conflict, this invitation is been ignored. {StatusCode} - {Body}", result.StatusCode, body);
                    return;
                }
                Log.Error("The new Front-End API returned a error when sending the invitations. {StatusCode} - {Body}", result.StatusCode, body);
                result.EnsureSuccessStatusCode();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <param name="sourceCompanyId">The Company that uploaded the File</param>
        /// <returns>hasDupe</returns>
        public async Task<bool> ImportPartsAsync(string inputFilePath, int? sourceCompanyId = null)
        {
            if (sourceCompanyId == null)
            {
                sourceCompanyId = UserContext.Company.Id;
            }

            var inputList = ExcelDataParser.GetProductsStream(inputFilePath).ToList();
            Log.Information("Found {Qnt} Entries in the Import File", inputList.Count);

            if (inputList.Count > MaxPartsToOnboard)
                throw new ValidationException($"You can only import {MaxPartsToOnboard} Parts per file");

            Log.Debug("Validating...");
            foreach (var row in inputList)
            {
                //TODO better Validation Error. Add Line #
                Validator.ValidateObject(row, new ValidationContext(row), true);
            }

            //var hasDupParts = (from row in inputList
            //                   group row by new { ThisProductIsFMadeByAVendor = row.ThisProductIsMadeByAVendor, row.PartNumber, row.PartRevision } into g
            //                   where g.Count() > 1
            //                   select g.Key).Any();
            //if (hasDupParts)
            //{
            //    throw new ValidationException("Duplicated PartNumber found in the Onboarding file");
            //}
            //Log.Debug("Valid.");


            var adminId = dbUser.Users.First(x => x.UserType == USER_TYPE.Admin).Id; //TODO Refactor
            var adminCompany = CompanyService.FindCompanyByUserId(adminId);

            var index = 0;
            bool hasDupe = false;
            foreach (var row in inputList)
            {
                Log.Verbose("Processing the Line {Line} of the OnBoarding file {Filename}. {@data}", inputFilePath, index, row);
                index++;

                var company = CompanyService.FindAllCompanies(onlyActive: false)
                                            .FirstOrDefault(x => x.Name.Equals(row.CompanyName, StringComparison.CurrentCultureIgnoreCase));
                if (company == null)
                {
                    Log.Warning("The company with name: {CompanyName} not exists in the database and is ignored by the importer.", row.CompanyName);
                    continue;
                }

                var theProductVendorId = row.ThisProductIsMadeByAVendor ? company.Id : (int)sourceCompanyId;
                var theProductCustomerId = row.ThisProductIsMadeByAVendor ? (int)sourceCompanyId : company.Id;

                if (row.Quantity1 == "0" || row.Quantity2 == "0" || row.Quantity3 == "0" || row.Quantity4 == "0" || row.Quantity5 == "0" || row.Quantity6 == "0" || row.Quantity7 == "0")
                {
                    Log.Warning("Quantity cannot be 0. Skip this row. Please change the qantity to a proper value and re-run this row later");
                    continue;
                }

                var priceBreaks = GetPriceBreaks(row).OrderBy(p => p.Quantity).ToList();

                //var product = ProductService.FindProductByPartNumberOfACustomer((int) sourceCompanyId, row.PartNumber, row.PartRevision);
                var product = ProductService.FindProductByPartNumber(theProductVendorId, theProductCustomerId, row.PartNumber, row.PartRevision);

                //BUG? When the Part is not from the Company.
                if (product == null)
                {
                    CreateProduct(row, priceBreaks, theProductVendorId, theProductCustomerId, adminId, adminCompany);
                }
                else
                {
                    Log.Warning("Product, Part always exists in for this customer. ProductId:{ProductId} Name:{ProductName}", product.Id, product.Name);
                    hasDupe = true;
                }
            }
            return hasDupe;
        }


        public Product AddExistingProduct(AddExistingProductViewModel vm)
        {
            using (var tran = AsyncTransactionScope.StartNew())
            {
                try
                {
                    VendorProductDataViewModel model = new VendorProductDataViewModel
                    {
                        BuildType = vm.BuildType,
                        Material = vm.Material,
                        PartName = vm.PartName,
                        Description = vm.Description,
                        PartNumber = vm.PartNumber,
                        PartRevision = vm.PartRevision,
                        Quantity1 = vm.Quantity1,
                        UnitPrice1 = vm.UnitPrice1?.ToString(),
                        Quantity2 = vm.Quantity2,
                        UnitPrice2 = vm.UnitPrice2?.ToString(),
                        Quantity3 = vm.Quantity3,
                        UnitPrice3 = vm.UnitPrice3?.ToString(),
                        Quantity4 = vm.Quantity4,
                        UnitPrice4 = vm.UnitPrice4?.ToString(),
                        Quantity5 = vm.Quantity5,
                        UnitPrice5 = vm.UnitPrice5?.ToString(),
                        Quantity6 = vm.Quantity6,
                        UnitPrice6 = vm.UnitPrice6?.ToString(),
                        Quantity7 = vm.Quantity7,
                        UnitPrice7 = vm.UnitPrice7?.ToString(),
                        SampleLeadTime = vm.SampleLeadTime,
                        ProductionLeadTime = vm.ProductionLeadTime,
                        HamonizedCode = vm.HarmonizedCode,
                        UnitOfMeasurement = vm.UnitOfMeasurement,
                    };
                    var priceBreaks = GetPriceBreaks(model).OrderBy(p => p.Quantity).ToList();

                    var product = ProductService.FindProductByPartNumber(vm.VendorId, vm.CustomerId, vm.PartNumber, vm.PartRevision);

                    if (product == null)
                    {
                        var adminId = dbUser.Users.First(x => x.UserType == USER_TYPE.Admin).Id;
                        var adminCompany = CompanyService.FindCompanyByUserId(adminId);
                        int unitOfMeasurement = string.IsNullOrEmpty(vm.UnitOfMeasurement) ? 0 : int.Parse(vm.UnitOfMeasurement);
                        product = CreateProduct(model, priceBreaks, vm.VendorId, vm.CustomerId, adminId, adminCompany, vm.ExpireDate);
                    }
                    else
                    {
                        Log.Warning("Product, Part already exists for this customer. ProductId:{ProductId} Name:{ProductName}", product.Id, product.Name);
                    }

                    // also add 
                    if (companiesCreditRelationshipService.FindCompaniesCreditRelationshipByCustomerIdVendorId(vm.CustomerId, vm.VendorId) == null)
                    {
                        var companiesCreditRelationship = new CompaniesCreditRelationship
                        {
                            CustomerId = vm.CustomerId,
                            VendorId = vm.VendorId,
                            Currency = vm.Currency ?? (product.VendorCompany.Currency > 0 ? product.VendorCompany.Currency : CurrencyCodes.USD),
                            //TermDays = termDays,
                            //CreditLimit = creditLimit,
                            //DiscountDays = discountDays,
                            //Discount = discount,
                            //Deposit = deposit,
                            //ToolingDepositPercentage = toolingDeposit,
                        };
                        companiesCreditRelationshipService.AddCompaniesCreditRelationship(companiesCreditRelationship);
                    }
                    tran.Complete();
                    return product;
                }
                catch (Exception e)
                {
                    Log.Warning(e, "Error In AddExistingProduct");
                    throw;
                }

            }
        }

        private static IEnumerable<(string Quantity, decimal? UnitPrice)> GetPriceBreaks(VendorProductDataViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Quantity1) && model.UnitPrice1 != null)
                yield return (model.Quantity1, (decimal?)double.Parse(model.UnitPrice1));
            if (!string.IsNullOrWhiteSpace(model.Quantity2) && model.UnitPrice2 != null)
                yield return (model.Quantity2, (decimal?)double.Parse(model.UnitPrice2));
            if (!string.IsNullOrWhiteSpace(model.Quantity3) && model.UnitPrice3 != null)
                yield return (model.Quantity3, (decimal?)double.Parse(model.UnitPrice3));
            if (!string.IsNullOrWhiteSpace(model.Quantity4) && model.UnitPrice4 != null)
                yield return (model.Quantity4, (decimal?)double.Parse(model.UnitPrice4));
            if (!string.IsNullOrWhiteSpace(model.Quantity5) && model.UnitPrice5 != null)
                yield return (model.Quantity5, (decimal?)double.Parse(model.UnitPrice5));
            if (!string.IsNullOrWhiteSpace(model.Quantity6) && model.UnitPrice6 != null)
                yield return (model.Quantity6, (decimal?)double.Parse(model.UnitPrice6));
            if (!string.IsNullOrWhiteSpace(model.Quantity7) && model.UnitPrice7 != null)
                yield return (model.Quantity7, (decimal?)double.Parse(model.UnitPrice7));

        }

        private Product CreateProduct(VendorProductDataViewModel row, IEnumerable<(string Quantity, decimal? UnitPrice)> priceBreaks,
                                      int vendorId, int companyId, string adminId, Company adminCompany, DateTime? expireDate = null )
        {
            int daysToExpire = Convert.ToInt32(ConfigurationManager.AppSettings["DaysToExpire"]);
            DateTime pastDays = DateTime.UtcNow.AddDays(-daysToExpire);
            int? productPriceQuoteId = null;

            // Refer [CompaniesCreditRelationships] table
            var relationship = companiesCreditRelationshipService.FindCompaniesCreditRelationshipByCustomerIdVendorId(companyId, vendorId);

            // Product
            var vendor = CompanyService.FindCompanyById(vendorId);
            var product = new Product
            {
                PartNumber = row.PartNumber,
                PartNumberRevision = row.PartRevision,
                AdminId = adminId,
                CustomerId = companyId,
                VendorId = vendorId,
                Name = row.PartName,
                Description = row.Description ?? "-",
                CreatedDate = pastDays,
                SampleLeadTime = row.SampleLeadTime,
                ProductionLeadTime = row.ProductionLeadTime,
                HarmonizedCode = row.HamonizedCode,
                Material = row.Material,
                BuildType = row.BuildType,
                WasOnboarded = true,
                PreferredCurrency = relationship?.Currency ??  vendor.Currency, 
            };
            int productId = 0;

            try
            {
                productId = ProductService.AddProduct(product);
                product = ProductService.FindProductById(productId);

                // Create a new entry in ProductPriceQuote table for this priceBreak
                ProductPriceQuote q = new ProductPriceQuote
                {
                    VendorId = product.VendorId.Value,
                    ProductId = productId,
                    ProductionLeadTime = product.ProductionLeadTime != null ? product.ProductionLeadTime.Value : 0,
                    ExpireDate = expireDate ?? DateTime.UtcNow.AddMonths(12), // default is set to 1 year
                    IsActive = true,

                    // ToDo: presuming quote doc is single, if it is multiple, here only choose the last one.
                    //QuoteDocUri = documentBL.DocumentService.FindDocumentByProductIdDocType(productId, DOCUMENT_TYPE.QUOTE_PDF).LastOrDefault().DocUri,
                    //CreatedAt = DateTime.UtcNow,
                };
                productPriceQuoteId = productPriceQuoteService.AddProductPriceQuote(q);

            }
            catch (DbEntityValidationException ex)
            {
                throw new ApplicationException("Add Product failed: " + ex.RetrieveDbEntityValidationException());
            }

            // TaskData
            var taskData = new TaskData()
            {
                StateId = (int)States.ProductionComplete,
                ProductId = productId,
                CreatedUtc = pastDays,
                ModifiedUtc = pastDays,
                UpdatedBy = UserContext.User?.UserName,
                CreatedByUserId = UserContext.UserId,
                ModifiedByUserId = UserContext.UserId,
                IsRiskBuild = false,
                isEnterprise = true,
            };

            int taskId = 0;
            try
            {
                taskId = TaskDataService.AddTaskData(taskData);
            }
            catch (DbEntityValidationException ex)
            {
                throw new ApplicationException("Add TaskData failed: " + ex.RetrieveDbEntityValidationException());
            }

            // Create PartRevision table
            var partRevision = new PartRevision()
            {
                OriginProductId = productId,
                StateId = (States)taskData.StateId,
                TaskId = taskId,
                Name = row.PartRevision,
                Description = "Initial Part Revision",
                CreatedBy = adminCompany.Name, //TODO: Check if this is correct.
                CreatedUtc = pastDays,
                CreatedByUserId = UserContext.UserId,
                ModifiedByUserId = UserContext.UserId,
            };
            try
            {
                product.PartRevisionId = PartRevisionService.AddPartRevision(partRevision);
            }
            catch (DbEntityValidationException ex)
            {
                throw new ApplicationException("Add Part Revision failed: " + ex.RetrieveDbEntityValidationException());
            }

            int unitOfMeasurement = 0;
            if (!string.IsNullOrEmpty(row.UnitOfMeasurement))
            {
                unitOfMeasurement = (int)Enum.Parse(typeof(MEASUREMENT_UNITS), row.UnitOfMeasurement);
            }

            // PriceBreak
            var priceBreakIds = new List<PriceBreak>();
            foreach (var priceBreakData in priceBreaks)
            {
                var priceBreak = new PriceBreak
                {
                    ProductId = productId,
                    TaskId = taskId,
                    Quantity = decimal.Parse(priceBreakData.Quantity) > 0 ? decimal.Parse(priceBreakData.Quantity) : 1,
                    UnitPrice = priceBreakData.UnitPrice ?? 0m,
                    VendorUnitPrice = priceBreakData.UnitPrice ?? 0m,
                    CreatedByUserId = UserContext.UserId,
                    ModifiedByUserId = UserContext.UserId,
                    ProductPriceQuoteId = productPriceQuoteId,
                    UnitOfMeasurement = unitOfMeasurement,

                };

                try
                {
                    var id = PriceBreakService.AddPriceBreak(priceBreak);
                    priceBreakIds.Add(priceBreak);
                }
                catch (DbEntityValidationException ex)
                {
                    throw new ApplicationException("Creating PriceBreak failed: " + ex.RetrieveDbEntityValidationException());
                }
            }
            product.PriceBreakId = priceBreakIds.OrderBy(pk => pk.UnitPrice).FirstOrDefault()?.Id;

            var rfqBit = new RFQQuantity
            {
                Qty1 = priceBreakIds.GetOrDefault(0)?.Quantity,
                Qty2 = priceBreakIds.GetOrDefault(1)?.Quantity,
                Qty3 = priceBreakIds.GetOrDefault(2)?.Quantity,
                Qty4 = priceBreakIds.GetOrDefault(3)?.Quantity,
                Qty5 = priceBreakIds.GetOrDefault(4)?.Quantity,
                Qty6 = priceBreakIds.GetOrDefault(5)?.Quantity,
            };
            try
            {
                product.RFQQuantityId = RfqQuantityService.AddRFQQuantity(rfqBit);
            }
            catch (DbEntityValidationException ex)
            {
                throw new ApplicationException("Creating RFQQuantity failed: " + ex.RetrieveDbEntityValidationException());
            }

            try
            {
                ProductService.UpdateProduct(product);
            }
            catch (DbEntityValidationException ex)
            {
                throw new ApplicationException("Updating Product failed: " + ex.RetrieveDbEntityValidationException());
            }

            return product;
        }


        public class NewFrontEndInvitationNewUserModel
        {
            [Required]
            public Inviter inviter { get; set; }

            [Required]
            public Invitee invitee { get; set; }
        }

        public class Inviter
        {
            [Required]
            public string companyId { get; set; }

            [Required]
            public string userId { get; set; }
        }

        public class Invitee
        {
            [Required]
            public string companyId { get; set; }

            [Required]
            public Phone phone { get; set; }

            [Required, EmailAddress]
            public string email { get; set; }

            [Required]
            public string firstName { get; set; }
            [Required]
            public string lastName { get; set; }
            [Required]
            public string companyName { get; set; }
            [CanBeNull]
            public string currency { get; set; }
            [Required]
            public bool hasCreditTerms { get; set; }

            [Required]
            public CompanyAddress companyAddress { get; set; }
        }

        public class Phone
        {
            [Required]
            public string country { get; set; }
            [Required]
            public string number { get; set; }
        }

        public class CompanyAddress
        {
            [Required]
            public string country { get; set; }
            public string region { get; set; }
            [Required]
            public string city { get; set; }
            [Required]
            public string address { get; set; }
            [Required]
            public string postalCode { get; set; }
        }
    }

    [NotMapped]
    public class ApplicationUserOnBoardInfo : ApplicationUser
    {
        public string Country { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddressRegion { get; set; }
        public string CompanyAddressCity { get; set; }
        public string CompanyAddressLine { get; set; }
        public string CompanyAddressPostalCode { get; set; }
        public string Currency { get; set; }
        public bool HasCreditTerms { get; set; }
    }

}
