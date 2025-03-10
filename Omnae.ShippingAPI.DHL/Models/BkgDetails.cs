using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Omnae.ShippingAPI.DHL.Models
{
    public class BkgDetails
    {
        [Display(Name = "Payment Country Code")]
        public string PaymentCountryCode { get; set; }
        public string Date { get; set; }
        public string ReadyTime { get; set; }
        public string ReadyTimeGMTOffset { get; set; }

        [Display(Name = "Dimension Unit")]
        public string DimensionUnit { get; set; }

        [Display(Name = "Weight Unit")]
        public string WeightUnit { get; set; }

        [Display(Name = "Shipment Weight")]
        public float? ShipmentWeight { get; set; }

        public float? Volume { get; set; }

        [Display(Name = "Number of Parts Per Carton or Box can hold *")]
        [RegularExpression(@"^[1-9][0-9]*$", ErrorMessage = "Number of Parts Per Carton or Boxe can hold is required and must be greater than 0")]
        public int NumberPartsPerPiece { get; set; }

        [Display(Name = "Pieces or Cartons, Boxes")]
        public List<Piece> Pieces { get; set; }

        [Required(ErrorMessage = "Number of Cartons or Boxes Needed Per MOQ Parts is required")]
        [Display(Name ="Number of Cartons or Boxes Needed Per MOQ Parts *")]
        public int NumberPieces { get; set; }

        [Display(Name = "Is Dutiable")]
        public string IsDutiable { get; set; }
        
        public QtdShp QtdShp { get; set; }

        [Display(Name = "Network Type Code (option)")]
        public string NetworkTypeCode { get; set; }

        [Display(Name = "Insured Value (option)")]
        public string InsuredValue { get; set; }

        [Display(Name = "Insured Currency (option)")]
        public string InsuredCurrency { get; set; }

        public string PaymentAccountNumber { get; set; }

        public IEnumerable<SelectListItem> DdlDimensionUnit { get; set; }
        public IEnumerable<SelectListItem> DdlWeightUnit { get; set; }
        public IEnumerable<SelectListItem> DdlIsDutiable { get; set; }

    }
}
