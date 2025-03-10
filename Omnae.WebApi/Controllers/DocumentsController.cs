using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common;
using Omnae.BusinessLayer;
using Omnae.BusinessLayer.Interface;
using Omnae.Common;
using Omnae.Data;
using Omnae.Model.Models;
using Omnae.Service.Service.Interfaces;
using Omnae.WebApi.DTO;
using Omnae.WebApi.Models;
using Omnae.WebApi.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using Libs;
using Serilog;

namespace Omnae.WebApi.Controllers
{
    /// <summary>
    /// Core Api for Docudment 
    /// </summary>
    [EnableCors("*", "*", "*")]
    [RoutePrefix("api/documents")]
    public class DocumentsController : BaseApiController
    {
        private IMapper Mapper { get; }
        private readonly DocumentBL documentBL;
        private readonly IDocumentService documentService;
        private readonly CompanyAccountsBL companyBL;
        private readonly IHomeBL homeBL;
        private readonly IOrderService orderService;

        /// <summary>
        /// Documents controller constructor
        /// </summary>
        public DocumentsController(ILogger log, DocumentBL documentBl, IDocumentService documentService, CompanyAccountsBL companyBl, IHomeBL homeBl, IOrderService orderService, IMapper mapper) : base(log)
        {
            documentBL = documentBl;
            this.documentService = documentService;
            companyBL = companyBl;
            homeBL = homeBl;
            this.orderService = orderService;
            Mapper = mapper;
        }

        /// <summary>
        /// Get all documents
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        [HttpGet]
        [Route("", Name = "GetDocument")]
        public async Task<PagedResultSet<DocumentDTO>> Get(int? page = 1, int pageSize = PageSize, string orderBy = nameof(DocumentDTO.Id), bool ascending = false)
        {
            //TODO: Remove this endpoint. This is a security risk.
            var docs = documentService.FindDocuments();
            return await PageOfResultsSetAlternativeMapperAsync<Document, DocumentDTO>(docs, page, pageSize, orderBy, ascending, "GetDocument");
        }


        /// <summary>
        /// Get document details which id = {id}
        /// </summary>
        /// <param name="id">The ID of the document.</param>
        [HttpGet]
        [Route("{id:int}")]
        [ResponseType(typeof(DocumentDTO))]
        public async Task<IHttpActionResult> Get(int id)
        {
            //var doc = await documentService.FindDocuments()
            //                                   .Where(d => d.Id == id)
            //                                   .ProjectTo<DocumentDTO>()
            //                                   .SingleOrDefaultAsync();

            var doc1 = await documentService.FindDocuments()
                .Where(d => d.Id == id)
                .SingleOrDefaultAsync();

            var doc = Mapper.Map<DocumentDTO>(doc1);
            if (doc == null)
            {
                return NotFound();
            }

            return Ok(doc);
        }

        /// <summary>
        /// Get company's document which companyId = {companyId}
        /// </summary>
        /// <param name="companyId">Company Id.</param>
        /// <param name="docType">Document Type.</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Numbe of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        [HttpGet]
        [Route("company/{companyId:int}", Name = "GetCompanyDocuments")]
        public async Task<PagedResultSet<DocumentDTO>> GetCompanyDocuments(int companyId, DOCUMENT_TYPE? docType = null, int? page = 1, int pageSize = PageSize, string orderBy = nameof(DocumentDTO.Id), bool ascending = false)
        {
            IQueryable<Document> docs = null;
            if (docType != null)
            {
                docs = documentService.FindDocumentsByCompanyId(companyId).Where(d => d.DocType == (int)docType);
            }
            else
            {
                docs = documentService.FindDocumentsByCompanyId(companyId);
            }
            var dto = await PageOfResultsSetAlternativeMapperAsync<Document, DocumentDTO>(docs, page, pageSize, orderBy, ascending, "GetCompanyDocuments");
            dto.Results = dto?.Results.Where(x => x.TaskId != null).Select(x => { x.OrderId = orderService.FindOrderByTaskId(x.TaskId.Value).FirstOrDefault()?.Id; return x; }); //TODO: Fix this. Slow
            return dto;
        }


        /// <summary>
        /// Get user's documents which user id = {userId}
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="docType">Document Type.</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        [HttpGet]
        [Route("user/{userId}", Name = "GetUserDocuments")]
        public async Task<PagedResultSet<DocumentDTO>> GetUserDocuments(string userId, DOCUMENT_TYPE? docType = null, int? page = 1, int pageSize = PageSize, string orderBy = nameof(DocumentDTO.Id), bool ascending = false)
        {
            var user = await companyBL.FindByIdAsync(userId, null);
            return await GetCompanyDocuments(user.CompanyId.Value, docType, page, pageSize, orderBy, ascending);
        }


        /// <summary>
        /// Modify document which id = {id}
        /// </summary>
        /// <param name="id">The id of the document.</param>
        /// <param name="document">The entity of the document to be modified.</param>
        [ResponseType(typeof(void))]
        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult Put(int id, Document document)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != document.Id)
            {
                return BadRequest();
            }

