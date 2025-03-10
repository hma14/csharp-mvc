using Intuit.Ipp.Data;
using System.Web.Mvc;

namespace Omnae.QuickBooks.ViewModels
{
    public class AddExistingUsersToQBOViewModel
    {
        public int[] UserIds { get; set; }
        public bool Taxable { get; set; }
        public decimal Balance { get; set; }
        public decimal BalanceWithJobs { get; set; }
        public string Notes { get; set; }
        public string Title { get; set; }
        

        public TelephoneNumber AlternatePhone { get; set; }
        public TelephoneNumber Mobile { get; set; }
        public TelephoneNumber Fax { get; set; }
        public WebSiteAddress WebAddr { get; set; }
        public MultiSelectList DdlCustomers { get; set; }
    }
}