using Omnae.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Omnae.Model.Models
{
    public class ApprovedCapability
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Index]
        [ForeignKey("Company")]
        public int VendorId { get; set; }

        public BUILD_TYPE BuildType { get; set; }

        public MATERIALS_TYPE MaterialType { get; set; }

        public Metals_Processes? MetalProcess { get; set; }

        public Plastics_Processes? PlasticsProcess { get; set; }

        public Process_Type? ProcessType { get; set; }

        public virtual Company Company { get; set; }
    }
}
