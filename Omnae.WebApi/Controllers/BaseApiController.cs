using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.Results;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Libs;
using Omnae.BusinessLayer.Interface;
using Omnae.BusinessLayer.Models;
using Omnae.Common;
using Omnae.Data.Query;
using Omnae.Model.Models;
using Omnae.Service.Service.Interfaces;
using Omnae.WebApi.DTO;
using Omnae.WebApi.Models;
using Omnae.WebApi.Util;
using Serilog;

namespace Omnae.WebApi.Controllers
{
    /// <summary>
    /// Base Api Controller
    /// </summary>
    public abstract class BaseApiController : ApiController
    {
        /// <summary>
        /// Logger
        /// </summary>
        protected ILogger Log { get; }

        /// <summary>
        /// API controller constructor
        /// </summary>
        /// <param name="log"></param>
        protected BaseApiController(ILogger log)
        {
            Log = log;
        }

        /// <summary>
        /// Page Size
        /// </summary>
        protected const int PageSize = 50;

        /// <summary>
        /// Template function to display page meta info and retrieved result
        /// </summary>
        /// <typeparam name="T">Input Type</typeparam>
        /// <typeparam name="TReturn">Returned ProjectTo target type such a DTO</typeparam>
        /// <param name="queryable">Query result from database</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Numbe of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        /// <param name="routeName">API Function Name</param>
        /// <returns></returns>
        [NonAction]
        protected async Task<PagedResultSet<TReturn>> PageOfResultsSetAsync<T, TReturn>(IQueryable<T> queryable, int? page, int pageSize = PageSize, string orderBy = null, bool ascending = true, string routeName = "DefaultApi")
        {
            if (page == null)
            {
                var results = await queryable.ProjectTo<TReturn>().ToListAsync();
                return new PagedResultSet<TReturn>
                {
                    Results = results,
                    Metadata = new PagedResultSet<TReturn>.PageMetadata()
                    {
                        PageNumber = 1,
                        PageSize = results.Count,
                        TotalNumberOfPages = 1,
                        TotalNumberOfRecords = results.Count,
                    }
                };
            }
            return await queryable.ToPagedResultSetAsync<T, TReturn>(this.RequestContext, (int)page, pageSize, orderBy, ascending, routeName);
        }

        [NonAction]
        protected async Task<PagedResultSet<TReturn>> PageOfResultsSetAlternativeMapperAsync<T, TReturn>(IQueryable<T> queryable, int? page, int pageSize = PageSize, string orderBy = null, bool ascending = true, string routeName = "DefaultApi")
        {
            if (page == null)
            {
                var results = (await queryable.ToListAsync()).Select(source => Mapper.Map<TReturn>(source)).ToList();
                return new PagedResultSet<TReturn>
                {
                    Results = results,
                    Metadata = new PagedResultSet<TReturn>.PageMetadata()
                    {
                        PageNumber = 1,
                        PageSize = results.Count,
                        TotalNumberOfPages = 1,
                        TotalNumberOfRecords = results.Count,
                    }
                };
            }
            return await queryable.ToPagedResultSetAlternativeMapperAsync<T, TReturn>(this.RequestContext, (int)page, pageSize, orderBy, ascending, routeName);
        }

        /// <summary>
        /// Template function to display page meta info and retrieved result
        /// </summary>
        /// <typeparam name="T">Input Type</typeparam>
        /// <param name="queryable">Query result from database</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Numbe of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        /// <param name="routeName">API Function Name</param>
        /// <returns></returns>
        [NonAction]
        protected async Task<PagedResultSet<T>> PageOfResultsSetAsync<T>(IQueryable<T> queryable, int? page, int pageSize = PageSize, string orderBy = null, bool ascending = true, string routeName = "DefaultApi")
        {
            if (page == null)
            {
                var results = await queryable.ProjectTo<T>().ToListAsync();
                return new PagedResultSet<T>
                {
                    Results = results,
                    Metadata = new PagedResultSet<T>.PageMetadata()
                    {
                        PageNumber = 1,
                        PageSize = results.Count,
                        TotalNumberOfPages = 1,
                        TotalNumberOfRecords = results.Count,
                    }
                };
            }
            return await queryable.ToPagedResultSetAsync<T>(this.RequestContext, (int)page, pageSize, orderBy, ascending, routeName);
        }

