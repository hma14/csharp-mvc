using Omnae.Common;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service.Interfaces
{
    public interface IApprovedCapabilityService
    {
        void Dispose();
        int AddApprovedCapability(ApprovedCapability entity);
        ApprovedCapability FindApprovedCapabilityById(int Id);
        List<ApprovedCapability> FindApprovedCapabilityByVendorId(int vendorId);
        List<ApprovedCapability> FindApprovedCapabilities();
        List<ApprovedCapability> FindApprovedCapabilitiesByParams(BUILD_TYPE buildType, MATERIALS_TYPE materialType);
        List<ApprovedCapability> FindApprovedCapabilitiesByParams(BUILD_TYPE buildType, MATERIALS_TYPE materialType, int process);

        void UpdateApprovedCapability(ApprovedCapability entity);
        void DeleteApprovedCapability(ApprovedCapability entity);
        List<ApprovedCapability> FindApprovedCapabilitiesByParams(BUILD_TYPE buildType, Process_Type processType);
    }
}
