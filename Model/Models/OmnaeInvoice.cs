using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Omnae.Model.Models
{
    public class OmnaeInvoice
    {
        private string _poDocUri;

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Index("IX_TaskId")]
        public int TaskId { get; set; }

        [Index("IX_OrderId")]
        public int OrderId { get; set; }
        
        public int UserType { get; set; }

        [Index("IX_CompanyId")]
        public int CompanyId { get; set; }
        
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal ToolingSetupCharges { get; set; }
        public decimal SalesTax { get; set; }
        public int ProductId { get; set; }
        public string EstimateId { get; set; }
        public string EstimateNumber { get; set; }
        public string InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public string BillId { get; set; }
        public string BillNumber { get; set; }
        public string PONumber { get; set; }


        [NotMapped]
        [Column(name: "PODocUri", TypeName = "VARCHAR")]
        public string PODocUri
        {
            get => _poDocUri = _poDocUri ?? PODocUriFromBd;
            set
            {
                _poDocUri = value;
                PODocUriFromBd = value?.Split(new[] {'?'}, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            }
        }

        [Column(name: "PODocUri", TypeName = "VARCHAR")]
        public string PODocUriFromBd { get; set; }


        public bool IsOpen { get; set; }
        public int? Term { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentRefNumber { get; set; }

        public DateTime? CloseDate { get; set; }

        // Navigation Property

        public virtual TaskData Task { get; set; }

        public virtual Order Order { get; set; }

        public virtual Company Company { get; set; }

    }
}
