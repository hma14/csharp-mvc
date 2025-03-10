using Omnae.Common;
using Omnae.Data.Abstracts;
using Omnae.Model.Models;
using System.Collections.Generic;

namespace Omnae.Data.Repositories.Interfaces
{
    public interface IApprovedCapabilityRepository : IRepository<ApprovedCapability>
    {
        void Dispose();
        int AddApprovedCapability(ApprovedCapability entity);
        ApprovedCapability GetApprovedCapabilityById(int Id);
        List<ApprovedCapability> GetApprovedCapabilityByVendorId(int vendorId);
        List<ApprovedCapability> GetApprovedCapabilities();
        List<ApprovedCapability> GetApprovedCapabilitiesByParams(BUILD_TYPE buildType, MATERIALS_TYPE materialType);
        List<ApprovedCapability> GetApprovedCapabilitiesByParams(BUILD_TYPE buildType, MATERIALS_TYPE materialType, int process);

        void UpdateApprovedCapability(ApprovedCapability entity);
        void RemoveApprovedCapability(ApprovedCapability entity);
        List<ApprovedCapability> GetApprovedCapabilitiesByParams(BUILD_TYPE buildType, Process_Type processType);
    }
}
