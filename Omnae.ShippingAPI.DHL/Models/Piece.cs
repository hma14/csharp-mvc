using System.ComponentModel.DataAnnotations;

namespace Omnae.ShippingAPI.DHL.Models
{
    public class Piece
    {
        [Display(Name = "Piece ID")]
        public int PieceID { get; set; }

        [Required]
        [RegularExpression(@"^[1-9][0-9]*$", ErrorMessage = "Height must be non-zero value")]
        public float Height { get; set; }

        [Required]
        [RegularExpression(@"^[1-9][0-9]*$", ErrorMessage = "Depth must be non-zero value")]
        public float Depth { get; set; }

        [Required]
        [RegularExpression(@"^[1-9][0-9]*$", ErrorMessage = "Width must be non-zero value")]
        public float Width { get; set; }

        [Required]
        [RegularExpression(@"^[1-9][0-9]*$", ErrorMessage = "Weight must be non-zero value")]
        public float Weight { get; set; }
    }
}
