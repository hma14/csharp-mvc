using Omnae.ViewModels;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Omnae.ViewModels
{
    public class StripeChargeViewModel
    {
        [Required]
        public string Token { get; set; }
        public int? TaskId { get; set; }
        public int ProductId { get; set; }


        [DisplayFormat(DataFormatString = "{0:C0}", ApplyFormatInEditMode = true)]
        public decimal Amount { get; set; } 

        public string Currency { get; set; }

        // These fields are optional and are not 
        // required for the creation of the token

        [Required]
        [Display(Name = "Card Holder Name")]
        public string CardHolderName { get; set; }

        public BillingAddressViewModel address { get; set; }
        public int OrderId { get; set; }
        public bool? IsReorder { get; set; }

    }
}
