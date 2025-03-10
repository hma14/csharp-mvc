using Omnae.Data.Abstracts;
using Omnae.Data.Repositories.Interfaces;
using Omnae.Service.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omnae.Model.Models;

namespace Omnae.Service.Service
{
    public class PartRevisionService : IPartRevisionService
    {
        IPartRevisionRepository partRevisionRepository;
        IUnitOfWork unitOfWork;

        public PartRevisionService(IPartRevisionRepository partRevisionRepository, UnitOfWork unitOfWork)
        {
            this.partRevisionRepository = partRevisionRepository;
            this.unitOfWork = unitOfWork;
        }

        public int AddPartRevision(PartRevision entity)
        {
            return partRevisionRepository.AddPartRevision(entity);
        }

        public void Dispose()
        {
            partRevisionRepository.Dispose();
        }

        public PartRevision FindPartRevisionById(int Id)
        {
            return partRevisionRepository.GetPartRevisionById(Id);
        }

        public List<PartRevision> FindPartRevisionByProductId(int productId)
        {
            return partRevisionRepository.GetPartRevisionByProductId(productId);
        }

        public IQueryable<PartRevision> FindPartRevisionsByProductId(int productId)
        {
            return partRevisionRepository.GetPartRevisionsByProductId(productId);
        }

        public PartRevision FindPartRevisionByTaskId(int TaskId)
        {
            return partRevisionRepository.GetPartRevisionByTaskId(TaskId);
        }

        public List<PartRevision> FindPartRevisionList()
        {
            return partRevisionRepository.GetPartRevisionList();
        }

        public IQueryable<PartRevision> FindPartRevisions()
        {
            return partRevisionRepository.GetPartRevisions();
        }

        public void RemovePartRevision(PartRevision entity)
        {
            partRevisionRepository.RemovePartRevision(entity);
        }

        public void UpdatePartRevision(PartRevision entity)
        {
            partRevisionRepository.UpdatePartRevision(entity);
        }
    }
}
