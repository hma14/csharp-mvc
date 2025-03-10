using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service.Interfaces
{
    public interface INCRImagesService
    {
        void Dispose();
        int AddNCRImages(NCRImages entity);
        List<NCRImages> FindNCRImagesByNCReportId(int ncreportId);
        List<NCRImages> FindNCRImagesByNCReportIdType(int ncreportId, int type);
        NCRImages FindNCRImagesById(int Id);
        List<NCRImages> FindNCRImagesList();
        void UpdateNCRImages(NCRImages entity);
        void DeleteNCRImages(NCRImages entity);
        //IQueryable<NCRImages> FindNCRImageListByNCReportId(int ncreportId);
    }
}
