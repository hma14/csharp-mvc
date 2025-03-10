using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AutoMapper;
using Omnae.BusinessLayer.Models;
using Omnae.Common;
using Omnae.Model.Context;
using Omnae.Model.Models;
using Omnae.Service.Service;
using Omnae.Service.Service.Interfaces;

namespace Omnae.BusinessLayer
{
    public class TaskSetup
    {
        private ILogedUserContext UserContext { get; }
        private ITaskDataService TaskDataService { get; }
        private OrderService OrderService { get; }
        private OmnaeInvoiceService OmnaeInvoiceService { get; }
        private DocumentService DocumentService { get; }

        private ProductService ProductService { get; }
        private PriceBreakService PriceBreakService { get; }
        private NCReportService NCReportService { get; }
        private CountryService CountryService { get; }
        private RFQBidService RFQBidService { get; }

        private ExtraQuantityService ExtraQuantityService { get; }
        private PartRevisionService PartRevisionService { get; }
        private AddressService AddressService { get; }
        private ShippingService ShippingService { get; }
        private CompanyService CompanyService { get; }

        private RFQQuantityService RFQQuantityService { get; }
        private NCRImagesService NCRImagesService { get; }

        private IMapper Mapper { get; }

        public TaskSetup(ILogedUserContext userContext, ITaskDataService taskDataService, OrderService orderService, OmnaeInvoiceService omnaeInvoiceService, DocumentService documentService, ProductService productService, PriceBreakService priceBreakService, NCReportService ncReportService, CountryService countryService, RFQBidService rfqBidService, ExtraQuantityService extraQuantityService, PartRevisionService partRevisionService, AddressService addressService, ShippingService shippingService, CompanyService companyService, RFQQuantityService rfqQuantityService, NCRImagesService ncrImagesService, IMapper mapper)
        {
            UserContext = userContext;
            TaskDataService = taskDataService;
            OrderService = orderService;
            OmnaeInvoiceService = omnaeInvoiceService;
            DocumentService = documentService;
            ProductService = productService;
            PriceBreakService = priceBreakService;
            NCReportService = ncReportService;
            CountryService = countryService;
            RFQBidService = rfqBidService;
            ExtraQuantityService = extraQuantityService;
            PartRevisionService = partRevisionService;
            AddressService = addressService;
            ShippingService = shippingService;
            CompanyService = companyService;
            RFQQuantityService = rfqQuantityService;
            NCRImagesService = ncrImagesService;
            this.Mapper = mapper;
        }

        public NcrDescriptionViewModel SetupNcrDescriptionViewModel(TaskData td)
        {        
            var ncr = NCReportService.FindNCReportByTaskId(td.TaskId);
            if (ncr == null)
            {
                return null;
            }

            var product = td.Product;
            string customerName = product.CustomerCompany.Name;
            string vendorName = string.Empty;
            if (product.VendorCompany != null)
            {
                vendorName = product.VendorCompany.Name;
            }
            else
            {
                vendorName = CompanyService.FindCompanyById(ncr.VendorId.Value).Name;
            }



            NcrDescriptionViewModel model = Mapper.Map<NcrDescriptionViewModel>(ncr);

            model.NCRId = ncr.Id;
            model.ProductId = td.ProductId.Value;
            model.Customer = customerName;
            model.Vendor = vendorName;
            model.ProductPartNo = product.PartNumber;
            model.PartRevisionNo = product.PartNumberRevision;
            model.ProductDescription = product.Description;
            model.PONumber = ncr.Order?.CustomerPONumber;
            model.TotalProductQuantity = OrderService.FindOrderByCustomerId(product.CustomerId.Value)
                .Where(o => o.ProductId == td.ProductId.Value)
                .Sum(q => q.Quantity);
            SetupNCRImages(ref model, ncr.Id);

            return model;
        }

        public bool CheckPreconditions(int productId, int taskId)
        {
            var task = TaskDataService.FindById(taskId);

            switch (task.StateId)
            {
                case (int)States.ProofingStarted:
                    {
                        var doc = task.Product?.Documents?.Where(x => x.DocType == (int)DOCUMENT_TYPE.PROOF_PDF);
                        return doc.Any();
                    }

                case (int)States.ProductionComplete:
                    {
                        var orders = OrderService.FindOrderByTaskId(task.TaskId);
                        if (orders == null || orders.Count == 0)
                        {
                            return false;
                        }
                        return orders?.First().IsForToolingOnly == true;
                    }
            }

            return false;
        }

