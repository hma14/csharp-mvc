using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Common;
using Microsoft.Azure;
using Omnae.BlobStorage;
using Omnae.Common;
using Omnae.Model.Context;
using Omnae.Model.Models;
using Omnae.Service.Service.Interfaces;

namespace Omnae.BusinessLayer
{
    public class DocumentBL
    {
        public readonly ILogedUserContext UserContext;
        public readonly IDocumentService DocumentService;
        public readonly IDocumentStorageService DocumentStorageService;
        public readonly IImageStorageService ImageStorageService;
        public readonly IProductService ProductService;
        public readonly ICompanyService CompanyService;
        public readonly IOrderService OrderService;
        public readonly INCRImagesService NcrImageService;

        public DocumentBL(ILogedUserContext userContext, IDocumentService documentService, IProductService productService,
            ICompanyService companyService, IOrderService orderService, INCRImagesService ncrImageService, IDocumentStorageService documentStorageService, IImageStorageService imageStorageService)
        {
            UserContext = userContext;
            DocumentService = documentService;
            ProductService = productService;
            CompanyService = companyService;
            this.OrderService = orderService;
            this.NcrImageService = ncrImageService;
            DocumentStorageService = documentStorageService;
            ImageStorageService = imageStorageService;
        }

        public bool UploadProofingDoc(List<HttpPostedFileBase> files, int productId, int taskId, List<string> fileExts = null)
        {
            //var proofDocVersionMap = new Dictionary<int, string>()
            //{
            //    { 1, "A" },
            //    { 2, "B" },
            //    { 3, "C" },
            //    { 4, "D" },
            //    { 5, "E" },
            //    { 6, "F" },
            //};

            var product = ProductService.FindProductById(productId);

            int preVersion = 0;
            var docs = DocumentService.FindDocumentByProductId(productId)?.Where(x => x.DocType == (int)DOCUMENT_TYPE.PROOF_PDF).ToArray();
            if (docs.Length > 0)
            {
                preVersion = docs[docs.Length - 1].Version;
            }

            for (int i = 0; i < files.Count; i++)
            {
                var postedFile = files[i];
                if (!string.IsNullOrEmpty(postedFile.FileName))
                {
                    string ext = Path.GetExtension(postedFile.FileName);
                    if (string.IsNullOrEmpty(ext))
                    {
                        ext = fileExts[i];
                        if (ext == null)
                        {
                            return false;
                        }
                    }
                    //string fileNewName = $"proof_pid-{product.Id}_version_{proofDocVersionMap[preVersion + i + 1]}{ext.ToLower()}";
                    string fileNewName = "proof_doc-" + Guid.NewGuid() + ".pdf";

                    string docUri = DocumentStorageService.Upload(postedFile, fileNewName);
                    var doc = DocumentService.FindDocumentByProductId(productId).Where(d => d.Name == fileNewName).FirstOrDefault();
                    if (doc != null)
                    {
                        continue;
                    }

                    doc = new Document()
                    {
                        Version = preVersion + 1,
                        Name = fileNewName,
                        DocType = (int)DOCUMENT_TYPE.PROOF_PDF,
                        UserType = (int)USER_TYPE.Vendor,
                        DocUri = docUri,
                        ProductId = productId,
                        UpdatedBy = UserContext.User.UserName,
                        CreatedByUserId = UserContext.User.UserId,
                        CreatedUtc = DateTime.UtcNow,
                        ModifiedUtc = DateTime.UtcNow,
                        TaskId = taskId,
                    };

                    DocumentService.AddDocument(doc);
                }
            }

            return true;
        }

        public Document Delete(int id)
        {
            var document = DocumentService.FindDocumentById(id);
            if (document != null)
            {
                // Delete document from Azure BlobStorage
                DocumentStorageService.Delete(document.DocUri);
                DocumentService.RemoveDocument(document);
            }
            return document;
        }
        public NCRImages DeleteNcrImage(int id)
        {
            var img = NcrImageService.FindNCRImagesById(id);
            if (img != null)
            {
                // Delete Image from Azure BlobStorage
                ImageStorageService.Delete(img.ImageUrl); //Old Files are in the Image Storage. Try to delete 1st.
                DocumentStorageService.Delete(img.ImageUrl);
                NcrImageService.DeleteNCRImages(img);
            }
            return img;
        }

