using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common;
using Intuit.Ipp.Data;
using Libs.Notification;
using Microsoft.Ajax.Utilities;
using Microsoft.Azure;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;
using Omnae.BlobStorage;
using Omnae.BusinessLayer;
using Omnae.BusinessLayer.Identity.Model;
using Omnae.BusinessLayer.Interface;
using Omnae.BusinessLayer.Models;
using Omnae.Common;
using Omnae.Common.Extensions;
using Omnae.Data;
using Omnae.Data.Migrations;
using Omnae.Data.Query;
using Omnae.Libs.Notification;
using Omnae.Model.Context;
using Omnae.Model.Context.Model;
using Omnae.Model.Extentions;
using Omnae.Model.Models;
using Omnae.QuickBooks.ViewModels;
using Omnae.Service.Service;
using Omnae.Service.Service.Interfaces;
using Omnae.WebApi.DTO;
using Omnae.WebApi.Models;
using Omnae.WebApi.Util;
using RazorEngine.Compilation.ImpromptuInterface;
using Serilog;
using static Omnae.Data.Query.CompanyQuery;
using static Omnae.Data.Query.NcrQuery;
using static Omnae.Data.Query.OrderQuery;
using Company = Omnae.Model.Models.Company;
using CompanyInfo = Omnae.Model.Context.Model.CompanyInfo;
using Task = System.Threading.Tasks.Task;
using UserInfo = Omnae.Model.Context.Model.UserInfo;

namespace Omnae.WebApi.Controllers
{
    /// <summary>
    /// Core Api for Company 
    /// </summary>
    [EnableCors("*", "*", "*")]
    [RoutePrefix("api/companies")]
    public class CompaniesController : BaseApiController
    {
        private IMapper Mapper { get; }
        private readonly ICompanyService companyService;
        private readonly ICompaniesCreditRelationshipService companiesCreditRelationshipService;
        private readonly IAddressService addressService;
        private readonly IShippingProfileService shippingProfileService;
        private readonly IShippingAccountService shippingAccountService;
        private readonly CompanyBL CompanyBL;
        private readonly CompanyAccountsBL companyAccountBl;
        private readonly ILogedUserContext userContext;
        private readonly IHomeBL homeBL;
        private readonly IImageStorageService ImageStorageService;
        private readonly ICountryService countryService;
        private readonly ITaskDataService taskDataService;
        private readonly OnBoardingApiBL OnBoardingApiBL;
        private readonly NotificationService NotificationService;
        private readonly IStateProvinceService StateProvinceService;
        private readonly IProductSharingService productSharingService;
        private readonly ProductBL productBL;
        private readonly IProductService productService;
        private readonly IOrderService orderService;
        private readonly ChartBL chartBL;
        private readonly INCReportService ncReportService;
        private readonly IProductStateTrackingService productStateTrackingService;


        /// <summary>
        /// Company controller constructor
        /// </summary>
        public CompaniesController(ILogger log, ICompanyService companyService, ICompaniesCreditRelationshipService companiesCreditRelationshipService, IAddressService addressService, IShippingProfileService shippingProfileService, IShippingAccountService shippingAccountService, CompanyBL companyBl, CompanyAccountsBL companyAccountBl, ILogedUserContext userContext, IHomeBL homeBl, ICountryService countryService, ITaskDataService taskDataService, OnBoardingApiBL onBoardingApiBl, NotificationService notificationService, IMapper mapper, IStateProvinceService stateProvinceService, IProductSharingService productSharingService, IOrderService orderService, ChartBL chartBL, INCReportService ncReportService, ProductBL productBL, IProductService productService, IImageStorageService imageStorageService, IProductStateTrackingService productStateTrackingService) : base(log)
        {
            this.companyService = companyService;
            this.companiesCreditRelationshipService = companiesCreditRelationshipService;
            this.addressService = addressService;
            this.shippingProfileService = shippingProfileService;
            this.shippingAccountService = shippingAccountService;
            CompanyBL = companyBl;
            this.companyAccountBl = companyAccountBl;
            this.userContext = userContext;
            homeBL = homeBl;
            this.countryService = countryService;
            this.taskDataService = taskDataService;
            OnBoardingApiBL = onBoardingApiBl;
            NotificationService = notificationService;
            Mapper = mapper;
            StateProvinceService = stateProvinceService;
            this.productSharingService = productSharingService;
            this.productService = productService;
            ImageStorageService = imageStorageService;
            this.productBL = productBL;
            this.orderService = orderService;
            this.chartBL = chartBL;
            this.ncReportService = ncReportService;
            this.productStateTrackingService = productStateTrackingService;
        }

        /// <summary>
        /// Get all companies
        /// </summary>
        /// <param name="filter">Filtered by company Name or Email</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        /// <returns>A page of Companies</returns>
        [Route("", Name = "GetCompanies")]
        [HttpGet]
        public async Task<PagedResultSet<CompanyDTO>> GetCompanies(string filter = null, int? page = 1, int pageSize = PageSize, string orderBy = nameof(CompanyDTO.Id), bool ascending = false)
        {
            var companies = companyService.FindAllCompanies().FilterBy(filter);

            return await PageOfResultsSetAsync<Company, CompanyDTO>(companies, page, pageSize, orderBy, ascending, "GetCompanies");
        }



        /// <summary>
        /// Get company details which id = {id}
        /// </summary>
        /// <param name="id">The ID of the company.</param>
        [HttpGet]
        [Route("{id:int}", Name = "GetCompany")]
        [ResponseType(typeof(CompanyDTO))]
        public IHttpActionResult GetCompany(int id)
        {
            var companyDto = companyService.FindAllCompanies(onlyActive: false)
                                         .Where(c => c.Id == id)
                                         .ProjectTo<CompanyDTO>()
                                         .SingleOrDefault();

            var company = companyService.FindCompanyById(id);
            companyDto.CurrencyCode = (CurrencyCodes) Enum.Parse(typeof(CurrencyCodes), company.Currency.ToString());

            if (companyDto == null)
                return NotFound();

            return Ok(companyDto);
        }

        /// <summary>
        /// Modify company which id = {id}
        /// </summary>
        /// <param name="id">The id of the company.</param>
        /// <param name="company">The entity of the company to be modified.</param>
        [ResponseType(typeof(void))]
        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult PutCompany(int id)//, CompanyDTO company)
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var httpRequest = HttpContext.Current.Request;
            var allFiles = GetPostedFiles(homeBL, filesAreRequired: false);

            try
            {
                var formDataDic = httpRequest.Form.ToDictionaryOfObjects().ClearEmptyEntries();
                var model = Map(formDataDic);

                Validate(model);
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                if (id != model.Id)
                    return BadRequest("Invalid Company ID.");
                if (allFiles.Count > 2)
                    return BadRequest("Use one or no file to the Company Logo.");

                var currentCompanyInDb = companyService.FindCompanyById(id);
                if (currentCompanyInDb == null)
                    return NotFound();

                var addressId = currentCompanyInDb.AddressId ?? currentCompanyInDb.Address?.Id ?? model.Address?.Id;
                var shippingId = currentCompanyInDb.ShippingId ?? currentCompanyInDb.Shipping?.Id ?? model.Shipping?.Id;
                var billAddressId = currentCompanyInDb.BillAddressId ?? currentCompanyInDb.BillAddress?.Id ?? model.BillingAddress?.Id;
                var mainCompanyAddressId = currentCompanyInDb.MainCompanyAddress_Id ?? currentCompanyInDb.MainCompanyAddress?.Id ?? model.MainCompanyAddress?.Id;


                var companyWithNewData = Mapper.Map(model, currentCompanyInDb);
                addressService.FixIds(companyWithNewData.Address);
                addressService.FixIds(companyWithNewData.BillAddress);
                addressService.FixIds(companyWithNewData.MainCompanyAddress);
                addressService.FixIds(companyWithNewData.Shipping?.Address);

                if (companyWithNewData.Address != null)
                {
                    companyWithNewData.Address.Id = addressId ?? 0;
                    companyWithNewData.AddressId = addressId;
                }
                if (companyWithNewData.Shipping != null)
                {
                    companyWithNewData.Shipping.Id = shippingId ?? 0;
                    companyWithNewData.ShippingId = shippingId;
                }
                if (companyWithNewData.BillAddress != null)
                {
                    companyWithNewData.BillAddress.Id = billAddressId ?? 0;
                    companyWithNewData.BillAddressId = billAddressId;
                }
                if (companyWithNewData.MainCompanyAddress != null)
                {
                    companyWithNewData.MainCompanyAddress.Id = mainCompanyAddressId ?? 0;
                    companyWithNewData.MainCompanyAddress_Id = mainCompanyAddressId;
                }

                companyWithNewData.WasInvited = companyWithNewData.WasInvited || companyWithNewData.InvitedByCompanyId != null;
                companyWithNewData.WasOnboarded = companyWithNewData.WasOnboarded || companyWithNewData.OnboardedByCompanyId != null;

                using (var tran = AsyncTransactionScope.StartNew())
                {
                    try
                    {
                        var logo = allFiles.FirstOrDefault();
                        if (logo != null)
                        {
                            string logoName = $"Logo_{currentCompanyInDb.Name}{new FileInfo(logo.FileName).Extension}";
                            companyWithNewData.CompanyLogoUri = allFiles?.Any() == true
                                                                    ? ImageStorageService.Upload(logo, logoName)
                                                                    : companyWithNewData.CompanyLogoUri;
                        }

                        CompanyBL.UpdateCompanyAndAddresses(companyWithNewData);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!CompanyExists(id))
                            return NotFound();
                        throw;
                    }

                    tran.Complete();
                }

                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            finally
            {
                DeleteUploadFiles(allFiles);
            }
        }

