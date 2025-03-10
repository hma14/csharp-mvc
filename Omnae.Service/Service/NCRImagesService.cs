using Omnae.Data.Abstracts;
using Omnae.Data.Repositories.Interfaces;
using Omnae.Model.Models;
using Omnae.Service.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omnae.BlobStorage;

namespace Omnae.Service.Service
{
    public class NCRImagesService : INCRImagesService
    {
        private readonly INCRImagesRepository nCRImagesRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly DocumentStorageService _documentStorage;

        public NCRImagesService(INCRImagesRepository nCRImagesRepository, IUnitOfWork unitOfWork, DocumentStorageService documentStorage)
        {
            this.nCRImagesRepository = nCRImagesRepository;
            this.unitOfWork = unitOfWork;
            this._documentStorage = documentStorage;
        }

        public int AddNCRImages(NCRImages entity)
        {
            return nCRImagesRepository.AddNCRImages(entity);
        }

        public void DeleteNCRImages(NCRImages entity)
        {
            nCRImagesRepository.RemoveNCRImages(entity);
        }

        public void Dispose()
        {
            nCRImagesRepository.Dispose();
        }

        public NCRImages FindNCRImagesById(int Id)
        {
            var ncr = nCRImagesRepository.GetNCRImagesById(Id);
            UpdateDocUrlWithSecurityToken(ncr);
            return ncr;
        }

        public List<NCRImages> FindNCRImagesByNCReportId(int ncreportId)
        {
            var list = nCRImagesRepository.GetNCRImagesByNCReportId(ncreportId);
            UpdateDocUrlWithSecurityToken(list);
            return list;
        }

        //public IQueryable<NCRImages> FindNCRImageListByNCReportId(int ncreportId)
        //{
        //    return nCRImagesRepository.GetNCRImageListByNCReportId(ncreportId);
        //}

        public List<NCRImages> FindNCRImagesByNCReportIdType(int ncreportId, int type)
        {
            var list = nCRImagesRepository.GetNCRImagesByNCReportIdType(ncreportId, type);
            UpdateDocUrlWithSecurityToken(list);
            return list;
        }

        public List<NCRImages> FindNCRImagesList()
        {
            var list = nCRImagesRepository.GetNCRImagesList();
            UpdateDocUrlWithSecurityToken(list);
            return list;
        }

        public void UpdateNCRImages(NCRImages entity)
        {
            nCRImagesRepository.UpdateNCRImages(entity);
        }

        public void UpdateDocUrlWithSecurityToken(NCRImages ncr, ExpireTokenInfo expireTokenInfo = ExpireTokenInfo.Hour)
        {
            if (ncr == null)
                return;

            var newUrl = _documentStorage.AddSecurityTokenToUrl(ncr.ImageUrl, expireTokenInfo);
            ncr.ImageUrl = newUrl;
        }
        public void UpdateDocUrlWithSecurityToken(IEnumerable<NCRImages> ncrs, ExpireTokenInfo expireTokenInfo = ExpireTokenInfo.Hour)
        {
            if (ncrs == null)
                return;

            foreach (var doc in ncrs)
            {
                if (doc == null)
                    continue;

                UpdateDocUrlWithSecurityToken(doc, expireTokenInfo);
            }
        }
    }
}
