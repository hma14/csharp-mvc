using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Common;
using Microsoft.Azure;
using Omnae.BlobStorage;
using Omnae.BusinessLayer.Models;
using Omnae.Common;
using Omnae.Data;
using Omnae.Model.Context;
using Omnae.Model.Models;
using Omnae.Model.Models.Aspnet;
using Omnae.Service.Service.Interfaces;

namespace Omnae.BusinessLayer
{
    public class NcrBL
    {
        private ICompanyService CompanyService { get; }
        private INCReportService NcReportService { get; }
        private IImageStorageService ImageStorageService { get; }
        private IDocumentStorageService DocumentStorageService { get; }

        private ILogedUserContext UserContext { get; }
        private ITaskDataService TaskDataService { get; }
        private IOrderStateTrackingService OrderStateTrackingService { get; }
        private IProductStateTrackingService ProductStateTrackingService { get; }
        private INCRImagesService NCRImagesService { get; }
        private readonly NotificationBL NotificationBL;
        private readonly IProductService ProductService;

        public NcrBL(ICompanyService CompanyService, INCReportService NcReportService,
                     ILogedUserContext UserContext, ITaskDataService TaskDataService, IOrderStateTrackingService OrderStateTrackingService,
                     IProductStateTrackingService ProductStateTrackingService, INCRImagesService NCRImagesService, 
                     NotificationBL NotificationBL, IProductService ProductService, IImageStorageService imageStorageService, IDocumentStorageService documentStorageService)
        {
            this.CompanyService = CompanyService;
            this.NcReportService = NcReportService;
            this.UserContext = UserContext;
            this.TaskDataService = TaskDataService;
            this.OrderStateTrackingService = OrderStateTrackingService;
            this.ProductStateTrackingService = ProductStateTrackingService;
            this.NCRImagesService = NCRImagesService;
            this.NotificationBL = NotificationBL;
            this.ProductService = ProductService;
            ImageStorageService = imageStorageService;
            DocumentStorageService = documentStorageService;
        }

        public async Task<int> CreateNCReport(NcrDescriptionViewModel model, IList<HttpPostedFileBase> files)
        {
            var company = CompanyService.FindCompanyById(model.CustomerId.Value);

            // Create a new TaskData of this product
            TaskData taskData = new TaskData
            {
                StateId = (int)model.StateId,
                ProductId = model.ProductId,
                CreatedUtc = DateTime.UtcNow,
                ModifiedUtc = DateTime.UtcNow,
                UpdatedBy = UserContext.User.UserName,
                isEnterprise = company.isEnterprise,
                ModifiedByUserId = UserContext.UserId,
                CreatedByUserId = UserContext.UserId,
            };
            var taskId = TaskDataService.AddTaskData(taskData);


            var ncr = new NCReport()
            {
                OrderId = (int)model.OrderId,
                ProductId = (int)model.ProductId,
                StateId = model.StateId,
                CustomerId = model.CustomerId,
                VendorId = model.VendorId,
                NCDetectedDate = model.NCDetectedDate,
                Quantity = model.Quantity,
                NCOriginator = model.NCOriginator,
                NCDetectedby = DETECTED_BY.CUSTOMER,
                NCDescription = model.NCDescription,
                Expectation = model.Expectation,
                RejectCorrectiveActionReason = model.RejectCorrectiveActionReason,
                RejectRootCauseReason = model.RejectRootCauseReason,
                RejectCorrectivePartsReason = model.RejectCorrectivePartsReason,
                NCRNumber = NcReportService.FindNCReportsYearlySequence(model.CustomerId.Value),
                NCRNumberForVendor = NcReportService.FindNCReportsYearlySequenceForVendor(model.VendorId.Value),
                ArbitrateCustomerCauseReason = model.ArbitrateCustomerCauseReason,
                ArbitrateVendorCauseReason = model.ArbitrateVendorCauseReason,
                TaskId = taskId,
                CarrierName = model.CarrierName,               
            };
            // Save to NCReport table
            int ncrId = NcReportService.AddNCReport(ncr);


            //Upload files
            if (files?.Count > 0 && files[0].ContentLength > 0)
            {
                for (int i = 0; i < files.Count; i++)
                {
                    var postedFile = files[i];
                    string ext = Path.GetExtension(postedFile.FileName);
                    string fileNewName = $"ncr_evident_oid-{model.OrderId}_pid-{model.ProductId}-v{i + 1}{ext}";

                    var url = DocumentStorageService.Upload(postedFile, fileNewName);
                    var entity = new NCRImages
                    {
                        NCReportId = ncrId,
                        ImageUrl = url,
                        Type = (int)NCR_IMAGE_TYPE.EVIDENCE,
                    };
                    NCRImagesService.AddNCRImages(entity);
                }
            }
            

            // Add to OrderStateTracking table with new state and time stamp
            var orderState = new OrderStateTracking()
            {
                OrderId = model.OrderId,
                StateId = taskData.StateId,
                UpdatedBy = taskData.UpdatedBy,
                ModifiedUtc = taskData.ModifiedUtc,
                ModifiedByUserId = taskData.ModifiedByUserId,
                NcrId = ncrId,
                TaskId = taskId,
            };
            OrderStateTrackingService.AddOrderStateTracking(orderState);

            // Fill in ProductStateTracking table with product id, state id ...
            var productState = new ProductStateTracking()
            {
                ProductId = taskData.ProductId.Value,
                StateId = taskData.StateId,
                UpdatedBy = taskData.UpdatedBy,
                ModifiedUtc = taskData.ModifiedUtc,
                ModifiedByUserId = taskData.ModifiedByUserId,
                NcrId = ncrId,
                OrderId = model.OrderId,
            };
            ProductStateTrackingService.AddProductStateTracking(productState);

            // Notify 
            // Sending notification to customer
            var uIds = new List<SimplifiedUser>();
            var product = ProductService.FindProductById(model.ProductId);
            if (product.VendorId != null)
            {
                uIds.AddRange(product.VendorCompany.Users);
            }
            else
            {
                var user = CompanyService.FindCompanyById(model.VendorId.Value);
                uIds.AddRange(user.Users);
            }
            foreach (var user in uIds)
            {
                var destination = user.Email;
                var destinationSms = user.PhoneNumber;
                try
                {
                    taskData.Product = product;
                    await NotificationBL.SendNotificationsAsync(taskData, destination, destinationSms);
                }
                catch (Exception ex)
                {
                    string errorMsg = ex.RetrieveErrorMessage();
                    if (errorMsg.Equals(IndicatingMessages.SmsWarningMsg) || errorMsg.Equals(IndicatingMessages.NoCellPhoneWarningMsg))
                    {
                        continue;
                    }
                    throw;
                }
            }
            return ncrId;
        }
    }
}