        /// <summary>
        /// Create a new company
        /// </summary>
        /// <returns>The Company</returns>
        [HttpPost]
        [Route("")]
        [ResponseType(typeof(CompanyDTO))]
        public IHttpActionResult PostCompany()//CompanyDTO company)
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var httpRequest = HttpContext.Current.Request;
            var allFiles = GetPostedFiles(homeBL, filesAreRequired: false);

            try
            {
                var formDataDic = httpRequest.Form.ToDictionaryOfObjects().ClearEmptyEntries();
                var model = Map(formDataDic);

                Validate(model);
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                if (allFiles.Count > 2)
                    return BadRequest("Use one or no file to the Company Logo.");

                var companyWithNewData = Mapper.Map<Company>(model);
                addressService.FixIds(companyWithNewData.Address);
                addressService.FixIds(companyWithNewData.BillAddress);
                addressService.FixIds(companyWithNewData.MainCompanyAddress);
                addressService.FixIds(companyWithNewData.Shipping?.Address);

                companyWithNewData.WasInvited = companyWithNewData.WasInvited || companyWithNewData.InvitedByCompanyId != null;

                using (var tran = AsyncTransactionScope.StartNew())
                {
                    var logo = allFiles.FirstOrDefault();
                    if (logo != null)
                    {
                        string logoName = $"Logo_{companyWithNewData.Name}{new FileInfo(logo.FileName).Extension}";
                        companyWithNewData.CompanyLogoUri = allFiles?.Any() == true
                            ? ImageStorageService.Upload(logo, logoName)
                            : companyWithNewData.CompanyLogoUri;
                    }


                    var cid = companyService.AddCompany(companyWithNewData);
                    CompanyBL.UpdateCompanyAndAddresses(companyWithNewData);

                    var result = Mapper.Map(companyWithNewData, model);

                    tran.Complete();
                    return CreatedAtRoute("GetCompany", new { id = cid }, result);
                }
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            finally
            {
                DeleteUploadFiles(allFiles);
            }
        }

        private static CompanyDTO Map(IDictionary<string, object> formDataDic)
        {
            if (formDataDic == null || formDataDic.Count == 0)
                return null;

            var mainCompanyAddress = MapAddr(formDataDic, "mainCompanyAddress");
            var address = MapAddr(formDataDic, "address");
            var shipping = MapShipping(formDataDic);
            var billingAddress = MapAddr(formDataDic, "billingAddress");

            CurrencyCodes currencyCode = CurrencyCodes.USD;
            if (Enum.TryParse<CurrencyCodes>(formDataDic.GetOrNull("currency")?.ToString(), out currencyCode) == false)
            {
                currencyCode = CurrencyCodes.USD;
            }
                

            var model = new CompanyDTO()
            {
                Id = formDataDic.GetOrNull("id")?.ToString()?.ToInt() ?? formDataDic.GetOrNull("Id")?.ToString()?.ToInt(),
                Name = formDataDic.GetOrNull("name")?.ToString() ?? formDataDic.GetOrNull("Name")?.ToString(),
                Term = formDataDic.GetOrNull("term")?.ToString()?.ToInt() ?? formDataDic.GetOrNull("Term")?.ToString()?.ToInt(),
                CreditLimit = formDataDic.GetOrNull("creditLimit")?.ToString()?.ToDecimal() ?? formDataDic.GetOrNull("creditlimit")?.ToString()?.ToDecimal() ?? formDataDic.GetOrNull("CreditLimit")?.ToString()?.ToDecimal(),
                isQualified = formDataDic.GetOrNull("isQualified")?.ToString()?.ToBool() ?? formDataDic.GetOrNull("isqualified")?.ToString()?.ToBool() ?? formDataDic.GetOrNull("IsQualified")?.ToString()?.ToBool(),
                AccountingEmail = formDataDic.GetOrNull("accountingEmail")?.ToString() ?? formDataDic.GetOrNull("accountingemail")?.ToString() ?? formDataDic.GetOrNull("AccountingEmail")?.ToString(),
                StripeCustomerId = formDataDic.GetOrNull("stripeCustomerId")?.ToString() ?? formDataDic.GetOrNull("stripecustomerid")?.ToString() ?? formDataDic.GetOrNull("StripeCustomerId")?.ToString(),

                Shipping = shipping,
                Address = address,
                MainCompanyAddress = mainCompanyAddress,
                BillingAddress = billingAddress,
                InvitedByCompanyId = formDataDic.GetOrNull("invitedByCompanyId")?.ToString()?.ToInt() ?? formDataDic.GetOrNull("InvitedByCompanyId")?.ToString()?.ToInt(),
                CurrencyCode = currencyCode,
            };
            if (model.InvitedByCompanyId != null)
            {
                model.WasInvited = true;
            }
            return model;
        }

        private static ShippingDTO MapShipping(IDictionary<string, object> formDataDic)
        {
            var shippingAddress = MapAddr(formDataDic, "shipping_address");

            if (formDataDic.Keys.Any(k => k.StartsWith($"shipping_", StringComparison.InvariantCultureIgnoreCase)))
            {
                return new ShippingDTO
                {
                    Id = formDataDic.GetOrNull("shipping_Id")?.ToString()?.ToInt() ?? formDataDic.GetOrNull("shipping_id")?.ToString()?.ToInt() ?? formDataDic.GetOrNull("Shipping_Id")?.ToString()?.ToInt(),
                    Attention_FreeText = formDataDic.GetOrNull("shipping_attention_FreeText")?.ToString() ?? formDataDic.GetOrNull("shipping_attention_freetext")?.ToString() ?? formDataDic.GetOrNull("Shipping_Attention_FreeText")?.ToString(),
                    EmailAddress = formDataDic.GetOrNull("shipping_emailAddress")?.ToString() ?? formDataDic.GetOrNull("shipping_emailAddress")?.ToString() ?? formDataDic.GetOrNull("shipping_emailAddress")?.ToString(),
                    PhoneNumber = formDataDic.GetOrNull("shipping_phoneNumber")?.ToString() ?? formDataDic.GetOrNull("shipping_phoneNumber")?.ToString() ?? formDataDic.GetOrNull("shipping_phoneNumber")?.ToString(),
                    Address = shippingAddress,
                };
            }

            return null;
        }

        private static AddressDTO MapAddr(IDictionary<string, object> formDataDic, string prefix)
        {
            if (formDataDic.Keys.Any(k => k.StartsWith($"{prefix}_", StringComparison.InvariantCultureIgnoreCase)))
            {
                return new AddressDTO
                {
                    Id = formDataDic.GetOrNull($"{prefix}_id")?.ToString()?.ToInt() ?? formDataDic.GetOrNull($"{prefix.ToLowerInvariant()}_id")?.ToString()?.ToInt() ?? formDataDic.GetOrNull($"{prefix.ToCapital()}_id")?.ToString()?.ToInt(),
                    Country = formDataDic.GetOrNull($"{prefix}_country")?.ToString() ?? formDataDic.GetOrNull($"{prefix.ToLowerInvariant()}_country")?.ToString() ?? formDataDic.GetOrNull($"{prefix.ToCapital()}_country")?.ToString(),
                    City = formDataDic.GetOrNull($"{prefix}_city")?.ToString() ?? formDataDic.GetOrNull($"{prefix.ToLowerInvariant()}_country")?.ToString() ?? formDataDic.GetOrNull($"{prefix.ToCapital()}_country")?.ToString(),
                    StateOrProvinceName = formDataDic.GetOrNull($"{prefix}_stateOrProvinceName")?.ToString() ?? formDataDic.GetOrNull($"{prefix.ToLowerInvariant()}_stateorprovincename")?.ToString() ?? formDataDic.GetOrNull($"{prefix.ToCapital()}_stateOrProvinceName")?.ToString(),
                    AddressLine1 = formDataDic.GetOrNull($"{prefix}_addressLine1")?.ToString() ?? formDataDic.GetOrNull($"{prefix.ToLowerInvariant()}_addressline1")?.ToString() ?? formDataDic.GetOrNull($"{prefix.ToCapital()}_addressLine1")?.ToString(),
                    AddressLine2 = formDataDic.GetOrNull($"{prefix}_addressLine2")?.ToString() ?? formDataDic.GetOrNull($"{prefix.ToLowerInvariant()}_addressline2")?.ToString() ?? formDataDic.GetOrNull($"{prefix.ToCapital()}_addressLine2")?.ToString(),
                    PostalCode = formDataDic.GetOrNull($"{prefix}_postalCode")?.ToString() ?? formDataDic.GetOrNull($"{prefix.ToLowerInvariant()}_postalCode")?.ToString() ?? formDataDic.GetOrNull($"{prefix.ToCapital()}_postalCode")?.ToString(),
                    ZipCode = formDataDic.GetOrNull($"{prefix}_zipCode")?.ToString() ?? formDataDic.GetOrNull($"{prefix.ToLowerInvariant()}_zipcode")?.ToString() ?? formDataDic.GetOrNull($"{prefix.ToCapital()}_zipCode")?.ToString(),
                };
            }

            return null;
        }

