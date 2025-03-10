using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service.Interfaces
{
    public interface IPartRevisionService
    {
        PartRevision FindPartRevisionById(int Id);
        PartRevision FindPartRevisionByTaskId(int TaskId);
        List<PartRevision> FindPartRevisionByProductId(int productId);
        IQueryable<PartRevision> FindPartRevisionsByProductId(int productId);
        List<PartRevision> FindPartRevisionList();
        int AddPartRevision(PartRevision entity);
        void UpdatePartRevision(PartRevision entity);
        void RemovePartRevision(PartRevision entity);
        IQueryable<PartRevision> FindPartRevisions();
        void Dispose();
    }
}
