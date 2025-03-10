using Omnae.Data.Abstracts;
using Omnae.Data.Repositories.Interfaces;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Data.Repositories
{
    public class NCRImagesRepository : RepositoryBase<NCRImages>, INCRImagesRepository
    {
        public NCRImagesRepository(OmnaeContext dbContext) : base(dbContext)
        {

        }

        public int AddNCRImages(NCRImages entity)
        {
            base.Add(entity);
            this.DbContext.Commit();
            return entity.Id;
        }

        public void Dispose()
        {
            this.DbContext.Dispose();
        }

        public NCRImages GetNCRImagesById(int Id)
        {
            return this.DbContext.NCRImages.Where(x => x.Id == Id).FirstOrDefault();
        }

        public List<NCRImages> GetNCRImagesByNCReportId(int ncreportId)
        {
            return this.DbContext.NCRImages.Where(x => x.NCReportId == ncreportId).ToList();

        }
        public IQueryable<NCRImages> GetNCRImageListByNCReportId(int ncreportId)
        {
            return this.DbContext.NCRImages.Where(x => x.NCReportId == ncreportId);

        }

        public List<NCRImages> GetNCRImagesByNCReportIdType(int ncreportId, int type)
        {
            return this.DbContext.NCRImages.Where(x => x.NCReportId == ncreportId && x.Type == type).ToList();
        }

        public List<NCRImages> GetNCRImagesList()
        {
            return this.DbContext.NCRImages.ToList();
        }

        public void RemoveNCRImages(NCRImages entity)
        {
            var entry = this.DbContext.Entry(entity);
            if (entry.State == System.Data.Entity.EntityState.Detached)
                DbContext.NCRImages.Attach(entity);
            base.Delete(entity);
            this.DbContext.Commit();
        }

        public void UpdateNCRImages(NCRImages entity)
        {
            base.Update(entity);
            this.DbContext.Commit();
        }
    }
}