        /// <summary>
        /// Get company Stripe id
        /// </summary>
        /// <param name="id">The ID of the company.</param>
        [HttpGet]
        [Route("{id:int}/stripe", Name = "GetStripeCompanyInfo")]
        [ResponseType(typeof(string))]
        public IHttpActionResult GetStripeCompanyInfo(int id)
        {
            var company = companyService.FindAllCompanies(onlyActive: false)
                .Where(c => c.Id == id)
                .ProjectTo<CompanyDTO>()
                .SingleOrDefault();

            if (company == null)
                return NotFound();

            return Ok(company.StripeCustomerId);
        }

        /// <summary>
        /// Modify company which id = {id}
        /// </summary>
        /// <param name="id">The id of the company.</param>
        /// <param name="company">The entity of the company to be modified.</param>
        /// <param name="stripeCustomerId">The strip ID</param>
        [ResponseType(typeof(void))]
        [HttpPut]
        [Route("{id:int}/stripe", Name = "UpdateStripeCompanyInfo")]
        public IHttpActionResult UpdateStripeCompanyInfo(int id, [Required] string stripeCustomerId) //, CompanyDTO company)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(stripeCustomerId))
                return BadRequest(ModelState);

            var currentCompanyInDb = companyService.FindCompanyById(id);
            if (currentCompanyInDb == null)
                return NotFound();

            currentCompanyInDb.StripeCustomerId = stripeCustomerId;

            using (var tran = AsyncTransactionScope.StartNew())
            {
                CompanyBL.UpdateCompanyAndAddresses(currentCompanyInDb);

                tran.Complete();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }


        /// <summary>
        /// Get company Logo
        /// </summary>
        /// <param name="id">The ID of the company.</param>
        [HttpGet]
        [Route("{id:int}/logo", Name = "GetCompanyLogo")]
        [ResponseType(typeof(string))]
        public IHttpActionResult GetCompanyLogo(int id)
        {
            var company = companyService.FindAllCompanies(onlyActive: false)
                .Where(c => c.Id == id)
                .ProjectTo<CompanyDTO>()
                .SingleOrDefault();

            if (company == null)
                return NotFound();

            return Ok(company.CompanyLogoUri);
        }

        /// <summary>
        /// Modify company logo id = {id}
        /// </summary>
        /// <param name="id">The id of the company.</param>
        /// <param name="company">The entity of the company to be modified.</param>
        [ResponseType(typeof(string))]
        [HttpPut]
        [Route("{id:int}/logo", Name = "UpdateCompanyLogo")]
        public IHttpActionResult UpdateCompanyLogo(int id)
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            try
            {
                var allFiles = GetPostedFiles(homeBL, filesAreRequired: true);

                try
                {
                    var currentCompanyInDb = companyService.FindCompanyById(id);
                    if (currentCompanyInDb == null)
                        return NotFound();

                    var logo = allFiles.FirstOrDefault();
                    if (logo != null)
                    {
                        string logoName = $"Logo_{currentCompanyInDb.Name}{new FileInfo(logo.FileName).Extension}";
                        currentCompanyInDb.CompanyLogoUri = allFiles?.Any() == true ? ImageStorageService.Upload(logo, logoName) : currentCompanyInDb.CompanyLogoUri;
                    }
                    else
                    {
                        currentCompanyInDb.CompanyLogoUri = null;
                    }

                    CompanyBL.UpdateCompanyAndAddresses(currentCompanyInDb);

                    return Ok(currentCompanyInDb.CompanyLogoUri);
                }
                finally
                {
                    DeleteUploadFiles(allFiles);
                }
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get all CompaniesCreditRelationships on Vendor ID
        /// </summary>
        /// <param name="vendorId">Vendor Id</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc - True will order 'orderBy' ascending</param>
        [HttpGet]
        [Route("vendor/{userId}/GetCompaniesCreditRelationshipsOnVendor/{vendorId:int}", Name = "GetCompaniesCreditRelationshipsOnVendor")]
        public async Task<PagedResultSet<CompaniesCreditRelationshipViewModel>> GetCompaniesCreditRelationshipsOnVendor(int vendorId, int? page = 1, int pageSize = PageSize,
                                                                                    string orderBy = nameof(CompaniesCreditRelationshipViewModel.CustomerId),
                                                                                    bool ascending = false)
        {
            var records = companiesCreditRelationshipService.FindCompaniesCreditRelationshipIQueryableByVendorId(vendorId);
            return await PageOfResultsSetAsync<CompaniesCreditRelationship, CompaniesCreditRelationshipViewModel>(records, page, pageSize, orderBy, ascending, "GetCompaniesCreditRelationshipsOnVendor");
        }

        /// <summary>
        /// Get all CompaniesCreditRelationships on Customer ID
        /// </summary>
        /// <param name="vendorId">Vendor Id</param>
        /// <param name="customerId">Customer Id</param>
        [HttpGet]
        [Route("vendor/{vendorId:int}/GetCreditRelationships/{customerId:int}", Name = "GetCreditRelationships")]
        [ResponseType(typeof(CompaniesCreditRelationshipViewModel))]
        public IHttpActionResult GetCreditRelationships(int vendorId, int customerId)
        {
            var record = companiesCreditRelationshipService.FindCompaniesCreditRelationshipByCustomerIdVendorId(customerId, vendorId);
            var model = Mapper.Map<CompaniesCreditRelationshipViewModel>(record);
            return Ok(model);
        }

        /// <summary>
        /// Assign Term days and Credit Limit to a customer by vendor
        /// </summary>
        /// <param name="vendorId">Vendor Id</param>
        /// <param name="customerId">Customer Id</param>
        [HttpPost]
        [Route("vendor/{vendorId:int}/AssignTermCreditToCustomer/{customerId:int}", Name = "AssignTermCreditToCustomer")]
        [ResponseType(typeof(CompaniesCreditRelationshipViewModel))]
        public IHttpActionResult AssignTermCreditToCustomer(int vendorId, int customerId)
        {
            var httpRequest = HttpContext.Current.Request;
            var formDataDic = httpRequest.Form.ToDictionary();
            if (!formDataDic.ContainsKey("isTerm") || !formDataDic.ContainsKey("termDays") || !formDataDic.ContainsKey("creditLimit"))
                return BadRequest(IndicatingMessages.MissingFormData);

            var curr = formDataDic.ContainsKey("currency") ? Enum.TryParse<CurrencyCodes>(formDataDic["currency"], true, out var curr1) ? (CurrencyCodes?)curr1 : null : null;
            var currText = formDataDic.ContainsKey("currencyText") ? Enum.TryParse<CurrencyCodes>(formDataDic["currencyText"], true, out var curr2) ? (CurrencyCodes?)curr2 : null : null;
            var taxPercentage = formDataDic.ContainsKey("taxPercentage") ? int.TryParse(formDataDic["taxPercentage"], out var tpt) ? tpt : 0 : 0;

            var model = new CompaniesCreditRelationshipViewModel
            {
                CustomerId = customerId,
                VendorId = vendorId,
                isTerm = bool.Parse(formDataDic["isTerm"]),
                TermDays = int.Parse(formDataDic["termDays"]),
                CreditLimit = decimal.Parse(formDataDic["creditLimit"]),
                DiscountDays = formDataDic.ContainsKey("discountDays") && string.IsNullOrEmpty(formDataDic["discountDays"]) == false ? int.Parse(formDataDic["discountDays"]) : (int?)null,
                Discount = formDataDic.ContainsKey("discount") && string.IsNullOrEmpty(formDataDic["discount"]) == false ? float.Parse(formDataDic["discount"]) : (float?)null,
                Deposit = formDataDic.ContainsKey("deposit") && string.IsNullOrEmpty(formDataDic["deposit"]) == false ? int.Parse(formDataDic["deposit"]) : (int?)null,
                Currency = currText ?? curr ?? CurrencyCodes.USD,
                ToolingDepositPercentage = formDataDic.ContainsKey("toolingDepositPercentage") &&
                                           string.IsNullOrEmpty(formDataDic["toolingDepositPercentage"]) == false ?
                                           int.Parse(formDataDic["toolingDepositPercentage"]) : (int?)null,
                TaxPercentage = taxPercentage,
            };

            var error = homeBL.AssignCustomerTerms(model);
            if (error != null)
            {
                return BadRequest(error);
            }

            return Ok();
        }

        /// <summary>
        /// Delete term and credit limits from a customer
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        [ResponseType(typeof(void))]
        [HttpDelete]
        [Route("vendor/{userId}/DeleteTermCreditFromCustomer/{customerId:int}")]
        public async Task<IHttpActionResult> DeleteTermCreditFromCustomer(int customerId)
        {
            var simplifiedUser = await companyAccountBl.FindByIdAsync(userContext.UserId, null);
            var vendorId = simplifiedUser.CompanyId.Value;

            var entity = companiesCreditRelationshipService.FindCompaniesCreditRelationshipByCustomerIdVendorId(customerId, vendorId);
            try
            {
                companiesCreditRelationshipService.DeleteCompaniesCreditRelationship(entity);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.RetrieveErrorMessage());
            }

            return Ok("Deleting success");
        }

