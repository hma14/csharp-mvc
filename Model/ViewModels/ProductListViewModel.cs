using Omnae.Common;
using System.ComponentModel.DataAnnotations;

namespace Omnae.Model.ViewModels
{
    public class ProductListViewModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [Display(Name = "Avatar Uri")]
        public string AvatarUri { get; set; }

        [Display(Name = "Part Number")]
        public int PartNumber { get; set; }

        [Display(Name = "Part Number Revision")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "PartNumberRevision can not have charactors, such as 1, 21, .. or  1.1, 5.2.1 ...")]
        public string PartNumberRevision { get; set; }


        [Display(Name = "Build Type")]
        public BUILD_TYPE BuildType { get; set; }

        [Display(Name = "Select Material")]
        public MATERIALS_TYPE Material { get; set; }

        public decimal UnitPrice { get; set; }

        //public PriceBreak PriceBreak { get; set; }
        //public List<Document> Documents { get; set; }

        public int? OrderId { get; set; }

        public int? TaskId { get; set; }

        public bool? IsRiskBuild { get; set; }
    }
}