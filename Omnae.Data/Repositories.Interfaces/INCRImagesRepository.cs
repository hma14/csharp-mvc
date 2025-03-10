using Omnae.Data.Abstracts;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Data.Repositories.Interfaces
{
    public interface INCRImagesRepository : IRepository<NCRImages>
    {
        void Dispose();
        int AddNCRImages(NCRImages entity);
        List<NCRImages> GetNCRImagesByNCReportId(int ncreportId);
        List<NCRImages> GetNCRImagesByNCReportIdType(int ncreportId, int type);
        NCRImages GetNCRImagesById(int Id);     
        List<NCRImages> GetNCRImagesList();     
        void UpdateNCRImages(NCRImages entity);
        void RemoveNCRImages(NCRImages entity);
        IQueryable<NCRImages> GetNCRImageListByNCReportId(int ncreportId);
    }
}
