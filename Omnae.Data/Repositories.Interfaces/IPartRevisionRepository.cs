using Omnae.Model.Models;
using Omnae.Data.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Data.Repositories.Interfaces
{
    public interface IPartRevisionRepository : IRepository<PartRevision>
    {
        PartRevision GetPartRevisionById(int Id);
        PartRevision GetPartRevisionByTaskId(int TaskId);
        List<PartRevision> GetPartRevisionByProductId(int productId);
        IQueryable<PartRevision> GetPartRevisionsByProductId(int productId);
        List<PartRevision> GetPartRevisionList();
        int AddPartRevision(PartRevision entity);
        void UpdatePartRevision(PartRevision entity);
        void RemovePartRevision(PartRevision entity);
        IQueryable<PartRevision> GetPartRevisions();

        void Dispose();
    }
}