        /// <summary>
        /// Template function to display page meta info and retrieved result
        /// </summary>
        /// <typeparam name="T">Input Type</typeparam>
        /// <param name="queryable">List of T</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Numbe of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        /// <param name="routeName">API Function Name</param>
        /// <returns></returns>
        [NonAction]
        protected PagedResultSet<T> PageOfResultsSet<T>(List<T> queryable, int? page, int pageSize = PageSize, string orderBy = null, bool ascending = true, string routeName = "DefaultApi")
        {
            if (page == null)
            {
                var results = queryable;
                return new PagedResultSet<T>
                {
                    Results = results,
                    Metadata = new PagedResultSet<T>.PageMetadata()
                    {
                        PageNumber = 1,
                        PageSize = results.Count(),
                        TotalNumberOfPages = 1,
                        TotalNumberOfRecords = results.Count(),
                    }
                };
            }
            return queryable.ToPagedResultSet<T>(this.RequestContext, (int)page, pageSize, orderBy, ascending, routeName);
        }

        /// <summary>
        /// Set IQueryable of ProductDTO from a given IQueryable of TaskData
        /// </summary>
        /// <param name="tasks">IQueryable of TaskData</param>
        /// <returns></returns>
        [NonAction]
        protected IQueryable<ProductDTO> SetProductDTO(IQueryable<TaskData> tasks)
        {
            var rfqs = tasks.Select(td => new ProductDTO
            {
                Id = td.ProductId.Value,
                TaskId = td.TaskId,
                ModifiedUtc = td.ModifiedUtc,
                State = (States)td.StateId,
                Name = td.Product.Name,
                Description = td.Product.Description,
                AvatarUri = td.Product.AvatarUri,
                AdminId = td.Product.AdminId,
                CustomerId = td.Product.CustomerId,
                CustomerName = td.Product.CustomerCompany.Name,
                VendorId = td.Product.VendorId,
                VendorName = td.Product.VendorCompany.Name,
                PriceBreakId = td.Product.PriceBreakId,
                PartNumber = td.Product.PartNumber,
                PartNumberRevision = td.Product.PartNumberRevision,
                ParentPartNumberRevision = td.Product.ParentPartNumberRevision,
                PartRevisionId = td.Product.PartRevisionId,
                ParentPartRevisionId = td.Product.ParentPartRevisionId,
                BuildType = td.Product.BuildType,
                Material = td.Product.Material,
                PrecisionMetal = td.Product.PrecisionMetal,
                MetalsProcesses = td.Product.MetalsProcesses,
                MetalType = td.Product.MetalType,
                MetalsSurfaceFinish = td.Product.MetalsSurfaceFinish,
                PrecisionPlastics = td.Product.PrecisionPlastics,
                PlasticsProcesses = td.Product.PlasticsProcesses,
                MembraneSwitches = td.Product.MembraneSwitches,
                MembraneSwitchesAttributes = td.Product.MembraneSwitchesAttributes,
                MembraneSwitchesAttributesWaterproof = td.Product.MembraneSwitchesAttributesWaterproof,
                MembraneSwitchesAttributesEmbossing = td.Product.MembraneSwitchesAttributesEmbossing,
                MembraneSwitchesAttributesLEDLighting = td.Product.MembraneSwitchesAttributesLEDLighting,
                MembraneSwitchesAttributesLED_EL_Backlighting = td.Product.MembraneSwitchesAttributesLED_EL_Backlighting,
                GraphicOverlaysAttributes = td.Product.GraphicOverlaysAttributes,
                GraphicOverlaysAttributesEmbossing = td.Product.GraphicOverlaysAttributesEmbossing,
                GraphicOverlaysAttributesSelectiveTexture = td.Product.GraphicOverlaysAttributesSelectiveTexture,
                Elastomers = td.Product.Elastomers,
                Labels = td.Product.Labels,
                MilledStone = td.Product.MilledStone,
                MilledWood = td.Product.MilledWood,
                FlexCircuits = td.Product.FlexCircuits,
                Others = td.Product.Others,
                MetalType_FreeText = td.Product.MetalType_FreeText,
                SurfaceFinish_FreeText = td.Product.SurfaceFinish_FreeText,
                PlasticType_FreeText = td.Product.PlasticType_FreeText,
                QuoteId = td.Product.QuoteId,
                ToolingLeadTime = td.Product.ToolingLeadTime,
                SampleLeadTime = td.Product.SampleLeadTime,
                ProductionLeadTime = td.Product.ProductionLeadTime,
                ToolingSetupCharges = td.Product.ToolingSetupCharges,
                Status = td.Product.Status,
                HarmonizedCode = td.Product.HarmonizedCode,
                RFQQuantityId = td.Product.RFQQuantityId,
                ExtraQuantityId = td.Product.ExtraQuantityId,
                CreatedDate = td.Product.CreatedDate,
                CustomerPriority = td.Product.CustomerPriority,
                UnitPrice = td.Orders.Where(x => x.UnitPrice != null).Select(x => x.UnitPrice).FirstOrDefault(),
                ProcessType = td.Product.ProcessType,
                AnodizingType = td.Product.AnodizingType,              

                Quantities = new Quantities
                {
                    Qty1 = td.Product.RFQQuantity.Qty1 != null ? td.Product.RFQQuantity.Qty1 : null,
                    Qty2 = td.Product.RFQQuantity.Qty2 != null ? td.Product.RFQQuantity.Qty2 : null,
                    Qty3 = td.Product.RFQQuantity.Qty3 != null ? td.Product.RFQQuantity.Qty3 : null,
                    Qty4 = td.Product.RFQQuantity.Qty4 != null ? td.Product.RFQQuantity.Qty4 : null,
                    Qty5 = td.Product.RFQQuantity.Qty5 != null ? td.Product.RFQQuantity.Qty5 : null,
                    Qty6 = td.Product.RFQQuantity.Qty6 != null ? td.Product.RFQQuantity.Qty6 : null,
                    Qty7 = td.Product.RFQQuantity.Qty7 != null ? td.Product.RFQQuantity.Qty7 : null,
                },
            });

            return rfqs;
        }