        /// <summary>
        /// Get company addresses
        /// </summary>
        /// <param name="id">The ID of the company.</param>
        [HttpGet]
        [Route("{companyId:int}/Addresses/", Name = "GetCompanyAddresses")]
        public async Task<PagedResultSet<AddressDTO>> GetCompanyAddresses([Required] int companyId, AddressesQuery.AddressFilter filterBy = AddressesQuery.AddressFilter.All, int? page = 1, int pageSize = PageSize, string orderBy = nameof(AddressDTO.Id), bool ascending = false)
        {
            var addrs = addressService.FindAllFromCompanies(companyId);
            return await PageOfResultsSetAsync<Address, AddressDTO>(addrs, page, pageSize, orderBy, ascending, "GetCompanyAddresses");
        }

        /// <summary>
        /// List Company Shipping Profiles
        /// </summary>
        /// <param name="companyId">The company ID</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        /// <returns>A page of Shipping Profiles</returns>
        [HttpGet]
        [Route("{companyId:int}/ShippingProfiles/", Name = "GetShippingProfiles")]
        public async Task<PagedResultSet<ShippingProfileDTO>> GetShippingProfiles([Required] int companyId, int? page = 1, int pageSize = PageSize, string orderBy = nameof(ShippingProfileDTO.Id), bool ascending = false)
        {
            var addrs = shippingProfileService.FindAllByCompanyId(companyId);
            return await PageOfResultsSetAsync<ShippingProfile, ShippingProfileDTO>(addrs, page, pageSize, orderBy, ascending, "GetShippingProfiles");
        }

        /// <summary>
        /// Get the Company Default Shipping Profiles
        /// </summary>
        /// <param name="companyId">The company ID</param>
        /// <returns>The Default Shipping Profiles</returns>
        [HttpGet]
        [Route("{companyId:int}/ShippingProfiles/default", Name = "GetShippingProfilesDefault")]
        [ResponseType(typeof(ShippingProfileDTO))]
        public async Task<IHttpActionResult> GetShippingProfilesDefault([Required] int companyId)
        {
            var addr = await shippingProfileService.FindAllByCompanyId(companyId).FirstOrDefaultAsync(s => s.IsActive && s.IsDefault);
            if (addr == null)
                return NotFound();
            return Ok(Mapper.Map<ShippingProfileDTO>(addr));
        }

        /// <summary>
        /// Get a Company Shipping Profile 
        /// </summary>
        /// <param name="companyId">The company ID</param>
        /// <param name="shippingProfileId">The Shipping Profile ID</param>
        /// <returns>The Shipping Profile data</returns>
        [HttpGet]
        [Route("{companyId:int}/ShippingProfiles/{shippingProfileId:int}", Name = "GetShippingProfile")]
        [ResponseType(typeof(ShippingProfileDTO))]
        public IHttpActionResult GetShippingProfile([Required] int companyId, [Required] int shippingProfileId)
        {
            var profile = shippingProfileService.FindById(shippingProfileId);

            if (profile.CompanyId != companyId)
                return NotFound();
            if (profile.IsActive == false)
                return NotFound();

            var dto = Mapper.Map<ShippingProfileDTO>(profile);
            return Ok(dto);
        }

        /// <summary>
        /// Set a default Company Shipping Profile 
        /// </summary>
        /// <param name="companyId">The company ID</param>
        /// <param name="shippingProfileId">The Shipping Profile ID</param>
        /// <returns>Nothing</returns>
        [HttpPut]
        [Route("{companyId:int}/ShippingProfiles/default", Name = "SetShippingProfileDefault")]
        public IHttpActionResult SetShippingProfileDefault([Required] int companyId, [Required] int shippingProfileId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var profileInDatabase = shippingProfileService.FindById(shippingProfileId);

            if (profileInDatabase == null)
                return NotFound();
            if (profileInDatabase.CompanyId != companyId)
                return NotFound();

            using (var tran = AsyncTransactionScope.StartNew())
            {
                shippingProfileService.SetShippingProfileAsDefault(profileInDatabase);

                tran.Complete();
            }
            return StatusCode(HttpStatusCode.OK);
        }

