using Libs.ViewModels;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.Libs.ViewModel
{
    public class SampleCompleteViewModel : BaseViewModel
    {       
        public string Carrier { get; set; }
        public string TrackingNumber { get; set; }
        public List<Document> Doc_Invoice { get; set; }
        public List<byte[]> InvoiceData { get; set; }
        public List<string> InvoiceName { get; set; }
        public List<string> QboEmails { get; set; }
    }
}