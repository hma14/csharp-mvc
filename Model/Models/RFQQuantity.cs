using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Model.Models
{
    public class RFQQuantity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //[Required(ErrorMessage = "Quantity 1 is required")]
        [Display(Name = "Qty 1")]
        [DisplayFormat(DataFormatString = "{0:n3}", ApplyFormatInEditMode = true)]
        //[RegularExpression(@"^([1-9][0]{1,4})", ErrorMessage = "MaxQuantity must be between 10 and 90000, such that 10, 20 ..., or 100, 200 ..., 90000, ")]
        public decimal? Qty1 { get; set; }

        [Display(Name = "Qty 2")]
        [DisplayFormat(DataFormatString = "{0:n3}", ApplyFormatInEditMode = true)]
        public decimal? Qty2 { get; set; }

        [Display(Name = "Qty 3")]
        [DisplayFormat(DataFormatString = "{0:n3}", ApplyFormatInEditMode = true)]
        public decimal? Qty3 { get; set; }

        [Display(Name = "Qty 4")]
        [DisplayFormat(DataFormatString = "{0:n3}", ApplyFormatInEditMode = true)]
        public decimal? Qty4 { get; set; }

        [Display(Name = "Qty 5")]
        [DisplayFormat(DataFormatString = "{0:n3}", ApplyFormatInEditMode = true)]
        public decimal? Qty5 { get; set; }

        [Display(Name = "Qty 6")]
        [DisplayFormat(DataFormatString = "{0:n3}", ApplyFormatInEditMode = true)]
        public decimal? Qty6 { get; set; }

        [Display(Name = "Qty 7")]
        [DisplayFormat(DataFormatString = "{0:n3}", ApplyFormatInEditMode = true)]
        public decimal? Qty7 { get; set; }

        public bool IsAddedExtraQty { get; set; }

        public int UnitOfMeasurement { get; set; }

    }
}
