using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Common;
using Libs.Notification;
using Microsoft.AspNet.Identity;
using Omnae.BlobStorage;
using Omnae.BusinessLayer.Identity.Model;
using Omnae.BusinessLayer.Models;
using Omnae.Common;
using Omnae.Libs.Notification;
using Omnae.Libs.ViewModel;
using Omnae.Model.Models;
using Omnae.Notification;
using Omnae.Notification.Model;
using Omnae.QuickBooks.QBO;
using Omnae.Service.Service.Interfaces;
using RazorEngine;
using RazorEngine.Templating;
using Serilog;

namespace Omnae.BusinessLayer
{
    public partial class NotificationBL
    {
        private NotificationService NotificationService { get; }
        private IDocumentService documentService { get; }
        private IOrderService orderService { get; }
        private ICompanyService companyService { get; }
        private IQboTokensService qboTokensService { get; }
        private ApplicationDbContext dbUser { get; }
        private IPriceBreakService priceBreakService { get; }
        private IEmailSender EmailSender { get; }
        private ISmsSender SmsSender { get; }
        private readonly INCRImagesService ncrImangeService;
        private readonly INCReportService ncReportService;

        private ILogger Log { get; }
        private string Url { get; }


        public NotificationBL(NotificationService notificationService, IDocumentService documentService, IOrderService orderService,
                              ICompanyService companyService, IQboTokensService qboTokensService, ApplicationDbContext dbUser,
                              IPriceBreakService priceBreakService, IEmailSender emailSender, ISmsSender smsSender,
                              INCRImagesService ncrImangeService, INCReportService ncReportService, ILogger log)
        {
            NotificationService = notificationService;
            this.documentService = documentService;
            this.orderService = orderService;
            this.companyService = companyService;
            this.qboTokensService = qboTokensService;
            this.dbUser = dbUser;
            this.priceBreakService = priceBreakService;
            this.EmailSender = emailSender;
            this.SmsSender = smsSender;
            this.ncrImangeService = ncrImangeService;
            this.ncReportService = ncReportService;
            Log = log;
            this.Url = ConfigurationManager.AppSettings["URL"];
        }


        public async Task SendNotificationsAsync(TaskData taskData, string destination, string destinationSms,
                                                 bool isAdmin = false,
                                                 List<byte[]> attachmentData = null,
                                                 List<string> VendorInvoicesNumbers = null)
        {
            string subject = string.Empty;
            List<Document> docs = null;
            List<string> invoiceName = null;
            ApplicationUser usr = dbUser.Users.Where(u => u.UserName.Equals(destination, StringComparison.CurrentCultureIgnoreCase) == true).FirstOrDefault();
            string qboid = string.Empty;
            NCReport ncReport = null;
            List<string> qboEmails = null;
            List<string> evidenceUrls = null;

            var orders = orderService.FindOrderByTaskId(taskData.TaskId);
            if (!orders.Any())
            {
                orders = orderService.FindOrdersByProductId(taskData.ProductId.Value);
            }
            Order order = null;
            if (orders != null && orders.Any())
            {
                var pb = priceBreakService.FindPriceBreakByTaskId(taskData.TaskId).FirstOrDefault() ??
                         priceBreakService.FindPriceBreakByProductId(taskData.ProductId.Value)
                                          .LastOrDefault(x => x.RFQBidId > 0 && x.RFQBid.IsActive == true && x.NumberSampleIncluded != null);

                var numberSampleIncluded = pb?.NumberSampleIncluded ?? 1;

                if (taskData.StateId == (int)States.SampleComplete)
                {
                    order = orders.FirstOrDefault(x => x.Quantity == numberSampleIncluded || x.Quantity == 1 || x.Quantity == 0);
                }
                else if (taskData.StateId == (int)States.VendorPendingInvoice || taskData.StateId == (int)States.ProductionComplete)
                {
                    if (taskData.IsRiskBuild == true)
                    {
                        order = orders.LastOrDefault(x => x.TrackingNumber != null && x.CarrierName != null);
                    }
                    else
                    {
                        order = orders.FirstOrDefault(x => x.Quantity > numberSampleIncluded || x.IsForToolingOnly == true && x.Quantity == numberSampleIncluded);
                    }
                    if (order == null)
                    {
                        order = orders.LastOrDefault();
                        if (order == null)
                        {
                            throw new Exception(IndicatingMessages.OrderNotFound);
                        }
                    }
                }

                if (order == null)
                {
                    order = orders.LastOrDefault();
                }
            }

            BaseViewModel entity = null;
            bool needInvoice = false;
            switch (taskData.StateId)
            {
                case (int)States.PendingRFQ:
                    subject = $"Omnae.com Notify a new part, {taskData.Product.PartNumber}, {taskData.Product.Description} is created";
                    entity = new RevisionRequestedViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        RevisingReason = taskData.RevisingReason,
                        Description = taskData.Product.Description,
                        Url = Url
                    };
                    break;
                case (int)States.PendingRFQRevision:
                    subject = $"Omnae.com Notify revision request for {taskData.Product.PartNumber}, {taskData.Product.Description} is pending";
                    entity = new RevisionRequestedViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        RevisingReason = taskData.RevisingReason,
                        Description = taskData.Product.Description,
                        Url = Url
                    };
                    break;
                case (int)States.RFQRevision:
                    subject = $"Omnae.com Revision Requested {taskData.Product.PartNumber}, {taskData.Product.Description}";
                    entity = new RevisionRequestedViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        RevisingReason = taskData.RevisingReason,
                        Description = taskData.Product.Description,
                        Url = Url
                    };
                    break;

