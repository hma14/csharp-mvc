using Microsoft.AspNet.Identity;
using Omnae.BlobStorage;
using Omnae.Model.Models;
using Omnae.Models;
using Omnae.Service.Service.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Omnae.Context;

namespace Omnae.Controllers
{
    public class ShippingsController : BaseController
    {
        public ShippingsController(IProductService productService, ICompanyService companyService, IDocumentService documentService, IBlobStorageService blobStorageService, ITaskDataService taskDataService, IPriceBreakService priceBreakService, IOrderService orderService, IShippingService shippingService, ICountryService countryService, IAddressService addressService, IStateProvinceService stateProvinceService, IOrderStateTrackingService orderStateTrackingService, IProductStateTrackingService productStateTrackingService, IPartRevisionService partRevisionService, INCReportService ncreportService, IRFQBidService rfqbidService, IRFQQuantityService rfqquantityService, IExtraQuantityService extraquantityService, IStoredProcedureService spService, IBidRequestRevisionService bidRequestRevisionService, ITimerSetupService timerSetupService, IQboTokensService qboTokensService, IOmnaeInvoiceService omnaeInvoiceService, INCRImagesService nCRImagesService, ApplicationDbContext dbUser, LogedUserContext logedUserContext, IShippingAccountService shippingAccountService) : base(productService, companyService, documentService, blobStorageService, taskDataService, priceBreakService, orderService, shippingService, countryService, addressService, stateProvinceService, orderStateTrackingService, productStateTrackingService, partRevisionService, ncreportService, rfqbidService, rfqquantityService, extraquantityService, spService, bidRequestRevisionService, timerSetupService, qboTokensService, omnaeInvoiceService, nCRImagesService, dbUser, logedUserContext, shippingAccountService)
        {
        }

        // GET: Shippings
        public ActionResult Index()
        {
            var currentUserId = User.Identity.GetUserId();
            var company = companyService.FindCompanyByUserId(currentUserId);
            var shippings = shippingService.FindShippingByUserId(company.Id);
            return View(shippings);
        }
   

        // GET: Shippings/Create
        public ActionResult Create()
        {
            var customerUsers = dbUser.Users.Where(x => x.UserType == Common.USER_TYPE.Customer);
            List<Company> CustomerCompanies = new List<Company>();
            foreach(var user in customerUsers)
            {
                var company = companyService.FindCompanyByUserId(user.Id);
                CustomerCompanies.Add(company);
            }
            ViewBag.Customers = new SelectList(CustomerCompanies, "Id", "Name");

            var vendorUsers = dbUser.Users.Where(x => x.UserType == Common.USER_TYPE.Vendor);
            List<Company> VendorCompanies = new List<Company>();
            foreach (var user in vendorUsers)
            {
                var company = companyService.FindCompanyByUserId(user.Id);
                VendorCompanies.Add(company);
            }
            ViewBag.Vendors = new SelectList(VendorCompanies, "Id", "Name");


            return View();
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                shippingService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