            try
            {
                documentService.UpdateDocument(document);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DocumentExists(id))
                {
                    return NotFound();
                }

                throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Create a new document 
        /// </summary>
        [ResponseType(typeof(DocumentDTO))]
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post(DocumentDTO docDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var doc = Mapper.Map<Document>(docDTO);

            var docId = documentService.AddDocument(doc);

            return CreatedAtRoute("DefaultApi", new { id = docId }, doc);
        }


        /// <summary>
        /// Upload drawing document(s) for a RFQ 
        /// </summary>
        [ResponseType(typeof(DocumentDTO))]
        [HttpPost]
        [Route("user/{userId}/uploadproductdocs/{pid:int}", Name = "UploadProductDocs")]
        public async Task<IHttpActionResult> UploadProductDocs(int pid)
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            List<HttpPostedFileBase> files = new List<HttpPostedFileBase>();
            try
            {
                string root = HttpContext.Current.Server.MapPath(@"~/Docs");
                Directory.CreateDirectory(root);

                var request = HttpContext.Current.Request;
                if (request.Files == null || request.Files.Count == 0 || request.Files[0] == null)
                {
                    return BadRequest(IndicatingMessages.ForgotUploadFile);
                }

                for (int i = 0; i < request.Files.Count; i++)
                {
                    string path = request.Files[i].FileName;
                    var filePath = Path.Combine(root, path);

                    if (!System.IO.File.Exists(filePath))
                    {
                        request.Files[i].SaveAs(filePath);
                    }

                    var postedFileBase = new MemoryFile(filePath, request.Files[i].ContentType);//homeBL.CreateHttpPostedFileBase(filePath, request.Files[i].ContentType);
                    files.Add(postedFileBase);
                }

                var provider = new MultipartFormDataStreamProvider(root);

                int taskId = 0;

                // Read the form data and return an async task.
                var task = await Request.Content.ReadAsMultipartAsync(provider).
                    ContinueWith<HttpResponseMessage>(t =>
                    {
                        if (t.IsFaulted || t.IsCanceled)
                        {
                            Request.CreateErrorResponse(HttpStatusCode.InternalServerError, t.Exception);
                        }
                        foreach (var key in provider.FormData.AllKeys)
                        {
                            foreach (var val in provider.FormData.GetValues(key))
                            {
                                switch (key)
                                {
                                    case "taskId":
                                        taskId = int.Parse(val);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }

                        return Request.CreateResponse(HttpStatusCode.OK);
                    });

                string error = documentBL.UploadProductDocs(files, pid, taskId);
                if (!string.IsNullOrEmpty(error))
                {
                    throw new System.Exception(error);
                }

                var newDoc = documentService.FindDocumentByProductId(pid);
                var newDocDTO = Mapper.Map<List<DocumentDTO>>(newDoc);

                return CreatedAtRoute("UploadProductDocs", new { id = pid }, newDocDTO);
            }
            finally
            {
                DeleteUploadFiles(files);
            }
        }

        /// <summary>
        /// Upload proofing document(s) for a RFQ 
        /// </summary>
        [ResponseType(typeof(DocumentDTO))]
        [HttpPost]
        [Route("user/{userId}/uploadproofingdoc", Name = "UploadProofingDoc")]
        public IHttpActionResult UploadProofingDoc()
        {
            using (var trans = AsyncTransactionScope.StartNew())
            {
                if (!Request.Content.IsMimeMultipartContent())
                {
                    var ex = new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                    return BadRequest(ex.RetrieveErrorMessage());
                }

                var formDataDic = HttpContext.Current.Request.Form.ToDictionary();
                if (!formDataDic.ContainsKey("productId") || !formDataDic.ContainsKey("taskId"))
                {
                    return BadRequest(IndicatingMessages.MissingFormData);
                }
                int productId = int.Parse(formDataDic["productId"]);
                int taskId = int.Parse(formDataDic["taskId"]);

                var files = GetPostedFiles(homeBL);
                List<string> fileExtentions = files.Select(x => Path.GetExtension(x.FileName)).ToList();
                try
                {
                    bool result = documentBL.UploadProofingDoc(files, productId, taskId, fileExtentions);
                    if (!result)
                    {
                        return Json("failed");
                    }
                    var newDoc = documentService.FindDocumentByProductId(productId).OrderBy(x => x.Id).LastOrDefault();
                    var newDocDTO = Mapper.Map<DocumentDTO>(newDoc);

                    trans.Complete();
                    return CreatedAtRoute("UploadProofingDoc", new { id = newDoc.Id }, newDocDTO);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.RetrieveErrorMessage());
                }
                finally
                {
                    DeleteUploadFiles(files);
                }
            }
        }

        /// <summary>
        /// Delete a document which id = {id}
        /// </summary>
        /// <param name="id">The id of the document.</param>
        [ResponseType(typeof(void))]
        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult Delete(int id)
        {
            documentBL.Delete(id);
            return Ok();
        }

        private bool DocumentExists(int id)
        {
            return documentService.FindDocumentById(id) != null;
        }
    }
}