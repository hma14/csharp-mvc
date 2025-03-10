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
    public class ExtraQuantityService : IExtraQuantityService
    {
        private readonly IExtraQuantityRepository extraQuantityService;
        private readonly IUnitOfWork unitOfWork;

        public ExtraQuantityService(IExtraQuantityRepository extraQuantityService, IUnitOfWork unitOfWork)
        {
            this.extraQuantityService = extraQuantityService;
            this.unitOfWork = unitOfWork;
        }

        public int AddExtraQuantity(ExtraQuantity entity)
        {
            return extraQuantityService.AddExtraQuantity(entity);
        }

        public void Dispose()
        {
            extraQuantityService.Dispose();
        }

        public ExtraQuantity FindExtraQuantityById(int Id)
        {
            return extraQuantityService.GetExtraQuantityById(Id);
        }

        public List<ExtraQuantity> FindExtraQuantityList()
        {
            return extraQuantityService.GetExtraQuantityList();
        }

        public void UpdateExtraQuantity(ExtraQuantity entity)
        {
            extraQuantityService.UpdateExtraQuantity(entity);
        }
    }
}