        public string UploadProductDocs(List<HttpPostedFileBase> fileBases, int productId, int taskId, int? partRevisionId = null)
        {
            var product = ProductService.FindProductById(productId);
            if (!ProductService.IsUsersProduct(product, UserContext, CompanyService))
                return "Product is not owned by current user";

            var userType = UserContext.UserType;
            int preVersion = 0;
            bool has2DFile = false;
            bool has3DFile = false;

            var docs = DocumentService.FindDocumentByProductId(product.Id).ToArray();
            if (docs.Length > 0)
            {
                // check if metal or plastique already has upload 3-D file
                has3DFile = docs.Any(x => x.DocType == (int)DOCUMENT_TYPE.PRODUCT_3D_STEP);
                preVersion = docs[docs.Length - 1].Version;
            }

            // check metal or plastique should upload 3-D file
            if (userType == USER_TYPE.Customer && (product.Material == MATERIALS_TYPE.PrecisionMetals || product.Material == MATERIALS_TYPE.PrecisionPlastics))
            {
                for (int i = 0; i < fileBases.Count; i++)
                {
                    var postedFile = fileBases[i];
                    if (fileBases[i].ContentType.ToLower().Equals("application/octet-stream") || fileBases[i].ContentType.ToLower().Equals("model/stl"))
                    {
                        has3DFile = true;
                    }
                    else
                    {
                        has2DFile = true;
                    }
                }

                if (has3DFile == false || has2DFile == false)
                {
                    return "Both Metal and Plastiques are required to upload at least one 2D and one 3D files!";
                }
            }

            for (int i = 0; i < fileBases.Count; i++)
            {
                var postedFile = fileBases[i];
                if (!string.IsNullOrEmpty(postedFile.FileName))
                {
                    string ext = Path.GetExtension(postedFile.FileName);
                    string fileNewName = "product_doc-" + Guid.NewGuid() + ".pdf";
                    string docUri = string.Empty;
                    try
                    {
                        docUri = DocumentStorageService.Upload(postedFile, fileNewName);
                    }
                    catch (Exception ex)
                    {
                        return ExceptionEx.RetrieveErrorMessage(ex);
                    }

                    DOCUMENT_TYPE docType = DOCUMENT_TYPE.PRODUCT_2D_PDF;
                    if (postedFile.ContentType.ToLower().Equals("application/octet-stream"))
                    {
                        docType = DOCUMENT_TYPE.PRODUCT_3D_STEP;
                    }
                    var doc = new Document()
                    {
                        TaskId = taskId,
                        Version = preVersion + 1,
                        Name = fileNewName,
                        DocType = (int)docType,
                        DocUri = docUri,
                        UserType = (int)userType,
                        ProductId = productId,
                        UpdatedBy = UserContext.User.UserName,
                        CreatedByUserId = UserContext.User.UserId,
                        CreatedUtc = DateTime.UtcNow,
                        ModifiedUtc = DateTime.UtcNow,
                        PartRevisionId = partRevisionId,
                    };

                    try
                    {
                        DocumentService.AddDocument(doc);
                    }
                    catch
                    {
                        return "Tried to upload same document twice! Duplicating document name: " + doc.Name;
                    }
                }
            }
            return null;
        }

        