                case (int)States.BidForRFQ:
                    subject = $"Omnae.com Notify vendors who are bidding for {taskData.Product.PartNumber}, {taskData.Product.Description}";
                    entity = new RevisionRequestedViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        RevisingReason = taskData.RevisingReason,
                        Description = taskData.Product.Description,
                        Url = Url
                    };
                    break;
                case (int)States.BidReview:
                    subject = $"Omnae.com Notify bid review for {taskData.Product.PartNumber}, {taskData.Product.Description}";
                    entity = new RevisionRequestedViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        RevisingReason = taskData.RevisingReason,
                        Description = taskData.Product.Description,
                        Url = Url
                    };
                    break;
                case (int)States.BidTimeout:
                    subject = $"Omnae.com Notify that the bid deadline is passed for {taskData.Product.PartNumber}, {taskData.Product.Description}";
                    entity = new RevisionRequestedViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        RevisingReason = taskData.RevisingReason,
                        Description = taskData.Product.Description,
                        Url = Url
                    };
                    break;
                case (int)States.BackFromRFQ:
                    subject = $"Omnae.com Notify customer to take action for Revision Request from Vendor for {taskData.Product.PartNumber}, {taskData.Product.Description}";
                    entity = new RevisionRequestedViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        RevisingReason = taskData.RevisingReason,
                        Description = taskData.Product.Description,
                        Url = Url
                    };
                    break;

                case (int)States.QuoteAccepted:
                    subject = $"Omnae.com Quote Complete {taskData.Product.PartNumber}, {taskData.Product.Description}";
                    entity = new BaseViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        Description = taskData.Product.Description,
                        Url = Url
                    };
                    break;

                case (int)States.ProofingComplete:
                    subject = $"Omnae.com Proof Complete {taskData.Product.PartNumber}, {taskData.Product.Description}";
                    docs = documentService.FindDocumentByTaskIdDocType(taskData.TaskId, DOCUMENT_TYPE.PROOF_PDF, ExpireTokenInfo.FourDays);
                    if (docs == null || !docs.Any())
                    {
                        docs = documentService.FindDocumentByProductIdDocType(taskData.ProductId.Value, DOCUMENT_TYPE.PROOF_PDF, ExpireTokenInfo.FourDays);
                    }

                    if (order == null)
                        throw new Exception("No order was found for this task.");

                    entity = new ProofApprovedViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        Description = taskData.Product.Description,
                        Url = Url,
                        Doc_Proof = docs.Any() ? docs.LastOrDefault() : null,
                        PONumber = order.CustomerPONumber,
                    };
                    break;
                case (int)States.ProofApproved:
                    subject = $"Omnae.com Proof Approved {taskData.Product.PartNumber}, {taskData.Product.Description}";
                    docs = documentService.FindDocumentByTaskIdDocType(taskData.TaskId, DOCUMENT_TYPE.PROOF_PDF, ExpireTokenInfo.FourDays);
                    if (docs == null || !docs.Any())
                    {
                        docs = documentService.FindDocumentByProductIdDocType(taskData.ProductId.Value, DOCUMENT_TYPE.PROOF_PDF, ExpireTokenInfo.FourDays).ToList();
                    }

                    if (order == null)
                        throw new Exception("No order was found for this task.");

                    entity = new ProofApprovedViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        Description = taskData.Product.Description,
                        Url = Url,
                        Doc_Proof = docs.Any() ? docs.LastOrDefault() : null,
                        PONumber = order.CustomerPONumber,
                    };
                    break;
                case (int)States.SampleStarted:
                    subject = $"Omnae.com Tooling Complete {taskData.Product.PartNumber}, {taskData.Product.Description}";
                    if (order == null)
                        throw new Exception("No order was found for this task.");

                    entity = new BaseViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        Description = taskData.Product.Description,
                        Url = Url,
                        PONumber = order.CustomerPONumber,
                    };
                    break;
                case (int)States.SampleComplete:
                    needInvoice = taskData.Product?.CustomerCompany.Term != null && taskData.Product?.CustomerCompany.Term > 0;
                    if (order == null)
                    {
                        throw new Exception("No order was found for this task.");
                    }
                    if (taskData.RejectReason == null)
                    {
                        if (needInvoice)
                        {
                            docs = documentService.FindDocumentByTaskIdDocType(taskData.TaskId, DOCUMENT_TYPE.QBO_INVOICE_PDF, ExpireTokenInfo.FourDays);

                            if (docs == null || docs.Count() == 0)
                            {
                                docs = documentService.FindDocumentByProductIdDocType(taskData.ProductId.Value, DOCUMENT_TYPE.QBO_INVOICE_PDF, ExpireTokenInfo.FourDays).ToList();
                            }

                            if (docs != null && docs.Count() > 0)
                            {
                                invoiceName = new List<string>();
                                foreach (var doc in docs)
                                {
                                    invoiceName.Add(doc.Name);
                                }
                            }
                        }
                    }
                    subject = $"Sample Complete {taskData.Product.PartNumber} with PO#: {order.CustomerPONumber}";

                    usr = dbUser.Users.Where(u => u.UserName.Equals(destination, StringComparison.CurrentCultureIgnoreCase) == true).FirstOrDefault();
                    if (taskData.isEnterprise == false && usr != null)
                    {
                        var company = companyService.FindCompanyByUserId(usr.Id);
                        qboid = company.QboId;
                        qboEmails = await GetQboCustomerEmails(qboid);
                    }

                    entity = new SampleCompleteViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        Description = taskData.Product.Description,
                        Url = Url,
                        Carrier = order.CarrierName,
                        TrackingNumber = order.TrackingNumber,
                        Doc_Invoice = needInvoice == true ? docs : null,
                        InvoiceData = needInvoice == true ? attachmentData : null,
                        InvoiceName = needInvoice == true ? invoiceName : null,
                        PONumber = order.CustomerPONumber,
                        QboEmails = qboEmails,
                    };
                    break;
                case (int)States.SampleApproved:
                    if (taskData.IsRiskBuild == true)
                    {
                        subject = $"Omnae.com Proof Approved {taskData.Product.PartNumber}, {taskData.Product.Description}";
                    }
                    else
                    {
                        subject = $"Omnae.com Sample Approved {taskData.Product.PartNumber}, {taskData.Product.Description}";
                    }

                    if (order == null)
                        throw new Exception("No order was found for this task.");

                    entity = new BaseViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        Description = taskData.Product.Description,
                        Url = Url,
                        PONumber = order.CustomerPONumber,
                    };
                    break;
                case (int)States.ProductionStarted:
                    subject = $"Omnae.com Production Started {taskData.Product.PartNumber}, {taskData.Product.Description}";
                    if (order == null)
                        throw new Exception("No order was found for this task.");

                    entity = new BaseViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        Description = taskData.Product.Description,
                        Url = Url,
                        PONumber = order.CustomerPONumber,
                    };
                    break;
                case (int)States.VendorPendingInvoice:
                case (int)States.ProductionComplete:
                    needInvoice = taskData.Product?.CustomerCompany.Term != null && taskData.Product?.CustomerCompany.Term > 0;
                    if (needInvoice)
                    {
                        if (taskData.StateId == (int)States.ProductionComplete && taskData.isEnterprise == true)
                        {
                            docs = documentService.FindDocumentByTaskIdDocType(taskData.TaskId, DOCUMENT_TYPE.ENTERPRISE_VENDOR_INVOICE_PDF, ExpireTokenInfo.FourDays);
                            if (docs == null || docs.Count() == 0)
                            {
                                docs = documentService.FindDocumentByProductIdDocType(taskData.ProductId.Value, DOCUMENT_TYPE.ENTERPRISE_VENDOR_INVOICE_PDF, ExpireTokenInfo.FourDays).ToList();
                            }
                        }
                        else
                        {
                            docs = documentService.FindDocumentByTaskIdDocType(taskData.TaskId, DOCUMENT_TYPE.QBO_INVOICE_PDF, ExpireTokenInfo.FourDays);
                            if (docs == null || docs.Count() == 0)
                            {
                                docs = documentService.FindDocumentByProductIdDocType(taskData.ProductId.Value, DOCUMENT_TYPE.QBO_INVOICE_PDF, ExpireTokenInfo.FourDays).ToList();
                            }
                        }


                        if (docs == null || docs.Count() == 0)
                        {
                            docs = documentService.FindDocumentByTaskIdDocType(taskData.TaskId, DOCUMENT_TYPE.QBO_INVOICE_PDF, ExpireTokenInfo.FourDays);
                        }
                        subject = $"Production Complete for Part #: {taskData.Product.PartNumber}";

                        if (order == null)
                            throw new Exception("No order was found for this task.");

                        if (docs != null && docs.Count() > 0)
                        {
                            invoiceName = new List<string>();
                            foreach (var doc in docs)
                            {
                                invoiceName.Add(doc.Name);
                            }
                        }
                    }

                    if (taskData.isEnterprise == false && usr != null)
                    {
                        var company = companyService.FindCompanyByUserId(usr.Id);
                        qboid = company.QboId;
                        qboEmails = await GetQboCustomerEmails(qboid);
                    }

                    entity = new SampleCompleteViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        Description = taskData.Product.Description,
                        Url = Url,
                        Carrier = order.CarrierName,
                        TrackingNumber = order.TrackingNumber,
                        Doc_Invoice = needInvoice == true ? docs : null,
                        InvoiceData = needInvoice == true ? attachmentData : null,
                        InvoiceName = needInvoice == true ? invoiceName : null,
                        PONumber = order.CustomerPONumber,
                        QboEmails = qboEmails,
                    };

                    break;
                case (int)States.OrderInitiated:
                    if (!string.IsNullOrEmpty(taskData.RejectReason))
                    {
                        subject = $"Payment Got Rejected for {taskData.Product.PartNumber}, {taskData.Product.Description}";
                    }
                    else
                    {
                        subject = $"Omnae.com Order Initiated {taskData.Product.PartNumber}, {taskData.Product.Description}";
                    }
                    entity = new BaseViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        Description = taskData.Product.Description,
                        Url = Url,
                        PONumber = order?.CustomerPONumber,
                        RejectReason = taskData.RejectReason,
                    };
                    break;
                case (int)States.ReOrderInitiated:
                    if (!string.IsNullOrEmpty(taskData.RejectReason))
                    {
                        subject = $"Payment Got Rejected for Re-order {taskData.Product.PartNumber}, {taskData.Product.Description}";
                    }
                    else
                    {
                        subject = $"Omnae.com Re-Order Initiated {taskData.Product.PartNumber}, {taskData.Product.Description}";
                    }
                    if (order == null)
                        throw new Exception("No order was found for this task.");

                    entity = new BaseViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        Description = taskData.Product.Description,
                        Url = Url,
                        PONumber = order.CustomerPONumber,
                        RejectReason = taskData.RejectReason,
                    };
                    break;
                case (int)States.PaymentMade:
                    subject = $"Omnae.com Payment Made for Order {taskData.Product.PartNumber}, {taskData.Product.Description}";
                    if (order == null)
                        throw new Exception("No order was found for this task.");

                    entity = new BaseViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        Description = taskData.Product.Description,
                        Url = Url,
                        PONumber = order?.CustomerPONumber,
                    };
                    break;

                case (int)States.OrderPaid:
                    subject = $"Omnae.com Payment Paid for Order {taskData.Product.PartNumber}, {taskData.Product.Description}";
                    if (order == null)
                        throw new Exception("No order was found for this task.");

                    entity = new BaseViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        Description = taskData.Product.Description,
                        Url = Url,
                        PONumber = order.CustomerPONumber,
                    };
                    break;

                case (int)States.ReOrderPaid:
                    subject = $"Omnae.com Payment Paid for Re-Order {taskData.Product.PartNumber}, {taskData.Product.Description}";
                    if (order == null)
                        throw new Exception("No order was found for this task.");

                    entity = new BaseViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        Description = taskData.Product.Description,
                        Url = Url,
                        PONumber = order.CustomerPONumber,
                    };
                    break;

                case (int)States.ReOrderPaymentMade:
                    subject = $"Omnae.com Payment Made for Re-Order {taskData.Product.PartNumber}, {taskData.Product.Description}";
                    if (order == null)
                        throw new Exception("No order was found for this task.");

                    entity = new BaseViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        Description = taskData.Product.Description,
                        Url = Url,
                        PONumber = order.CustomerPONumber,
                    };
                    break;
                case (int)States.AddExtraQuantities:
                    subject = string.Format($"Add Extra Quantities requested by customer for part#  {taskData.Product.PartNumber} of {taskData.Product.Description}");

                    entity = new BaseViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        Description = taskData.Product.Description,
                        Url = Url,
                    };
                    break;
                case (int)States.SetupMarkupExtraQty:
                    subject = string.Format($"Extra Quantities have been successfully added for part# {taskData.Product.PartNumber} of {taskData.Product.Description}");

                    entity = new BaseViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        Description = taskData.Product.Description,
                        Url = Url,
                    };
                    break;

                case (int)States.NCRVendorRootCauseAnalysis:
                    if (isAdmin)
                    {
                        subject = string.Format($"Admin arbitrated the NCR root cause to vendor");

                        //ncReport = ncReportService.FindNCReportByProductIdOrderId(taskData.ProductId.Value, order.Id).LastOrDefault();
                        ncReport = ncReportService.FindNCReportByTaskId(taskData.TaskId);
                        evidenceUrls = ncReport != null ? ncrImangeService.FindNCRImagesByNCReportId(ncReport.Id)
                                                                            .Where(x => x.Type == (int)NCR_IMAGE_TYPE.ARBITRATE_VENDOR_CAUSE_REF)
                                                                            .Select(x => x.ImageUrl).ToList() : null;
                        entity = new BaseViewModel
                        {
                            UserName = destination,
                            PartNumber = taskData.Product.PartNumber,
                            Description = taskData.Product.Description,
                            Url = Url,
                            NCRArbitrateReasonByAdmin = ncReport?.ArbitrateVendorCauseReason,
                            EvidenceFileUrls = evidenceUrls,
                        };
                    }
                    else
                    {
                        subject = string.Format($"Customer Created a new NC Report regarding part# {taskData.Product.PartNumber} of {taskData.Product.Description}");
                        entity = new BaseViewModel
                        {
                            UserName = destination,
                            PartNumber = taskData.Product.PartNumber,
                            Description = taskData.Product.Description,
                            Url = Url,
                        };
                    }

                    break;
                case (int)States.NCRCustomerApproval:
                    subject = string.Format($"Vendor sent replacement/rework parts to customer for approval regarding the NCR");

                    entity = new BaseViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        Description = taskData.Product.Description,
                        Url = Url,
                    };
                    break;
                case (int)States.NCRVendorCorrectivePartsInProduction:
                    subject = $"Vendor puts corrective parts of the NCR in production";
                    //ncReport = ncReportService.FindNCReportByProductIdOrderId(taskData.ProductId.Value, order.Id).LastOrDefault();
                    ncReport = ncReportService.FindNCReportByTaskId(taskData.TaskId);
                    entity = new BaseViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        Description = taskData.Product.Description,
                        Url = Url,
                        RejectCorrectivePartsReason = ncReport?.RejectCorrectivePartsReason,
                    };
                    break;
                case (int)States.NCRCustomerRejectCorrective:
                    subject = string.Format($"Customer rejected received corrective parts regarding the NCR");
                    ncReport = ncReportService.FindNCReportByProductIdOrderId(taskData.ProductId.Value, order.Id).LastOrDefault();
                    entity = new BaseViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        Description = taskData.Product.Description,
                        Url = Url,
                        CustomerRejectedReasons = ncReport?.RejectCorrectiveActionReason,
                    };
                    break;
                case (int)States.NCRCustomerRejectRootCause:
                    subject = $"Customer rejected root cause of the NCR";
                    //ncReport = ncReportService.FindNCReportByProductIdOrderId(taskData.ProductId.Value, order.Id).LastOrDefault();
                    ncReport = ncReportService.FindNCReportByTaskId(taskData.TaskId);
                    entity = new BaseViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        Description = taskData.Product.Description,
                        Url = Url,
                        CustomerRejectedReasons = ncReport?.RejectRootCauseReason,
                    };
                    break;
                case (int)States.NCRRootCauseDisputes:
                    subject = $"Vendor has raised root cause disputes on this NCR";

                    entity = new BaseViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        Description = taskData.Product.Description,
                        Url = Url,
                    };
                    break;
                case (int)States.NCRAdminDisputesIntervention:
                    subject = $"Admin will intervene to the disputes on this NCR";

                    entity = new BaseViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        Description = taskData.Product.Description,
                        Url = Url,
                    };
                    break;
                case (int)States.NCRDamagedByCustomer:
                    subject = $"Admin arbitrated the root cause of the NCR to damaged by customer";

                    //ncReport = ncReportService.FindNCReportByProductIdOrderId(taskData.ProductId.Value, order.Id).LastOrDefault();
                    ncReport = ncReportService.FindNCReportByTaskId(taskData.TaskId);
                    evidenceUrls = ncReport != null ? ncrImangeService.FindNCRImagesByNCReportId(ncReport.Id)
                        .Where(x => x.Type == (int)NCR_IMAGE_TYPE.ARBITRATE_CUSTOMER_DAMAGE_REF)
                        .Select(x => x.ImageUrl).ToList() : null;
                    entity = new BaseViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        Description = taskData.Product.Description,
                        Url = Url,
                        NCRArbitrateReasonByAdmin = ncReport?.ArbitrateCustomerCauseReason,
                        EvidenceFileUrls = evidenceUrls,
                    };
                    break;
                case (int)States.NCRCustomerRevisionNeeded:
                    subject = $"Admin arbitrated revision needed by customer regarding the NCR";

                    //ncReport = ncReportService.FindNCReportByProductIdOrderId(taskData.ProductId.Value, order.Id).LastOrDefault();
                    ncReport = ncReportService.FindNCReportByTaskId(taskData.TaskId);
                    if (ncReport == null)
                    {
                        ncReport = ncReportService.FindNCReportByProductId(taskData.ProductId.Value);
                    }
                    evidenceUrls = ncReport != null ? ncrImangeService.FindNCRImagesByNCReportId(ncReport.Id)
                        .Where(x => x.Type == (int)NCR_IMAGE_TYPE.ARBITRATE_CUSTOMER_CAUSE_REF)
                        .Select(x => x.ImageUrl).ToList() : null;
                    entity = new BaseViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        Description = taskData.Product.Description,
                        Url = Url,
                        NCRArbitrateReasonByAdmin = ncReport?.ArbitrateCustomerCauseReason,
                        EvidenceFileUrls = evidenceUrls,
                    };
                    break;
                case (int)States.NCRVendorCorrectivePartsComplete:
                    subject = $"Vendor completed corrective parts for the NCR";

                    ncReport = ncReportService.FindNCReportByTaskId(taskData.TaskId);
                    evidenceUrls = ncReport != null ? ncrImangeService.FindNCRImagesByNCReportId(ncReport.Id)
                        .Where(x => x.Type == (int)NCR_IMAGE_TYPE.ARBITRATE_CUSTOMER_CAUSE_REF)
                        .Select(x => x.ImageUrl).ToList() : null;
                    entity = new BaseViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        Description = taskData.Product.Description,
                        Url = Url,
                        NCRArbitrateReasonByAdmin = ncReport?.ArbitrateCustomerCauseReason,
                        EvidenceFileUrls = evidenceUrls,
                        CarrierName = ncReport?.CarrierName,
                        TrackingNumber = ncReport?.TrackingNumber,
                    };
                    break;

                case (int)States.NCRCustomerCorrectivePartsReceived:
                    subject = $"Customer received corrective parts for the NCR";

                    ncReport = ncReportService.FindNCReportByTaskId(taskData.TaskId);
                    evidenceUrls = ncReport != null ? ncrImangeService.FindNCRImagesByNCReportId(ncReport.Id)
                        .Where(x => x.Type == (int)NCR_IMAGE_TYPE.ARBITRATE_CUSTOMER_CAUSE_REF)
                        .Select(x => x.ImageUrl).ToList() : null;
                    entity = new BaseViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        Description = taskData.Product.Description,
                        Url = Url,
                        NCRArbitrateReasonByAdmin = ncReport?.ArbitrateCustomerCauseReason,
                        EvidenceFileUrls = evidenceUrls,

                    };
                    break;
                case (int)States.NCRCustomerCorrectivePartsAccepted:
                    subject = $"Customer accepted corrective parts for the NCR";
                    entity = new BaseViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        Description = taskData.Product.Description,
                        Url = Url,
                    };
                    break;
                case (int)States.NCRClosed:
                    subject = $"This NCR is closed";
                    entity = new BaseViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        Description = taskData.Product.Description,
                        Url = Url,
                    };
                    break;
                case (int)States.VendorCancelledRFQ:
                    subject = $"Vendor cancelled RFQ bidding";
                    entity = new BaseViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        Description = taskData.Product.Description,
                        Url = Url,
                        VendorName = taskData.RFQBid?.VendorCompany.Name,
                    };
                    break;
                case (int)States.ProofRejected:
                    subject = $"Customer rejected Proof";
                    docs = documentService.FindDocumentByTaskIdDocType(taskData.TaskId, DOCUMENT_TYPE.PROOF_PDF, ExpireTokenInfo.FourDays);
                    if (docs == null || !docs.Any())
                    {
                        docs = documentService.FindDocumentByProductIdDocType(taskData.ProductId.Value, DOCUMENT_TYPE.PROOF_PDF, ExpireTokenInfo.FourDays);
                    }

                    if (order == null)
                        throw new Exception("No order was found for this task.");

                    entity = new ProofApprovedViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        Description = taskData.Product.Description,
                        Url = Url,
                        Doc_Proof = docs.Any() ? docs.LastOrDefault() : null,
                        PONumber = order.CustomerPONumber,
                        RejectReason = taskData.RejectReason,
                    };

                    break;
                case (int)States.SampleRejected:
                    subject = $"Customer rejected Sample";
                    docs = documentService.FindDocumentByTaskIdDocType(taskData.TaskId, DOCUMENT_TYPE.CORRESPOND_SAMPLE_REJECT_PDF, ExpireTokenInfo.FourDays);
                    entity = new BaseViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        Description = taskData.Product.Description,
                        Url = Url,
                        VendorName = taskData.RFQBid?.VendorCompany.Name,
                        PONumber = order?.CustomerPONumber,
                        RejectReason = taskData.RejectReason,
                        DocUri = docs.Select(x => x.DocUri).LastOrDefault(),
                    };
                    break;

                case (int)States.CustomerCancelledRFQ:
                    subject = $"Customer cancelled RFQ bidding";
                    entity = new BaseViewModel
                    {
                        UserName = destination,
                        PartNumber = taskData.Product.PartNumber,
                        Description = taskData.Product.Description,
                        Url = Url,
                        CustomerName = taskData.Product.CustomerCompany.Name,
                    };
                    break;

                default:
                    return;
            }

            if (!string.IsNullOrEmpty(subject))
            {
                try
                {
                    NotificationService.NotifyStateChange((States)taskData.StateId, subject, destination, destinationSms, entity, false, isAdmin);
                }
                catch (Exception ex)
                {
                    string msg = ex.RetrieveErrorMessage();
                    if (!msg.Equals(IndicatingMessages.SmsWarningMsg) && !msg.Equals(IndicatingMessages.NoCellPhoneWarningMsg))
                    {
                        throw;
                    }
                }

            }
        }

        public async Task SendBidWillExpireNotificationsAsync(TaskData taskData, string destinationFullName, string destination, string timeToExpire)
        {
            var model = new BidTimeLimitWillExpireEmailModel
            {
                UserName = !string.IsNullOrEmpty(destinationFullName) ? destinationFullName : destination,
                ProductName = taskData.Product.Name,
                ProductPartNumber = taskData.Product.PartNumber,
                ProductDescription = taskData.Product.Description,
                CustomerName = taskData.Product.CustomerCompany.Name,
                TimeToExpire = timeToExpire,
                Url = Url,
            };

            Log.Information("Sending email to {destination}. {@Data}", destination, model);

            var bodyTemplate = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Template/BidTimeLimitWillExpireEmail.html"));

            var body = Engine.Razor.RunCompile(bodyTemplate, null, model);

            var message = new EmailMessage { Subject = $"Omnae - Bid deadline for {model.ProductName} will expire soon.", DestinationEmail = destination, Body = body };

            EmailSender.SendEmail(message);
        }

        public void NotifyCreateNewRevision(string subject, string destination, string destinationSms, CreatePartRevisionViewModel model)
        {
            var template = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Template/CreatePartRevisionEmail.html"));

            var body = Engine.Razor.RunCompile(template, null, model);
            var message = new EmailMessage { Subject = subject, DestinationEmail = destination, Body = body};

            EmailSender.SendEmail(message);
        }

        public void NotifyCreateProductSharing(string subject, string destination, string destinationSms, ProductsharingViewModel model)
        {
            var template = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Template/CreateProductSharingEmail.html"));

            var message = new EmailMessage { Subject = subject, DestinationEmail = destination, Body = Engine.Razor.RunCompile(template, null, model)};

            EmailSender.SendEmail(message);
        }

        public void NotifyUpdateQuantities(string subject, string destination, string destinationSms, RFQUpdateQuantityViewModel model)
        {
            var template = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Template/UpdateQFQQuantitiesEmail.html"));

            var message = new EmailMessage { Subject = subject, DestinationEmail = destination, Body = Engine.Razor.RunCompile(template, null, model)};

            EmailSender.SendEmail(message);
        }

        private async Task<List<string>> GetQboCustomerEmails(string qboid)
        {
            QboApi qboApi = new QboApi(qboTokensService);
            return await qboApi.GetQboCustomerEmails(qboid);
        }

    }
}