        /// <summary>
        /// Set IQueryable of ProductDTO from a given IQueryable of TaskData
        /// </summary>
        /// <param name="tasks">IQueryable of TaskData</param>
        /// <param name="requiredActionStates">List of TaskData States values on which customer or vendor required to action</param>
        /// <returns></returns>
        [NonAction]
        protected IQueryable<ProductDTO> SetProductDTO(IQueryable<TaskData> tasks, IQueryable<int> requiredActionStates)
        {
            var rfqs = tasks.Select(td => new ProductDTO
            {
                Id = td.ProductId.Value,
                TaskId = td.TaskId,
                ModifiedUtc = td.ModifiedUtc,
                State = (States)td.StateId,
                Name = td.Product.Name,
                Description = td.Product.Description,
                AvatarUri = td.Product.AvatarUri,
                AdminId = td.Product.AdminId,
                CustomerId = td.Product.CustomerId,
                CustomerName = td.Product.CustomerCompany.Name,
                VendorId = td.Product.VendorId,
                VendorName = td.Product.VendorCompany.Name,
                PriceBreakId = td.Product.PriceBreakId,
                PartNumber = td.Product.PartNumber,
                PartNumberRevision = td.Product.PartNumberRevision,
                ParentPartNumberRevision = td.Product.ParentPartNumberRevision,
                PartRevisionId = td.Product.PartRevisionId,
                ParentPartRevisionId = td.Product.ParentPartRevisionId,
                BuildType = td.Product.BuildType,
                Material = td.Product.Material,
                PrecisionMetal = td.Product.PrecisionMetal,
                MetalsProcesses = td.Product.MetalsProcesses,
                MetalType = td.Product.MetalType,
                MetalsSurfaceFinish = td.Product.MetalsSurfaceFinish,
                PrecisionPlastics = td.Product.PrecisionPlastics,
                PlasticsProcesses = td.Product.PlasticsProcesses,
                MembraneSwitches = td.Product.MembraneSwitches,
                MembraneSwitchesAttributes = td.Product.MembraneSwitchesAttributes,
                MembraneSwitchesAttributesWaterproof = td.Product.MembraneSwitchesAttributesWaterproof,
                MembraneSwitchesAttributesEmbossing = td.Product.MembraneSwitchesAttributesEmbossing,
                MembraneSwitchesAttributesLEDLighting = td.Product.MembraneSwitchesAttributesLEDLighting,
                MembraneSwitchesAttributesLED_EL_Backlighting = td.Product.MembraneSwitchesAttributesLED_EL_Backlighting,
                GraphicOverlaysAttributes = td.Product.GraphicOverlaysAttributes,
                GraphicOverlaysAttributesEmbossing = td.Product.GraphicOverlaysAttributesEmbossing,
                GraphicOverlaysAttributesSelectiveTexture = td.Product.GraphicOverlaysAttributesSelectiveTexture,
                Elastomers = td.Product.Elastomers,
                Labels = td.Product.Labels,
                MilledStone = td.Product.MilledStone,
                MilledWood = td.Product.MilledWood,
                FlexCircuits = td.Product.FlexCircuits,
                Others = td.Product.Others,
                MetalType_FreeText = td.Product.MetalType_FreeText,
                SurfaceFinish_FreeText = td.Product.SurfaceFinish_FreeText,
                PlasticType_FreeText = td.Product.PlasticType_FreeText,
                QuoteId = td.Product.QuoteId,
                ToolingLeadTime = td.Product.ToolingLeadTime,
                SampleLeadTime = td.Product.SampleLeadTime,
                ProductionLeadTime = td.Product.ProductionLeadTime,
                ToolingSetupCharges = td.Product.ToolingSetupCharges,
                Status = td.Product.Status,
                HarmonizedCode = td.Product.HarmonizedCode,
                RFQQuantityId = td.Product.RFQQuantityId,
                ExtraQuantityId = td.Product.ExtraQuantityId,
                CreatedDate = td.Product.CreatedDate,
                CustomerPriority = td.Product.CustomerPriority,
                UnitPrice = td.Orders.Where(x => x.UnitPrice != null).Select(x => x.UnitPrice).FirstOrDefault(),
                ProcessType = td.Product.ProcessType,
                AnodizingType = td.Product.AnodizingType,
                ActionRequired = requiredActionStates.Contains(td.StateId),

                Quantities = new Quantities
                {
                    Qty1 = td.Product.RFQQuantity.Qty1 != null ? td.Product.RFQQuantity.Qty1 : null,
                    Qty2 = td.Product.RFQQuantity.Qty2 != null ? td.Product.RFQQuantity.Qty2 : null,
                    Qty3 = td.Product.RFQQuantity.Qty3 != null ? td.Product.RFQQuantity.Qty3 : null,
                    Qty4 = td.Product.RFQQuantity.Qty4 != null ? td.Product.RFQQuantity.Qty4 : null,
                    Qty5 = td.Product.RFQQuantity.Qty5 != null ? td.Product.RFQQuantity.Qty5 : null,
                    Qty6 = td.Product.RFQQuantity.Qty6 != null ? td.Product.RFQQuantity.Qty6 : null,
                    Qty7 = td.Product.RFQQuantity.Qty7 != null ? td.Product.RFQQuantity.Qty7 : null,
                },
            });

            return rfqs;
        }


