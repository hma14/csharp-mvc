using Omnae.Data.Abstracts;
using Omnae.Data.Repositories.Interfaces;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Omnae.Data.Repositories
{
    public class PartRevisionRepository : RepositoryBase<PartRevision>, IPartRevisionRepository
    {
        public PartRevisionRepository(OmnaeContext dbContext) : base(dbContext)
        {

        }

        
        public int AddPartRevision(PartRevision entity)
        {
            base.Add(entity);
            this.DbContext.Commit();
            return entity.Id;
        }

        public void Dispose()
        {
            this.DbContext.Dispose();
        }

        public PartRevision GetPartRevisionById(int Id)
        {
            return this.DbContext.PartRevisions.FirstOrDefault(x => x.Id == Id);
        }
        public PartRevision GetPartRevisionByTaskId(int TaskId)
        {
            return this.DbContext.PartRevisions.FirstOrDefault(x => x.TaskId == TaskId);
        }

        public List<PartRevision> GetPartRevisionByProductId(int productId)
        {
            return this.DbContext.PartRevisions.Where(p => p.OriginProductId == productId).OrderBy(x => x.CreatedUtc).ToList();
        }
        public IQueryable<PartRevision> GetPartRevisionsByProductId(int productId)
        {
           
            return this.DbContext.PartRevisions.Where(p => p.OriginProductId == productId).OrderBy(x => x.CreatedUtc);
        }
        public List<PartRevision> GetPartRevisionList()
        {
            return this.DbContext.PartRevisions.ToList();
        }
        public IQueryable<PartRevision> GetPartRevisions()
        {
            return this.DbContext.PartRevisions;
        }

        public void RemovePartRevision(PartRevision entity)
        {
            base.Delete(entity);
            this.DbContext.Commit();
        }

        public void UpdatePartRevision(PartRevision entity)
        {
            base.Update(entity);
            this.DbContext.Commit();
        }

        
    }
}