        /// <summary>
        /// Update a Company Shipping Profile 
        /// </summary>
        /// <param name="companyId">The company ID</param>
        /// <param name="shippingProfileId">The Shipping Profile ID</param>
        /// <param name="shippingProfileModel">The Shipping Profile Data</param>
        /// <param name="userId">Optional - The user id that is Changing the data</param>
        /// <returns>Nothing</returns>
        [HttpPut]
        [Route("{companyId:int}/ShippingProfiles/{shippingProfileId:int}", Name = "PutShippingProfile")]
        [ResponseType(typeof(ShippingProfileDTO))]
        public IHttpActionResult PutShippingProfile([Required] int companyId, [Required] int shippingProfileId, [Required] ShippingProfileDTO shippingProfileModel, string userId = null)
        {
            if (shippingProfileModel.Id == default)
            {
                shippingProfileModel.Id = shippingProfileId;
            }
            if (shippingProfileModel.CompanyId == null)
            {
                shippingProfileModel.CompanyId = companyId;
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (shippingProfileId != shippingProfileModel.Id)
                return BadRequest("Invalid shippingProfileId");

            if (companyId != shippingProfileModel.CompanyId)
                return BadRequest("Invalid Company");

            var profileInDatabase = shippingProfileService.FindById(shippingProfileId);

            if (profileInDatabase == null)
                return NotFound();
            if (profileInDatabase.CompanyId != companyId)
                return NotFound();
            if (profileInDatabase.IsActive == false)
                return NotFound();

            if (shippingProfileModel.IsDefault == null)
            {
                shippingProfileModel.IsDefault = profileInDatabase.IsDefault;
            }

            var shippingProfileWithNewData = Mapper.Map(shippingProfileModel, profileInDatabase);

            if (profileInDatabase.Shipping.AddressId != shippingProfileModel.Address.Id)
            {
                shippingProfileWithNewData.ShippingId = null;
                shippingProfileWithNewData.Shipping = Mapper.Map<Shipping>(shippingProfileModel);
            }

            try
            {
                using (var tran = AsyncTransactionScope.StartNew())
                {
                    shippingProfileService.Update(shippingProfileWithNewData);

                    tran.Complete();
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Log.Error(ex, "Conflict Updating the ShippingProfile:{shippingProfileId} ", shippingProfileId);
                return Conflict();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Add a Company Shipping Profile
        /// </summary>
        /// <param name="companyId">The company ID</param>
        /// <param name="shippingProfileModel">Shipping Profile Model</param>
        /// <param name="userId">Optional - The user id that is Changing the data</param>
        /// <returns>The Shipping Profile Data</returns>
        [HttpPost]
        [Route("{companyId:int}/ShippingProfiles/", Name = "PostShippingProfile")]
        [ResponseType(typeof(ShippingProfileDTO))]
        public IHttpActionResult PostShippingProfile([Required] int companyId, [Required] ShippingProfileDTO shippingProfileModel, string userId = null)
        {
            if (shippingProfileModel.CompanyId == null)
            {
                shippingProfileModel.CompanyId = companyId;
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (companyId != shippingProfileModel.CompanyId)
                return BadRequest();


            var shippingProfile = Mapper.Map<ShippingProfile>(shippingProfileModel);

            using (var tran = AsyncTransactionScope.StartNew())
            {
                var spId = shippingProfileService.Add(shippingProfile);

                var createdData = shippingProfileService.FindById(spId);
                var resultDTO = Mapper.Map<ShippingProfileDTO>(createdData);

                tran.Complete();

                return CreatedAtRoute("GetShippingProfile", new { companyId = companyId, shippingProfileId = spId }, resultDTO);
            }
        }

        /// <summary>
        /// Delete a company Shipping Profiles which id = {id}
        /// </summary>
        /// <param name="companyId">The company ID</param>
        /// <param name="shippingProfileId">The Shipping Profile ID</param>
        /// <returns>Nothing</returns>
        [HttpDelete]
        [Route("{companyId:int}/ShippingProfiles/{shippingProfileId:int}", Name = "DeleteShippingProfile")]
        [ResponseType(typeof(void))]
        public IHttpActionResult DeleteShippingProfile([Required] int companyId, [Required] int shippingProfileId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var profileInDatabase = shippingProfileService.FindById(shippingProfileId);

            if (profileInDatabase.CompanyId != companyId)
                return NotFound();
            if (profileInDatabase.IsActive == false)
                return NotFound();

            using (var tran = AsyncTransactionScope.StartNew())
            {
                shippingProfileService.Delete(profileInDatabase);

                tran.Complete();
            }
            return Ok();
        }



        /// <summary>
        /// List the Shipping Accounts for a Company
        /// </summary>
        /// <param name="companyId">The ID of the company</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        /// <returns>A page of Shipping Accounts</returns>
        [HttpGet]
        [Route("{companyId:int}/ShippingAccounts/", Name = "GetShippingAccounts")]
        public async Task<PagedResultSet<ShippingAccountDTO>> GetShippingAccounts([Required] int companyId, int? page = 1, int pageSize = PageSize, string orderBy = nameof(ShippingAccountDTO.Id), bool ascending = false)
        {
            var addrs = shippingAccountService.FindAllByCompanyId(companyId);
            return await PageOfResultsSetAsync<ShippingAccount, ShippingAccountDTO>(addrs, page, pageSize, orderBy, ascending, "GetShippingAccounts");
        }

        /// <summary>
        /// Get the Shipping Account for a Company
        /// </summary>
        /// <param name="companyId">The ID of the company</param>
        /// <param name="shippingAccountId">The ID Shipping Account</param>
        /// <returns>The Shipping Account data</returns>
        [HttpGet]
        [Route("{companyId:int}/ShippingAccounts/{shippingAccountId:int}", Name = "GetShippingAccount")]
        [ResponseType(typeof(ShippingAccountDTO))]
        public IHttpActionResult GetShippingAccount([Required] int companyId, [Required] int shippingAccountId)
        {
            var profile = shippingAccountService.FindById(shippingAccountId);

            if (profile == null || profile.CompanyId != companyId)
                return NotFound();
            if (profile == null || profile.IsActive == false)
                return NotFound();

            var dto = Mapper.Map<ShippingAccountDTO>(profile);
            return Ok(dto);
        }

        /// <summary>
        /// Get the Default Shipping Account for a Company
        /// </summary>
        /// <param name="companyId">The ID of the company</param>
        /// <param name="shippingAccountId">The ID Shipping Account</param>
        /// <returns>The Default Shipping Account data</returns>
        [HttpGet]
        [Route("{companyId:int}/ShippingAccounts/{shippingAccountId:int}/default", Name = "GetShippingAccountDefault")]
        [ResponseType(typeof(ShippingAccountDTO))]
        public async Task<IHttpActionResult> GetShippingAccountDefault([Required] int companyId, [Required] int shippingAccountId)
        {
            var profile = await shippingAccountService.FindAllByCompanyId(shippingAccountId).FirstOrDefaultAsync(s => s.IsActive && s.IsDefault);
            if (profile == null)
                return NotFound();

            var dto = Mapper.Map<ShippingAccountDTO>(profile);
            return Ok(dto);
        }

        /// <summary>
        /// Update the Shipping Account for a Company
        /// </summary>
        /// <param name="companyId">The ID of the company</param>
        /// <param name="shippingAccountId">The ID Shipping Account</param>
        /// <param name="shippingAccountModel">The Shipping Account</param>
        /// <param name="userId">The user ID that is doing the change</param>
        /// <returns>Nothing</returns>
        [HttpPut]
        [Route("{companyId:int}/ShippingAccounts/{shippingAccountId:int}", Name = "PutShippingAccount")]
        [ResponseType(typeof(ShippingAccountDTO))]
        public IHttpActionResult PutShippingAccount([Required] int companyId, [Required] int shippingAccountId, [Required] ShippingAccountDTO shippingAccountModel, string userId = null)
        {
            if (shippingAccountModel.Id == default)
            {
                shippingAccountModel.Id = shippingAccountId;
            }
            if (shippingAccountModel.CompanyId == null)
            {
                shippingAccountModel.CompanyId = companyId;
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (companyId != shippingAccountModel.CompanyId)
                return BadRequest();

            var accountInDatabase = shippingAccountService.FindById(shippingAccountId);

            if (accountInDatabase.CompanyId != companyId)
                return NotFound();
            if (accountInDatabase.IsActive == false)
                return NotFound();

            if (shippingAccountModel.IsDefault == null)
            {
                shippingAccountModel.IsDefault = accountInDatabase.IsDefault;
            }

            var shippingAccountWithNewData = Mapper.Map(shippingAccountModel, accountInDatabase);

            try
            {
                using (var tran = AsyncTransactionScope.StartNew())
                {
                    shippingAccountService.UpdateShippingAccount(shippingAccountWithNewData);

                    tran.Complete();
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Log.Error(ex, "Conflict Updating the ShippingAccount:{shippingAccountId} ", shippingAccountId);
                return Conflict();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Update the Shipping Account for a Company
        /// </summary>
        /// <param name="companyId">The ID of the company</param>
        /// <param name="shippingAccountId">The ID Shipping Account</param>
        /// <returns>Nothing</returns>
        [HttpPut]
        [Route("{companyId:int}/ShippingAccounts/{shippingAccountId:int}/default", Name = "SetShippingAccountDefault")]
        public IHttpActionResult SetShippingAccountDefault([Required] int companyId, [Required] int shippingAccountId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var accountInDatabase = shippingAccountService.FindById(shippingAccountId);

            if (accountInDatabase.CompanyId != companyId)
                return NotFound();

            using (var tran = AsyncTransactionScope.StartNew())
            {
                shippingAccountService.SetShippingAccountAsDefault(accountInDatabase);

                tran.Complete();
            }
            return StatusCode(HttpStatusCode.OK);
        }

        /// <summary>
        /// Create the Shipping Account for a Company
        /// </summary>
        /// <param name="companyId">The ID of the company</param>
        /// <param name="shippingAccountModel">The Shipping Account</param>
        /// <param name="userId">The user ID that is doing the change</param>
        /// <returns>The new Shipping Account data with new ID</returns>
        [HttpPost]
        [Route("{companyId:int}/ShippingAccounts/", Name = "PostShippingAccount")]
        [ResponseType(typeof(ShippingAccountDTO))]
        public IHttpActionResult PostShippingAccount([Required] int companyId, [Required] ShippingAccountDTO shippingAccountModel, string userId = null)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (companyId != shippingAccountModel.CompanyId)
                return BadRequest();


            var account = Mapper.Map<ShippingAccount>(shippingAccountModel);

            using (var tran = AsyncTransactionScope.StartNew())
            {
                var spId = shippingAccountService.AddShippingAccount(account);

                var createdData = shippingAccountService.FindById(spId);
                var resultDTO = Mapper.Map<ShippingAccountDTO>(createdData);

                tran.Complete();
                return CreatedAtRoute("GetShippingAccount", new { companyId = companyId, shippingAccountId = spId }, resultDTO);
            }
        }

        /// <summary>
        /// Delete a company ShippingAccount which id = {id}
        /// </summary>
        /// <param name="companyId">The company ID</param>
        /// <param name="shippingAccountId">The Shipping Account ID.</param>
        [ResponseType(typeof(void))]
        [HttpDelete]
        [Route("{companyId:int}/ShippingAccounts/{shippingAccountId:int}", Name = "DeleteShippingAccount")]
        public IHttpActionResult DeleteShippingAccount([Required] int companyId, [Required] int shippingAccountId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var accountInDatabase = shippingAccountService.FindById(shippingAccountId);

            if (accountInDatabase.CompanyId != companyId)
                return NotFound();

            using (var tran = AsyncTransactionScope.StartNew())
            {
                shippingAccountService.Delete(accountInDatabase);

                tran.Complete();
            }
            return Ok();
        }

        /// <summary>
        /// Delete a company which id = {id}
        /// </summary>
        /// <param name="id">The id of the company.</param>
        [ResponseType(typeof(void))]
        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult DeleteCompany(int id)
        {
            Company company = companyService.FindCompanyById(id);
            if (company == null)
                return NotFound();

            companyService.RemoveCompany(company);

            return Ok();
        }

        ////////////////////////////////////

        [HttpPost]
        [Route("{companyId:int}/onBoarding/vendor/{userId?}", Name = "PostOnBoardingVendorFiles")]
        public async Task<IHttpActionResult> PostOnBoardingVendorFiles([Required] int companyId, string userId = null)
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (companyId <= 0)
                return BadRequest("Invalid CompanyID.");

            var company = companyService.FindCompanyById(companyId);
            if (company == null)
                return BadRequest("Invalid CompanyID.");

            var formDataDic = HttpContext.Current.Request.Form.ToDictionary();
            if (!formDataDic.ContainsKey("invitationMessage"))
                return BadRequest("Invalid invitationMessage.");



            List<HttpPostedFileBase> allFiles = null;
            try
            {
                allFiles = GetPostedFiles(homeBL, filesAreRequired: false, overwrite: true);
                if (allFiles == null || allFiles?.Count == 0)
                {
                    Log.Warning("Ignoring a onBoarding with a file.");
                    return StatusCode(HttpStatusCode.NoContent);
                }
                if (allFiles?.Count != 1)
                    return BadRequest("Upload just one file to process.");

                var companyAdminUser = CompanyBL.GetAdminUser(companyId);
                if (userContext?.User == null && companyAdminUser == null)
                    return BadRequest("No User found to the the inviter for the Invitations");

                var inviterUser = userContext?.User ?? new UserInfo(companyAdminUser.UserId, $"{companyAdminUser.FirstName} {companyAdminUser.LastName}".TrimAll(), companyAdminUser.Email);
                var inviterCompany = userContext?.Company ?? new CompanyInfo(companyId, company.Name, company.CompanyType ?? CompanyType.None, company.isEnterprise);

                var file = allFiles.First();
                var invitationMessage = formDataDic.GetOrNull("invitationMessage");

                string inputFileName = Path.GetFileName(file.FileName);
                string inputFilePath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath(@"~/Docs/"), inputFileName);

                var needCheckPartnerLimits = formDataDic.ContainsKey("needCheckPartnerLimits") == true ? bool.Parse(formDataDic["needCheckPartnerLimits"]) : (bool?)null;
                await OnBoardingApiBL.CompanyImport(company, inputFilePath, invitationMessage, inviterUser, inviterCompany, needCheckPartnerLimits);

                return Ok();
            }
            catch (ValidationException ve)
            {
                Log.Warning(ve, "Validation error in Company onBoarding: {Message}", ve.Message);
                return BadRequest(ve.Value == null ? ve.Message : $"{ve.Message}:{ve.Value}");
            }
            finally
            {
                DeleteUploadFiles(allFiles);
            }
        }

        [HttpPost]
        [Route("{companyId:int}/onBoarding/partList/{userId?}", Name = "PostOnBoardingPartListFiles")]
        public async Task<IHttpActionResult> PostOnBoardingPartListFiles([Required] int companyId, string userId = null)
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var company = companyService.FindCompanyById(companyId);
            if (company == null)
                return BadRequest("Invalid CompanyID.");

            var companyAdminUser = CompanyBL.GetAdminUser(companyId);
            if (userContext?.User == null && companyAdminUser == null)
                return BadRequest("No User found");

            List<HttpPostedFileBase> allFiles = null;
            try
            {
                allFiles = GetPostedFiles(homeBL, filesAreRequired: false, overwrite: true);
                if (allFiles == null || allFiles?.Count == 0)
                {
                    Log.Warning("Ignoring a onBoarding with a file.");
                    return StatusCode(HttpStatusCode.NoContent);
                }
                if (allFiles?.Count != 1)
                    return BadRequest("Upload just one file to process.");


                var file = allFiles.First();

                string inputFileName = Path.GetFileName(file.FileName);
                string inputFilePath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath(@"~/Docs/"), inputFileName);


                var hasDupe = await OnBoardingApiBL.PartsImport(company, inputFilePath);
                if (hasDupe == true)
                {
                    return BadRequest("Warning: You tried to import part which is already existing in the system, skipped duplicating parts.");
                }

                return Ok("Success");
            }
            catch (ValidationException ve)
            {
                Log.Warning(ve, "Validation error in part onBoarding: {Message}", ve.Message);
                return BadRequest(ve.Message);
            }
            finally
            {
                DeleteUploadFiles(allFiles);
            }
        }


        /// <summary>
        /// Get a customer's vendors
        /// </summary>
        /// <param name="companyId">Company ID.</param>
        /// <param name="filter">Filter products into first-time order or reorder or non-filter</param>
        /// <param name="type">User type</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Numbe of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        [HttpGet]
        [Route("{companyId:int}/customer/vendors", Name = "GetCustomerVendors")]
        public async Task<PagedResultSet<CompanyDTO>> GetCustomerVendors(int companyId, TaskDataQuery.ProductFilter filter = TaskDataQuery.ProductFilter.All,
                     TaskDataQuery.UserType type = TaskDataQuery.UserType.Customer, int? page = 1, int pageSize = PageSize,
                     string orderBy = nameof(ProductDTO.Id), bool ascending = false)
        {
            var tasks = taskDataService.FindTaskDatasByCustomerId(companyId)
                .FilterBy(filter, type);

            var vendors = tasks
                .Select(x => x.Product.VendorCompany)
                .Where(x => x != null)
                .GroupBy(g => g.Id)
                .Select(x => x.FirstOrDefault());

            var dto = await PageOfResultsSetAsync<Company, CompanyDTO>(vendors, page, pageSize, orderBy, ascending, "GetCustomerVendors");
            dto.Results = homeBL.GetUserPerformanceWrapper(dto.Results, CompanyType.Vendor, companyId);

            return dto;
        }

        /// <summary>
        /// Get a vendor's customer
        /// </summary>
        /// <param name="companyId">Company ID.</param>
        /// <param name="filter">Filter products into first-time order or reorder or non-filter</param>
        /// <param name="type">User type</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Numbe of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        [HttpGet]
        [Route("{companyId:int}/vendor/customers", Name = "GetVendorCustomers")]
        public async Task<PagedResultSet<CompanyDTO>> GetVendorCustomers(int companyId, TaskDataQuery.ProductFilter filter = TaskDataQuery.ProductFilter.All,
                     TaskDataQuery.UserType type = TaskDataQuery.UserType.Customer, int? page = 1, int pageSize = PageSize,
                     string orderBy = nameof(ProductDTO.Id), bool ascending = false)
        {
            var tasks = taskDataService.FindTaskDatasByVendorId(companyId).FilterBy(filter, type);
            var customers = tasks.Select(x => x.Product.CustomerCompany).GroupBy(g => g.Id).Select(x => x.FirstOrDefault());
            var dto = await PageOfResultsSetAsync<Company, CompanyDTO>(customers, page, pageSize, orderBy, ascending, "GetVendorCustomers");
            dto.Results = dto.Results.Where(x => !string.IsNullOrEmpty(x.Email));
            dto.Results = homeBL.GetUserPerformanceWrapper(dto.Results, CompanyType.Customer, companyId);

            return dto;
        }

#if true
        /// <summary>
        /// Get a company's Quality Analytics performance
        /// </summary>
        /// <param name="companyId">Company ID.</param>
        /// <param name="productId">Product ID.</param>
        /// <param name="mode">Company is a Customer or Vendor or Sharer</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Numbe of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        [HttpGet]
        [Route("{companyId:int}/companyqa", Name = "GetCompanyQA")]
        public async Task<PagedResultSet<CompanyDTO>> GetCompanyQA(int companyId, int? productId = null,
                                                                    UserMode mode = UserMode.Customer,
                                                                    int? page = 1, int pageSize = PageSize,
                                                                    string orderBy = nameof(CompanyDTO.Id), bool ascending = false)
        {
            var orders = (mode == UserMode.Vendor ? orderService.FindOrdersByVendorId(companyId) : orderService.FindOrdersByCustomerId(companyId))
                .Where(x => productId != null ? x.ProductId == productId : true);

            var ncrs = (mode == UserMode.Vendor ? ncReportService.FindNCReportByVendorId(companyId) : ncReportService.FindNCReportByCustomerId(companyId))
                .Where(x => productId != null ? x.ProductId == productId : true);

            if (mode == UserMode.Sharer)
            {
                orders = orders.Where(x => x.ProductSharingId != null && x.ProductSharing.OwnerCompanyId == companyId);
                ncrs = ncrs.Where(x => x.Order.ProductSharingId != null && x.Order.ProductSharing.OwnerCompanyId == companyId);
            }

            IQueryable<Company> companies = Enumerable.Empty<Company>().AsQueryable();
            if (mode == UserMode.Vendor)
            {
                companies = orders
                       .Select(x => x.CustomerId != null ? x.Customer : x.Product.CustomerCompany)
                       .GroupBy(x => x.Id)
                       .Select(g => g.FirstOrDefault());
            }
            else
            {
                companies = orders
                    .Select(x => x.Product.VendorCompany)
                    .GroupBy(x => x.Id)
                    .Select(g => g.FirstOrDefault());
            }


            var dto = await PageOfResultsSetAsync<Company, CompanyDTO>(companies, page, pageSize, orderBy, ascending, "GetCompanyQA");
            dto.Results = homeBL.GetUserPerformanceByProductIdWrapper(dto.Results, orders, ncrs);

            return dto;
        }
#endif

        /// <summary>
        /// Get a company's Quality Analytics performance
        /// </summary>
        /// <param name="companyId">Company ID.</param>
        /// <param name="productId">Product ID.</param>
        /// <param name="vendorId">Vendor ID.</param>
        /// <param name="isSummarize">Is for Summarize only?</param>
        /// <param name="mode">Company is a Customer or Vendor or Sharer</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Numbe of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        [HttpGet]
        [Route("{companyId:int}/qualityanalytics", Name = "GetQualityAnalytics")]
        public async Task<QualityAnalyticsDto> GetQualityAnalytics(int companyId,
                                                                    bool isSummarize,
                                                                    int? productId = null,
                                                                    int? vendorId = null,
                                                                    UserMode mode = UserMode.Customer,
                                                                    int? page = 1, int pageSize = PageSize,
                                                                    string orderBy = nameof(CompanyDTO.Id), bool ascending = false)
        {
            QualityAnalyticsDto qa = new QualityAnalyticsDto();
            if (isSummarize)
            {
                var orders = (mode == UserMode.Vendor ? orderService.FindOrdersByVendorId(companyId) : orderService.FindOrdersByCustomerId(companyId))
                .Where(x => productId != null ? x.ProductId == productId : true)
                .Where(x => vendorId != null ? x.Product.VendorId == vendorId : true);

                var ncrs = (mode == UserMode.Vendor ? ncReportService.FindNCReportByVendorId(companyId) : ncReportService.FindNCReportByCustomerId(companyId))
                    .Where(x => productId != null ? x.ProductId == productId : true)
                    .Where(x => vendorId != null ? x.VendorId == vendorId : true);

                if (mode == UserMode.Sharer)
                {
                    orders = orders.Where(x => x.ProductSharingId != null && x.ProductSharing.OwnerCompanyId == companyId);
                    ncrs = ncrs.Where(x => x.Order.ProductSharingId != null && x.Order.ProductSharing.OwnerCompanyId == companyId);
                }

                IQueryable<Company> companies = Enumerable.Empty<Company>().AsQueryable();
                if (mode == UserMode.Vendor)
                {
                    companies = orders
                           .Select(x => x.CustomerId != null ? x.Customer : x.Product.CustomerCompany)
                           .GroupBy(x => x.Id)
                           .Select(g => g.FirstOrDefault());
                }
                else
                {
                    companies = orders
                        .Select(x => x.Product.VendorCompany)
                        .GroupBy(x => x.Id)
                        .Select(g => g.FirstOrDefault());
                }
                var dto = await PageOfResultsSetAsync<Company, CompanyDTO>(companies, page, pageSize, orderBy, ascending, "GetQualityAnalytics");
                dto.Results = homeBL.GetUserPerformanceByProductIdWrapper(dto.Results, orders, ncrs);
                qa.Companies = dto;
            }
            else
            {
                qa.ChartData = chartBL.GetChartData(companyId, mode, NCR_CHART_FILTERS.Product, productId, vendorId);
            }
            return qa;
        }


        /// <summary>
        /// Get a company's Quality Analytics Statistics
        /// </summary>
        /// <param name="companyId">Company ID.</param>
        /// <param name="StartDate">Start Date.</param>
        /// <param name="EndDate">End Date</param>
        /// <param name="mode">Company is a Customer or Vendor or Sharer</param>    
        [HttpGet]
        [Route("{companyId:int}/GetCompanyQAStatistics", Name = "GetCompanyQAStatistics")]
        public async Task<CompanyQualityAnalyticsStatisticsViewModel> GetCompanyQAStatistics([FromUri] int companyId,
                                                                                              [Required] DateTime StartDate,
                                                                                              DateTime? EndDate = null,
                                                                                              UserMode mode = UserMode.Customer)
        {
            if (EndDate == null) EndDate = DateTime.UtcNow;

            var orders = (mode == UserMode.Vendor ? orderService.FindOrdersByVendorId(companyId) : orderService.FindOrdersByCustomerId(companyId))
                    .Where(x => x.TaskData.StateId == (int)States.ProductionComplete && x.OrderDate >= StartDate && x.OrderDate <= EndDate);

            var ncrs = (mode == UserMode.Vendor ? ncReportService.FindNCReportByVendorId(companyId) : ncReportService.FindNCReportByCustomerId(companyId))
                    .Where(x => x.StateId != States.NCRClosed && x._createdAt >= StartDate && x._createdAt <= EndDate);

            if (orders.Count() == 0)
            {
                return new CompanyQualityAnalyticsStatisticsViewModel();
            }
            var result = await homeBL.GetCompanyQualityAnalyticsStatistics(orders, ncrs, companyId, mode);
            return result;

        }

        /// <summary>
        /// Get a company's Completion Statistics
        /// </summary>
        /// <param name="companyId">Company ID.</param>
        /// <param name="StartDate">Start Date.</param>
        /// <param name="EndDate">End Date</param>
        /// <param name="mode">Company is a Customer or Vendor or Sharer</param>    
        [HttpGet]
        [Route("{companyId:int}/GetCompletionStatistics", Name = "GetCompletionStatistics")]
        public async Task<CompanyCompletionStatistics> GetCompletionStatistics( [FromUri] int companyId,
                                                                                [Required] DateTime StartDate,
                                                                                DateTime? EndDate = null,
                                                                                UserMode mode = UserMode.Customer)
        {
            if (EndDate == null) EndDate = DateTime.UtcNow;

            IQueryable<TaskData> tasks = Enumerable.Empty<TaskData>().AsQueryable();
            if (mode == UserMode.Customer)
            {
                tasks = taskDataService.FindTaskDatasByCustomerId(companyId);
            }
            else
            {
                tasks = taskDataService.FindTaskDatasByVendorId(companyId);
            }

            tasks = tasks
                .Where(x => x.StateId == (int) States.QuoteAccepted && x.ModifiedUtc >= StartDate && x.ModifiedUtc <= EndDate)
                .GroupBy(x => x.ProductId)
                .Select(x => x.FirstOrDefault());


            var completedRFQsCount = tasks.Count();

            var completedNCRsCount = (mode == UserMode.Vendor ? ncReportService.FindNCReportByVendorId(companyId) : ncReportService.FindNCReportByCustomerId(companyId))
                .Where(x => x.StateId == States.NCRClosed && x._updatedAt >= StartDate && x._updatedAt <= EndDate)
                .Count();

            if (tasks.Count() == 0)
            {
                return new CompanyCompletionStatistics();
            }

            var stats = new CompanyCompletionStatistics
            {
                CompletedRFQsCount = completedRFQsCount,
                CompletedNCRsCount = completedNCRsCount,
            };
            return await Task.FromResult(stats);
        }



        /// <summary>
        /// Create/Update bank info for a company
        /// </summary>
        /// <returns>The new/updated bank info regarding current user</returns>
        [HttpPost]
        [Route("user/{userId}/CreateCompanyBankInfo", Name = "CreateCompanyBankInfo")]
        [ResponseType(typeof(CompanyBankInfoDTO))]
        public IHttpActionResult CreateCompanyBankInfo()
        {
            if (userContext.User == null || userContext.Company == null)
            {
                return BadRequest("Invalid User or Customer");
            }
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var httpRequest = HttpContext.Current.Request;
            var formDataDic = httpRequest.Form.ToDictionaryOfObjects().ClearEmptyEntries();
            var model = Slapper.AutoMapper.Map<CompanyBankInfoViewModel>(formDataDic, false);
            using (var tran = AsyncTransactionScope.StartNew())
            {
                var companyId = userContext.Company.Id;
                var company = companyService.FindCompanyById(companyId);
                CompanyBankInfo companyBankInfo = null;
                if (company.CompanyBankInfoId == null)
                {
                    companyBankInfo = CompanyBL.CreateCompanyBankInfo(model);
                    company.CompanyBankInfoId = companyBankInfo.Id;
                    companyService.UpdateCompany(company);
                }
                else
                {
                    companyBankInfo = CompanyBL.UpdateCompanyBankInfo(company.CompanyBankInfoId.Value, model);
                }
                var resultDTO = Mapper.Map<CompanyBankInfoDTO>(companyBankInfo);
                tran.Complete();
                return CreatedAtRoute("CreateCompanyBankInfo", new { companyId = companyId }, resultDTO);
            }
        }


        /// <summary>
        /// Get bank info for a company
        /// </summary>
        /// <param name="companyId">Company ID which owns this Bank Info</param>
        /// <returns>CompanyBankInfoDTO</returns>
        [HttpGet]
        [Route("GetCompanyBankInfo/{companyId:int}", Name = "GetCompanyBankInfo")]
        [ResponseType(typeof(CompanyBankInfoDTO))]
        public CompanyBankInfoDTO GetCompanyBankInfo(int companyId)
        {
            var company = companyService.FindCompanyById(companyId);
            CompanyBankInfoDTO dto = new CompanyBankInfoDTO();
            if (company != null && company.CompanyBankInfoId != null)
            {
                dto = Mapper.Map<CompanyBankInfoDTO>(company.CompanyBankInfo);
            }
            return dto;
        }

        /// <summary>
        /// Get Country List
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        /// <returns>CountryDTO</returns>
        [HttpGet]
        [Route("GetCountryList", Name = "GetCountryList")]
        [ResponseType(typeof(Country))]
        public async Task<PagedResultSet<Country>> GetCountryList(int? page = 1, int pageSize = PageSize, string orderBy = nameof(CompanyDTO.Id), bool ascending = false)
        {
            var countryList = countryService.FindAllCountryList();
            return await PageOfResultsSetAsync<Country>(countryList, page, pageSize, orderBy, ascending, "GetCountryList");
        }

        /// <summary>
        /// Get State or Province List
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        /// <returns>CountryDTO</returns>
        [HttpGet]
        [Route("GetStateProvinceList", Name = "GetStateProvinceList")]
        [ResponseType(typeof(StateProvinceDTO))]
        public async Task<PagedResultSet<StateProvinceDTO>> GetStateProvinceList(int? page = 1, int pageSize = PageSize, string orderBy = nameof(CompanyDTO.Id), bool ascending = false)
        {
            var stateProvinceList = StateProvinceService.FindStateProvinceList();
            return await PageOfResultsSetAsync<StateProvince, StateProvinceDTO>(stateProvinceList, page, pageSize, orderBy, ascending, "GetStateProvinceList");
        }

        /// <summary>
        /// Get a list of partners for a company
        /// </summary>
        /// <param name="companyId">Company id.</param>
        /// <param name="page">Page number</param>
        /// <param name="type">Partner type</param>
        /// <param name="search">Search for company by name/email</param>
        /// <param name="pageSize">Numbe of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        [HttpGet]
        [Route("{companyId:int}/partners", Name = "GetCompanyPartners")]
        public async Task<PagedResultSet<CompanyDTO>> GetCompanyPartners(int companyId,
                                                                            PARTNER_TYPE type = PARTNER_TYPE.All,
                                                                            string search = null,
                                                                            int? page = 1, int pageSize = 10,
                                                                            string orderBy = nameof(CompanyDTO.Id),
                                                                            bool ascending = false)
        {
            var result = productBL.GetCompanyParnters(companyId, type, search);
            return await PageOfResultsSetAsync<Company, CompanyDTO>(result, page, pageSize, orderBy, ascending, "GetCompanyPartners");
        }

        /// <summary>
        /// Get a list of partners for a company along with their QA Stats info
        /// </summary>
        /// <param name="companyId">Company id.</param>
        /// <param name="page">Page number</param>
        /// <param name="type">Partner type</param>
        /// <param name="search">Search for company by name/email</param>
        /// <param name="pageSize">Numbe of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        [HttpGet]
        [Route("{companyId:int}/GetCompanyPartnersWithQAStats", Name = "GetCompanyPartnersWithQAStats")]
        public PagedResultSet<VendorStatsViewModel> GetCompanyPartnersWithQAStats(int companyId,
                                                                                    PARTNER_TYPE type = PARTNER_TYPE.All,
                                                                                    string search = null,
                                                                                    int? page = 1, int pageSize = 10,
                                                                                    string orderBy = nameof(VendorStatsViewModel.CompletedOrders),
                                                                                    bool ascending = false)
        {
            var result = productBL.GetCompanyParntersWithQAStats(companyId, type, search);
            return PageOfResultsSet<VendorStatsViewModel>(result, page, pageSize, orderBy, ascending, "GetCompanyPartnersWithQAStats");
        }

        /// <summary>
        /// Add company as customer to QBO of Omnae account
        /// </summary>
        /// <param name="vm">Type of CustomerInfoViewModel</param>
        [HttpPost]
        [Route("AddCompanyToQBO", Name = "AddCompanyToQBO")]
        public async Task<IHttpActionResult> AddCompanyToQBO([FromBody] CustomerInfoViewModel vm)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using (var trans = AsyncTransactionScope.StartNew())
            {
                try
                {
                    await homeBL.CreateUserInQBO(vm);

                    trans.Complete();
                    return Ok("Success");
                }
                catch (ValidationException ex)
                {
                    return BadRequest(ex.RetrieveErrorMessage());
                }
            }
        }


        /// <summary>
        /// Get customer info for which to be added into QBO
        /// </summary>
        /// <param name="companyId">Company ID</param>
        [HttpGet]
        [Route("GetCustomerInfo/{companyId:int}", Name = "GetCustomerInfo")]
        public async Task<CustomerInfo> GetCustomerInfo(int companyId)
        {
            var company = companyService.FindCompanyById(companyId);
            if (company == null)
                throw new Exception(IndicatingMessages.CompanyNotFound);

            CustomerInfo customerInfo = await Task.FromResult(homeBL.CreateUsersForQBO(companyId));
            return customerInfo;
        }


        /// <summary>
        /// Get customer/vendor info for which to be added into QBO
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <param name="vendorId">Vendor ID</param>
        /// <param name="isVendor">is for creating a vendor in QBO?</param>
        [HttpGet]
        [Route("GetCompanyInfo", Name = "GetCompanyInfo")]
        public async Task<CustomerInfo> GetCompanyInfo(int customerId, int vendorId, bool isVendor)
        {
            var companyId = isVendor ? vendorId : customerId;
            var company = companyService.FindCompanyById(companyId);
            if (company == null)
                throw new Exception(IndicatingMessages.CompanyNotFound);

            // Get currency from [CompaniesCreditRelationships] table 
            var relation = companiesCreditRelationshipService.FindCompaniesCreditRelationshipByCustomerIdVendorId(customerId, vendorId);
            var currencyCode = relation?.Currency ?? company.Currency;
            var termDays = relation?.TermDays ?? 0;
            CustomerInfo customerInfo = await Task.FromResult(homeBL.CreateUsersForQBO(companyId, currencyCode, termDays));
            return customerInfo;
        }

        /// <summary>
        /// Get credit relationship records by customerId
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        [HttpGet]
        [Route("GetCustomerCurrencies/{customerId:int}", Name = "GetCustomerCurrencies")]
        public async Task<List<RelationshipCurrencyDto>> GetCustomerCurrencies([FromUri] int customerId)
        {
            var list = companiesCreditRelationshipService.FindCompaniesCreditRelationshipsByCustomerId(customerId);
            List<RelationshipCurrencyDto> dtoList = new List<RelationshipCurrencyDto>();
            foreach (var item in list)
            {
                var dto = new RelationshipCurrencyDto
                {
                    CustomerId = item.CustomerId,
                    VendorId = item.VendorId,
                    Currency = Enum.GetName(typeof(CurrencyCodes), item.Currency),
                };
                dtoList.Add(dto);
            }
            return await Task.FromResult(dtoList);
        }

        /// <summary>
        /// Update credit relationship currency between customer and vendor
        /// </summary>
        /// <param name="dto">RelationshipCurrencyDto</param>
        [HttpPost]
        [Route("UpdateCreditRelationshipCurrency", Name = "UpdateCreditRelationshipCurrency")]
        public async Task UpdateCreditRelationshipCurrency(RelationshipCurrencyDto dto)
        {
            int cid = dto.CustomerId;
            int vid = dto.VendorId;  
            var ent = companiesCreditRelationshipService.FindCompaniesCreditRelationshipByCustomerIdVendorId(cid, vid);
            var cucy = dto.Currency.ToEnum<CurrencyCodes>(); // "CAD" -> 124
            try
            {
                if (ent != null)
                {
                    ent.Currency = cucy;
                    companiesCreditRelationshipService.UpdateCompaniesCreditRelationship(ent);
                }
                else
                {
                    // create a new relationship in CompaniesCreditRelationships table
                    ent = new CompaniesCreditRelationship
                    {
                        CustomerId = cid,
                        VendorId = vid,
                        isTerm = false,
                        Currency = cucy,
                    };
                    companiesCreditRelationshipService.AddCompaniesCreditRelationship(ent);
                    
                }
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                await Task.FromException(ex);
            }
        }



        ////////////////////////////////////

        private bool CompanyExists(int id)
        {
            return companyService.FindCompanyById(id) != null;
        }
    }
}