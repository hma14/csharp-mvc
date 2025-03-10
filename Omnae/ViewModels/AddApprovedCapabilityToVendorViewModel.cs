using Omnae.Common;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Omnae.ViewModels
{
    public class AddApprovedCapabilityToVendorViewModel
    {
        public int VendorId { get; set; }
        public BUILD_TYPE BuildType { get; set; }

        public MATERIALS_TYPE MaterialType { get; set; }


        public Metals_Processes? MetalProcess { get; set; }

        public Plastics_Processes? PlasticsProcess { get; set; }
        
        // Process Type 
        public Process_Type? ProcessType { get; set; }

        public Anodizing_Type? Anodizing { get; set; }

        public SelectList VendorList { get; set; }
        public List<ApprovedCapability> ExistingApprovedCapabilities { get; set; }
    }
}