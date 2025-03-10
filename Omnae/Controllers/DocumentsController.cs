using AutoMapper;
using Microsoft.Azure;
using Omnae.BlobStorage;
using Omnae.Common;
using Omnae.Model.Models;
using Omnae.Service.Service.Interfaces;
using Omnae.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Azure;
using Common;
using Libs.Notification;
using Omnae.BusinessLayer;
using Omnae.BusinessLayer.Identity.Model;
using Omnae.BusinessLayer.Services;
using Omnae.Context;
using Omnae.Service.Service;
using Omnae.BusinessLayer.Models;
using Omnae.Libs.Notification;
using Omnae.Model.Context;

namespace Omnae.Controllers
{
    public class DocumentsController : BaseController
    {
        private DocumentBL DocumentBl { get; }

        public DocumentsController(IRFQBidService rfqBidService, ICompanyService companyService, ITaskDataService taskDataService, IPriceBreakService priceBreakService, IOrderService orderService, ILogedUserContext userContext, IProductService productService, IDocumentService documentService, IShippingService shippingService, ICountryService countryService, IAddressService addressService, IStateProvinceService stateProvinceService, IOrderStateTrackingService orderStateTrackingService, IProductStateTrackingService productStateTrackingService, IPartRevisionService partRevisionService, IStoredProcedureService spService, INCReportService ncReportService, IRFQQuantityService rfqQuantityService, IExtraQuantityService extraQuantityService, IBidRequestRevisionService bidRequestRevisionService, ITimerSetupService timerSetupService, IQboTokensService qboTokensService, IOmnaeInvoiceService omnaeInvoiceService, INCRImagesService ncrImagesService, IApprovedCapabilityService approvedCapabilityService, IShippingAccountService shippingAccountService, ApplicationDbContext dbUser, ProductBL productBl, NotificationService notificationService, UserContactService userContactService, TimerTriggerService timerTriggerService, NotificationBL notificationBl, PaymentBL paymentBl, ShipmentBL shipmentBl, ChartBL chartBl, IMapper mapper, NcrBL ncrBL, IDocumentStorageService documentStorageService, IImageStorageService imageStorageService, DocumentBL documentBl) : base(rfqBidService, companyService, taskDataService, priceBreakService, orderService, userContext, productService, documentService, shippingService, countryService, addressService, stateProvinceService, orderStateTrackingService, productStateTrackingService, partRevisionService, spService, ncReportService, rfqQuantityService, extraQuantityService, bidRequestRevisionService, timerSetupService, qboTokensService, omnaeInvoiceService, ncrImagesService, approvedCapabilityService, shippingAccountService, dbUser, productBl, notificationService, userContactService, timerTriggerService, notificationBl, paymentBl, shipmentBl, chartBl, mapper, ncrBL, documentStorageService, imageStorageService)
        {
            DocumentBl = documentBl;
        }

        // GET: Documents
        public ActionResult Index()
        {
            var documents = DocumentService.FindDocumentList();
            return View(documents);
        }

        // GET: Documents/Details/5
        public ActionResult Details(int id)
        {
            Document document = DocumentService.FindDocumentById(id);
            if (document == null)
            {
                HandleErrorInfo info = new HandleErrorInfo(new Exception("Document with id: " + id + " was not found"), "Documents", "Details");
                return View("Error", info);
            }
            return View(document);
        }

        // GET: Documents/Create
        public ActionResult Create(int? id, int? taskId)
        {
            if (id == null || taskId == null)
            {
                TempData["ErrorMessage"] = "Product Id or Task Id is null";
                return View();
            }

            var model = new DocumentProdIdViewModel
            {
                ProductId = id.Value,
                TaskId = taskId.Value,
            };
            var docs = DocumentService.FindDocumentByProductId(id.Value).Where(d => d.DocType == (int)DOCUMENT_TYPE.PRODUCT_2D_PDF || d.DocType == (int)DOCUMENT_TYPE.PRODUCT_3D_STEP).ToList();
            if (docs.Any())
            {
                model.Documents = new List<Document>();
                foreach (var doc in docs)
                {
                    model.Documents.Add(doc);
                }
            }
            return View(model);
        }
        
        // POST: Documents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DocumentProdIdViewModel model)
        {
            if (!ModelState.IsValid)
                return View();
            
            if (Request.Files == null || Request.Files.Count == 0 || Request.Files[0] == null)
            {
                TempData["ErrorMessage"] = "Please select a file to upload!";
                return RedirectToAction("Create", "Documents", new { @id = model.ProductId, @taskId = model.TaskId });
            }

            // upload product docs

            List<HttpPostedFileBase> fileBases = new List<HttpPostedFileBase>();
            for (int i = 0; i < Request.Files.Count; i++)
            {
                fileBases.Add(Request.Files[i]);
            }

            string result = DocumentBl.UploadProductDocs(fileBases, model.ProductId, model.TaskId);
            if (result != null)
            {
                TempData["ErrorMessage"] = result;
                return RedirectToAction("Create", "Documents", new { @id = model.ProductId, @taskId = model.TaskId });
            }

            return RedirectToAction("Index", "Home");
        }
        
        [HttpPost]
        public ActionResult UploadProofingDoc()
        {
            if (Request.Files == null || Request.Files.Count == 0 || Request.Files[0] == null)
            {
                var info = new HandleErrorInfo(new Exception("Please select a file to upload!"), "Documents", "UploadProofingDoc");
                return View("Error", info);
            }

            int productId = int.Parse(Request.Params["productId"]);
            int taskId = int.Parse(Request.Params["taskId"]);
            List<HttpPostedFileBase> fileBases = new List<HttpPostedFileBase>();
            for (int i = 0; i < Request.Files.Count; i++)
            {
                fileBases.Add(Request.Files[i]);
            }
            try
            {
                var result = DocumentBl.UploadProofingDoc(fileBases, productId, taskId);
                if (!result)
                {
                    Json("already exists");
                }
            }
            catch (RequestFailedException ex)
            {
                TempData["AzureException"] = ex.RetrieveErrorMessage();
                return RedirectToAction("UploadProofingDoc", "Documents", new { id = productId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.RetrieveErrorMessage();
                return RedirectToAction("Index", "Home");
            }
            return Json("success");
        }

        // GET: Documents/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            var document = DocumentService.FindDocumentById(id.Value);
            if (document == null)
                return HttpNotFound();
            
            return View(document);
        }
        
        [HttpGet]
        public ActionResult Delete(int id, string actionName, string controllerName)
        {
            var document = DocumentBl.Delete(id);

            return RedirectToAction(actionName, controllerName, new { @id = document.ProductId, @taskId = document.TaskId });
        }

        public ActionResult DeleteQuoteDoc(int id)
        {
            DocumentBl.Delete(id);

            var model = TempData["StateTransitionViewModel"] as StateTransitionViewModel;
            model.RFQVM.QuoteDoc = null;
            ModelState.Clear();
            return PartialView("_BidRFQ", model);
        }


        
    }
}