        public IList<Document> UploadMissingFiles(List<HttpPostedFileBase> files, CUSTOMER_MISSING_DOCUMENT_TYPE docType, TaskData td, int? orderId = null)
        {
            var docVersionMap = new Dictionary<int, string>()
            {
                { 1, "A" },
                { 2, "B" },
                { 3, "C" },
                { 4, "D" },
                { 5, "E" },
                { 6, "F" },
                { 7, "G" },
                { 8, "H" },
                { 9, "I" },
                { 10, "J" },
            };

            var product = td.Product;
            int productId = td.ProductId.Value;

            int preVersion = 0;
            var docs = DocumentService.FindDocumentByProductId(productId)?.Where(x => x.DocType == (int)docType).ToList();
            if (docs.Count > 0)
            {
                preVersion = docs.OrderBy(v => v.Version).Last().Version;
            }
            int currentVersion = ++preVersion;
            USER_TYPE userType = USER_TYPE.Customer;
            var vendor = product.VendorCompany;
            for (int i = 0; i < files.Count; i++)
            {
                var postedFile = files[i];
                if (!string.IsNullOrEmpty(postedFile.FileName))
                {
                    string ext = Path.GetExtension(postedFile.FileName).ToLower();
                    string fileNewName = string.Empty;

                    switch (docType)
                    {
                        case CUSTOMER_MISSING_DOCUMENT_TYPE.PRODUCT_2D_PDF:
                        case CUSTOMER_MISSING_DOCUMENT_TYPE.PRODUCT_3D_STEP:
                            fileNewName = "product_doc-" + Guid.NewGuid() + ".pdf";
                            break;
                        case CUSTOMER_MISSING_DOCUMENT_TYPE.PO_PDF:
                            if (orderId == null)
                            {
                                throw new Exception(IndicatingMessages.UploadDocOnInvalidState);
                            }
                            fileNewName = $"customer_po_oid-{orderId}_pid-{productId}_{currentVersion + i}{ext}";
                            fileNewName = "po_doc-" + Guid.NewGuid() + ".pdf";
                            break;
                        case CUSTOMER_MISSING_DOCUMENT_TYPE.PROOF_PDF:
                            fileNewName = "proof_doc-" + Guid.NewGuid() + ".pdf";
                            //if (orderId == null)
                            //{
                            //    throw new Exception(IndicatingMessages.UploadDocOnInvalidState);
                            //}
                            //if (currentVersion + i > 10)
                            //{
                            //    fileNewName = $"proof_oid-{orderId}_pid-{productId}_version-{currentVersion + i}{ext}";                               
                            //}
                            //else
                            //{
                            //    fileNewName = $"proof_oid-{orderId}_pid-{productId}_version-{docVersionMap[currentVersion + i]}{ext}";
                            //}
                            userType = USER_TYPE.Vendor;
                            break;
                        case CUSTOMER_MISSING_DOCUMENT_TYPE.REVISING_DOCS:
                            fileNewName = $"{vendor.Name}-revising_doc-" + Guid.NewGuid() + ".pdf";
                            //if (currentVersion + i > 10)
                            //{
                            //    //fileNewName = $"revising_tid-{td.TaskId}_pid-{productId}_{currentVersion + i}{ext}";
                            //    fileNewName = $"{vendor.Name}-{product.PartNumber}-{product.PartNumberRevision}_version-{currentVersion + i}{ext}";
                            //}
                            //else
                            //{
                            //    //fileNewName = $"revising_tid-{td.TaskId}_pid-{productId}_{docVersionMap[currentVersion + i]}{ext}";
                            //    fileNewName = $"{vendor.Name}-{product.PartNumber}-{product.PartNumberRevision}_{docVersionMap[currentVersion + i]}{ext}";
                            //}

                            break;
                        case CUSTOMER_MISSING_DOCUMENT_TYPE.CORRESPOND_PROOF_REJECT_PDF:
                            if (orderId == null)
                            {
                                throw new Exception(IndicatingMessages.UploadDocOnInvalidState);
                            }
                            fileNewName = $"Proof_Reject_Reason-" + Guid.NewGuid() + ".pdf";
                            break;
                        case CUSTOMER_MISSING_DOCUMENT_TYPE.CORRESPOND_SAMPLE_REJECT_PDF:
                            if (orderId == null)
                            {
                                throw new Exception(IndicatingMessages.UploadDocOnInvalidState);
                            }
                            fileNewName = $"Sample_Reject_Reason-" + Guid.NewGuid() + ".pdf"; 
                            break;
                        case CUSTOMER_MISSING_DOCUMENT_TYPE.PRODUCT_AVATAR:
                            fileNewName = Path.GetFileName(postedFile.FileName);
                            break;

                        default:
                            throw new Exception(IndicatingMessages.InvalidDocumentType);

                    }


                    string docUri = DocumentStorageService.Upload(postedFile, fileNewName);
                    var doc = DocumentService.FindDocumentByProductId(productId).Where(d => d.Name == fileNewName).FirstOrDefault();
                    if (doc != null)
                    {
                        continue;
                    }

                    doc = new Document()
                    {
                        Version = currentVersion + i,
                        Name = fileNewName,
                        DocType = (int)docType,
                        UserType = (int)userType,
                        DocUri = docUri,
                        ProductId = productId,
                        UpdatedBy = UserContext.User.UserName,
                        CreatedByUserId = UserContext.User.UserId,
                        CreatedUtc = DateTime.UtcNow,
                        ModifiedUtc = DateTime.UtcNow,
                        TaskId = td.TaskId,
                    };

                    DocumentService.AddDocument(doc);
                    docs.Add(doc);
                }
            }
            return docs;
        }

