using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.ViewModels
{
    public class DocumentProdIdViewModel
    {       
        public int ProductId { get; set; }
        public int TaskId { get; set; }
        public List<Document> Documents { get; set; }
    }
}