        public bool CheckPreconditions(TaskData task)
        {
            Debug.Assert(task.ProductId != null, "task.ProductId != null");

            switch (task.StateId)
            {
                case (int)States.ProofingStarted:
                    {
                        //var doc = documentService.FindDocumentByProductIdDocType(task.ProductId.Value, DOCUMENT_TYPE.PROOF_PDF); 
                        var doc = task.Product?.Documents.ToList();
                        return doc.Any();
                    }

                case (int)States.ProductionComplete:
                    {
                        var orders = task.Orders;
                        if (orders == null || orders.Count == 0)
                        {
                            return false;
                        }
                        return orders.First().IsForToolingOnly == true;
                    }
            }

            return false;
        }

        public List<Document> GetDocumentsByProductIdDocType(int productId, DOCUMENT_TYPE type)
        {
            return DocumentService.FindDocumentByProductIdDocType(productId, type).ToList();
        }

        public NCReport GetNCReport(int productId, int? orderId = null)
        {
            NCReport ncr;
            IEnumerable<NCReport> ncrs = new List<NCReport>();
            if (orderId != null)
            {
                ncrs = NCReportService.FindNCReportByProductIdOrderId(productId, orderId.Value).OrderBy(x => x.Id);
            }
            else
            {
                ncrs = NCReportService.FindNCReportsByProductId(productId);
            }
            if (ncrs.Any())
            {
                ncr = ncrs.Where(x => x.StateId != States.NCRClosed).LastOrDefault();
                if (ncr == null)
                {
                    return null;
                }
                return ncr;
            }
            return null;
        }

        public void SetupNCRImages(ref NcrDescriptionViewModel model, int ncrId)
        {
            model.EvidenceImageUrl = model.EvidenceImageUrl ?? new List<string>();
            model.ArbitrateVendorCauseImageRefUrl = model.ArbitrateVendorCauseImageRefUrl ?? new List<string>();
            model.ArbitrateCustomerCauseImageRefUrl = model.ArbitrateCustomerCauseImageRefUrl ?? new List<string>();
            model.ArbitrateCustomerDamageImageRefUrl = model.ArbitrateCustomerDamageImageRefUrl ?? new List<string>();
            model.RootCauseOnCustomerImageRefUrl = model.RootCauseOnCustomerImageRefUrl ?? new List<string>();
            model.RootCauseOnVendorImageRefUrl = model.RootCauseOnVendorImageRefUrl ?? new List<string>();
            model.PackingSlipInspectionReport = model.PackingSlipInspectionReport ?? new List<string>();
            model.NCRImageRefUrl = model.NCRImageRefUrl ?? new List<string>();

            List<NCRImages> nCRImages = NCRImagesService.FindNCRImagesByNCReportId(ncrId);

            foreach (var img in nCRImages)
            {
                switch (img.Type)
                {
                    case (int)NCR_IMAGE_TYPE.EVIDENCE:
                        {
                            model.EvidenceImageUrl.Add(img.ImageUrl);
                            break;
                        }
                    case (int)NCR_IMAGE_TYPE.ARBITRATE_VENDOR_CAUSE_REF:
                        {
                            model.ArbitrateVendorCauseImageRefUrl.Add(img.ImageUrl);
                            break;
                        }
                    case (int)NCR_IMAGE_TYPE.ARBITRATE_CUSTOMER_CAUSE_REF:
                        {
                            model.ArbitrateCustomerCauseImageRefUrl.Add(img.ImageUrl);
                            break;
                        }
                    case (int)NCR_IMAGE_TYPE.ROOT_CAUSE_ON_CUSTOMER:
                        {
                            model.RootCauseOnCustomerImageRefUrl.Add(img.ImageUrl);
                            break;
                        }
                    case (int)NCR_IMAGE_TYPE.ROOT_CAUSE_ON_VENDOR:
                        {
                            model.RootCauseOnVendorImageRefUrl.Add(img.ImageUrl);
                            break;
                        }
                    case (int)NCR_IMAGE_TYPE.ARBITRATE_CUSTOMER_DAMAGE_REF:
                        {
                            model.ArbitrateCustomerDamageImageRefUrl.Add(img.ImageUrl);
                            break;
                        }
                    case (int)NCR_IMAGE_TYPE.PACKING_SLIP_INSPECTION_REPORT_PDF:
                        {
                            model.PackingSlipInspectionReport.Add(img.ImageUrl);
                            break;
                        }
                    default:
                        model.NCRImageRefUrl.Add(img.ImageUrl);
                        break;

                }
            }
        }
    }
}