        public IList<NCRImages> UploadMissingNcrImages(List<HttpPostedFileBase> files, NCR_IMAGE_TYPE docType, int ncrId, int orderId)
        {
            int version = NcrImageService.FindNCRImagesByNCReportIdType(ncrId, (int)docType).Count;
            for (int i = 0; i < files.Count; i++)
            {
                var postedFile = files[i];
                if (!string.IsNullOrEmpty(postedFile.FileName))
                {
                    string ext = Path.GetExtension(postedFile.FileName).ToLower();
                    string fileNewName = string.Empty;

                    switch (docType)
                    {
                        case NCR_IMAGE_TYPE.EVIDENCE:
                            fileNewName = $"ncr_evident_oid-{orderId}_ncrid-{ncrId}_v-{++version}{ext}";
                            break;

                        case NCR_IMAGE_TYPE.VENDOR_CAUSE_REF:
                            fileNewName = $"ncr_arbitrate_to_vendor_cause_image_oid-{orderId}_ncrid-{ncrId}_v-{++version}{ext}";
                            break;

                        case NCR_IMAGE_TYPE.CUSTOMER_CAUSE_REF:
                            fileNewName = $"ncr_customer_cause_image_oid-{orderId}_ncrid-{ncrId}_v-{++version}{ext}";
                            break;

                        case NCR_IMAGE_TYPE.ROOT_CAUSE_ON_CUSTOMER:
                            fileNewName = $"ncr_rootcause_on_customer_oid-{orderId}_ncrid-{ncrId}_v-{++version}{ext}";
                            break;

                        case NCR_IMAGE_TYPE.ARBITRATE_VENDOR_CAUSE_REF:
                            fileNewName = $"ncr_arbitrate_to_vendor_cause_image_oid-{orderId}_ncrid-{ncrId}_v-{++version}{ext}";
                            break;
                        case NCR_IMAGE_TYPE.ARBITRATE_CUSTOMER_DAMAGE_REF:
                            fileNewName = $"ncr_customer_cause_image_oid-{orderId}_ncrid-{ncrId}_v-{++version}{ext}";
                            break;
                        case NCR_IMAGE_TYPE.ARBITRATE_CUSTOMER_CAUSE_REF:
                            fileNewName = $"ncr_customer_cause_image_oid-{orderId}_ncrid-{ncrId}_v-{++version}{ext}";
                            break;

                        default:
                            throw new Exception(IndicatingMessages.InvalidDocumentType);
                    }

                    string imgUri = DocumentStorageService.Upload(postedFile, fileNewName);
                    NCRImages img = new NCRImages()
                    {
                        NCReportId = ncrId,
                        Type = (int)docType,
                        ImageUrl = imgUri,
                    };

                    NcrImageService.AddNCRImages(img);
                }
            }
            return NcrImageService.FindNCRImagesByNCReportId(ncrId);
        }

        public IList<Document> UploadGeneralFiles(List<HttpPostedFileBase> files, string fileNameWithoutExtention, DOCUMENT_TYPE docType, TaskData td, int userType)
        {
            var docVersionMap = new Dictionary<int, string>()
            {
                { 1, "A" },
                { 2, "B" },
                { 3, "C" },
                { 4, "D" },
                { 5, "E" },
                { 6, "F" },
                { 7, "G" },
                { 8, "H" },
                { 9, "I" },
                { 10, "J" },
            };

            var product = td.Product;
            int productId = td.ProductId.Value;


            int preVersion = 0;
            var docs = DocumentService.FindDocumentByProductId(productId)?.Where(x => x.DocType == (int)docType).ToArray();
            if (docs.Length > 0)
            {
                preVersion = docs.OrderBy(v => v.Version).Last().Version;
            }
            int currentVersion = ++preVersion;
            for (int i = 0; i < files.Count; i++)
            {
                var postedFile = files[i];
                if (!string.IsNullOrEmpty(postedFile.FileName))
                {
                    string ext = Path.GetExtension(postedFile.FileName).ToLower();
                    string fileNewName = string.Empty;
                    if (currentVersion + i > 10)
                    {
                        fileNewName = $"{fileNameWithoutExtention}_{currentVersion + i}{ext}";
                    }
                    else
                    {
                        fileNewName = $"{fileNameWithoutExtention}_{docVersionMap[currentVersion + i]}{ext}";
                    }

                    string docUri = DocumentStorageService.Upload(postedFile, fileNewName);
                    var doc = DocumentService.FindDocumentByProductId(productId).Where(d => d.Name == fileNewName).FirstOrDefault();
                    if (doc != null)
                    {
                        continue;
                    }

                    doc = new Document()
                    {
                        Version = currentVersion + i,
                        Name = fileNewName,
                        DocType = (int)docType,
                        UserType = userType,
                        DocUri = docUri,
                        ProductId = productId,
                        UpdatedBy = UserContext.User.UserName,
                        CreatedByUserId = UserContext.User.UserId,
                        CreatedUtc = DateTime.UtcNow,
                        ModifiedUtc = DateTime.UtcNow,
                        TaskId = td.TaskId,
                    };

                    DocumentService.AddDocument(doc);
                }
            }
            return docs.ToList();
        }

    }
}