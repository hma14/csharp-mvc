using Omnae.Common;
using Omnae.Data.Abstracts;
using Omnae.Data.Repositories.Interfaces;
using Omnae.Model.Models;
using Omnae.Service.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service
{
    public class ApprovedCapabilityService : IApprovedCapabilityService
    {
        private readonly IApprovedCapabilityRepository approvedCapabilityRepository;
        private readonly IUnitOfWork unitOfWork;

        public ApprovedCapabilityService(IApprovedCapabilityRepository approvedCapabilityRepository, IUnitOfWork unitOfWork)
        {
            this.approvedCapabilityRepository = approvedCapabilityRepository;
            this.unitOfWork = unitOfWork;
        }

        public int AddApprovedCapability(ApprovedCapability entity)
        {
            return approvedCapabilityRepository.AddApprovedCapability(entity);
        }

        public void DeleteApprovedCapability(ApprovedCapability entity)
        {
            approvedCapabilityRepository.RemoveApprovedCapability(entity);
        }

        public void Dispose()
        {
            approvedCapabilityRepository.Dispose();
        }

        public List<ApprovedCapability> FindApprovedCapabilities()
        {
            return approvedCapabilityRepository.GetApprovedCapabilities();
        }

        public List<ApprovedCapability> FindApprovedCapabilitiesByParams(BUILD_TYPE buildType, MATERIALS_TYPE materialType)
        {
            return approvedCapabilityRepository.GetApprovedCapabilitiesByParams(buildType, materialType);
        }

        public List<ApprovedCapability> FindApprovedCapabilitiesByParams(BUILD_TYPE buildType, MATERIALS_TYPE materialType, int process)
        {
            return approvedCapabilityRepository.GetApprovedCapabilitiesByParams(buildType, materialType, process);
        }

        public List<ApprovedCapability> FindApprovedCapabilitiesByParams(BUILD_TYPE buildType, Process_Type processType)
        {
            return approvedCapabilityRepository.GetApprovedCapabilitiesByParams(buildType, processType);
        }

        public ApprovedCapability FindApprovedCapabilityById(int Id)
        {
            return approvedCapabilityRepository.GetApprovedCapabilityById(Id);
        }

        public List<ApprovedCapability> FindApprovedCapabilityByVendorId(int vendorId)
        {
            return approvedCapabilityRepository.GetApprovedCapabilityByVendorId(vendorId);
        }

        public void UpdateApprovedCapability(ApprovedCapability entity)
        {
            approvedCapabilityRepository.UpdateApprovedCapability(entity);
        }
    }
}
