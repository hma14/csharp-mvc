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
    public class RFQQuantityService : IRFQQuantityService
    {
        private readonly IRFQQuantityRepository rFQQuantityRepository;
        private readonly IUnitOfWork unitOfWork;

        public RFQQuantityService(IRFQQuantityRepository rFQQuantityRepository, IUnitOfWork unitOfWork)
        {
            this.rFQQuantityRepository = rFQQuantityRepository;
            this.unitOfWork = unitOfWork;
        }

        public int AddRFQQuantity(RFQQuantity entity)
        {
            return rFQQuantityRepository.AddRFQQuantity(entity);
        }

        public void Dispose()
        {
            rFQQuantityRepository.Dispose();
        }

        public RFQQuantity FindRFQQuantityById(int Id)
        {
            return rFQQuantityRepository.GetRFQQuantityById(Id);
        }

        public List<RFQQuantity> FindRFQQuantityList()
        {
            return rFQQuantityRepository.GetRFQQuantityList();
        }

        public void UpdateRFQQuantity(RFQQuantity entity)
        {
            rFQQuantityRepository.UpdateRFQQuantity(entity);
        }
    }
}
