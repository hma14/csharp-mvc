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
    public class CustomerUploadMissingFilesViewModel
    {
        public int ProductId { get; set; }
        public CUSTOMER_MISSING_DOCUMENT_TYPE DocType { get; set; }
        public SelectList ProductDDL { get; set; }
        public IEnumerable<Document> Documents { get; set; }
    }
}
