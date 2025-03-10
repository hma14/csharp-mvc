using Omnae.Common;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Omnae.BusinessLayer.Models
{
    public class UploadMissingNcrImagesViewModel
    {
        public int NcrId { get; set; }
        public NCR_IMAGE_TYPE DocType { get; set; }
        public SelectList NcrDDL { get; set; }
        public IEnumerable<NCRImages> NcrImages { get; set; }
    }
}