        /// <summary>
        /// Get uploaded file from Request.Files
        /// </summary>
        /// <param name="homeBL">HomeBL Instance</param>
        /// <param name="filesAreRequired">If have no Files</param>
        /// <returns></returns>
        protected List<HttpPostedFileBase> GetPostedFiles(IHomeBL homeBL, bool filesAreRequired = true, bool overwrite = false)
        {
            string root = HttpContext.Current.Server.MapPath(@"~/Docs");
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }

            var request = HttpContext.Current.Request;
            if(filesAreRequired && (request.Files == null || request.Files.Count == 0 || request.Files[0] == null))
                throw new ValidationException(IndicatingMessages.ForgotUploadFile);
            
            var files = new List<HttpPostedFileBase>();
            for (int i = 0; i < request.Files.Count; i++)
            {
                string path = request.Files[i].FileName;
                var filePath = Path.Combine(root, path);

                if (!File.Exists(filePath) || overwrite)
                {
                    request.Files[i].SaveAs(filePath);
                }
                
                var postedFileBase = new MemoryFile(filePath, request.Files[i].ContentType);
                files.Add(postedFileBase);
            }
            return files;
        }

        /// <summary>
        /// Delete uploaded files which otherwise will stay on server
        /// </summary>
        /// <param name="files">List of HttpPostedFileBase</param>
        protected void DeleteUploadFiles(List<HttpPostedFileBase> files)
        {
            if (files == null)
                return;

            string root = HttpContext.Current.Server.MapPath(@"~/Docs");

            //delete files
            foreach (var file in files)
            {
                if (file == null)
                    continue;

                file.InputStream.Close();
                (file as IDisposable)?.Dispose();

                var filePath = Path.Combine(root, file.FileName);
                TryDeleteFile(filePath);
            }
        }

