using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Omnae.Model.Models
{
    public class NCRImages
    {
        private string _imageUrl;

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("NCReport")]
        public int NCReportId { get; set; }

        [NotMapped]
        [Column(name: "ImageUrl", TypeName = "VARCHAR")]
        public string ImageUrl
        {
            get => _imageUrl = _imageUrl ?? ImageUrlFromBd;
            set
            {
                _imageUrl = value;
                ImageUrlFromBd = value?.Split(new[] { '?' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            }
        }

        [Column(name: "ImageUrl", TypeName = "VARCHAR")]
        public string ImageUrlFromBd { get; set; }

        public int Type { get; set; }

        public virtual NCReport NCReport { get; set; }

    }
}
