using Intuit.Ipp.Data;

namespace Omnae.QuickBooks.ViewModels
{
    public class CustomerInfo
    {
        public bool Taxable { get; set; }
        public PhysicalAddress BillAddr { get; set; }
        public PhysicalAddress ShipAddr { get; set; }
        public int? Term { get; set; }
        public int PaymentMethod { get; set; }
        public decimal Balance { get; set; }
        public decimal BalanceWithJobs { get; set; }
        public string Notes { get; set; }
        public string Title { get; set; }
        public string GivenName { get; set; }
        public string MiddleName { get; set; }
        public string FamilyName { get; set; }
        public string Suffix { get; set; }
        public string FullyQualifiedName { get; set; }
        public string CompanyName { get; set; }
        public string DisplayName { get; set; }
        public string PrintOnCheckName { get; set; }
        public bool Active { get; set; }
        public TelephoneNumber primaryPhone { get; set; }
        public string FreeFormNumber { get; set; }
        public TelephoneNumber alternatePhone { get; set; }
        public TelephoneNumber mobile { get; set; }
        public TelephoneNumber fax { get; set; }
        public EmailAddress primaryEmailAddr { get; set; }
        public WebSiteAddress webAddr { get; set; }
        //public string CurrencyName { get; set; } // i.e.: United States Dollar, Canadian Dollar
        public string CurrencyText { get; set; } // i.e.: USD, CAD

    }
}