        private bool TryDeleteFolder(string folder, int maxRetries = 10, int millisecondsDelay = 50)
        {
            if (folder == null) throw new ArgumentNullException(folder);
            if (maxRetries < 1) throw new ArgumentOutOfRangeException(nameof(maxRetries));
            if (millisecondsDelay < 1) throw new ArgumentOutOfRangeException(nameof(millisecondsDelay));

            for (int i = 0; i < maxRetries; ++i)
            {
                try
                {
                    if (Directory.Exists(folder))
                    {
                        Directory.Delete(folder, true);
                    }

                    return true;
                }
                catch (DirectoryNotFoundException)
                {
                    return true;
                }
                catch (IOException)
                {
                    Thread.Sleep(millisecondsDelay);
                }
                catch (UnauthorizedAccessException)
                {
                    Thread.Sleep(millisecondsDelay);
                }
            }

            return false;
        }

        private bool TryDeleteFile(string filePath, int maxRetries = 10, int millisecondsDelay = 50)
        {
            if (filePath == null) throw new ArgumentNullException(filePath);
            if (maxRetries < 1) throw new ArgumentOutOfRangeException(nameof(maxRetries));
            if (millisecondsDelay < 1) throw new ArgumentOutOfRangeException(nameof(millisecondsDelay));

            for (int i = 0; i < maxRetries; ++i)
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                     
                    return true;
                }
                catch (DirectoryNotFoundException)
                {
                    return true;
                }
                catch (IOException)
                {
                    Thread.Sleep(millisecondsDelay);
                }
                catch (UnauthorizedAccessException)
                {
                    Thread.Sleep(millisecondsDelay);
                }
            }

            return false;
        }

        protected override BadRequestResult BadRequest()
        {
            Log.Warning("A BadRequest was return to the user.");
            return base.BadRequest();
        }

        protected override BadRequestErrorMessageResult BadRequest(string message)
        {
            Log.Warning("A BadRequest with a message was return to the user: {Message}", message);
            return base.BadRequest(message);
        }

        protected override InvalidModelStateResult BadRequest(ModelStateDictionary modelState)
        {
            var msgs = modelState?.Values?.SelectMany(v => v.Errors).Select(v => v.ErrorMessage)?.ToArray() ?? new string[] {};
            Log.Warning("A BadRequest for a validation issue was return to the user: {Message}", string.Join(" - ", msgs));
            return base.BadRequest(modelState);
        }

        protected bool IsUniqueRFQOn3Factors(IProductService productService, int customerId, string partNumber, string partNumberRevision)
        {
            var prods = productService.HasUniqueProducts(customerId, partNumber, partNumberRevision);
            return (prods == null || prods.Count(x => x.VendorId == null) == 0);
        }

        protected bool IsUniqueProductOn3Factors(IProductService productService, int customerId, string partNumber, string partNumberRevision)
        {
            var prods = productService.HasUniqueProducts(customerId, partNumber, partNumberRevision);
            return (prods == null || prods.Count(x => x.VendorId != null) == 0);
        }

        protected bool IsUniqueProductOn4Factors(IProductService productService, int customerId, int vendorId, string partNumber, string partNumberRevision)
        {
            var prods = productService.HasUniqueProducts(customerId, partNumber, partNumberRevision);
            return (prods == null || prods.Count(x => x.VendorId != null && x.VendorId == vendorId) == 0);
        }
    }
}