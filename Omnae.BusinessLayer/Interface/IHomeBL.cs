using Omnae.BusinessLayer.Models;
using Omnae.Common;
using Omnae.Model.Models;
using Omnae.QuickBooks.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static Omnae.Data.Query.OrderQuery;

namespace Omnae.BusinessLayer.Interface
{
    public interface IHomeBL
    {
        Task<string> TaskStateHandler(StateTransitionViewModel model, List<HttpPostedFileBase> files = null);
        Task<int> RFQBidReview(RFQViewModel model, List<HttpPostedFileBase> files);
        string DoRFQRevision(TaskData taskData, string revisingReason, USER_TYPE userType, List<HttpPostedFileBase> files = null);
        UserPerformanceViewModel GetUserPerformance(int fromCompanyId, CompanyType companyType, int? toCompanyId = null);
        PackingSlipUriViewModel CreatePackingSlip(TaskData td, ControllerContext controllerContext, Order order);
        string NcrDetails(int id, ref NcrDescriptionViewModel model);
        string AssignCustomerTerms(CompaniesCreditRelationshipViewModel model);
        string AssignTermCreditLimit(CompaniesCreditRelationshipViewModel model);
        string RemoveTermCreditLimit(RemoveCreditRelationshipViewModel model);
        IQueryable<TaskData> Search(string search, int companyId);
        IList<Document> GetDocumentsByProductId(int productId);
        IEnumerable<CompanyDTO> GetUserPerformanceWrapper(IEnumerable<CompanyDTO> vendors, CompanyType companyType, int? toCompanyId = null);
        IEnumerable<CompanyDTO> GetUserPerformanceByProductIdWrapper(IEnumerable<CompanyDTO> vendors, IQueryable<Order> orders, IQueryable<NCReport> ncrs);
       
        Task SendNotifications(TaskData taskData, string destination, string destinationSms,
                                      bool isAdmin = false, List<byte[]> attachmentData = null,
                                      List<string> VendorInvoicesNumbers = null);

        CustomerInfo CreateUsersForQBO(int id);
        CustomerInfo CreateUsersForQBO(int id, CurrencyCodes currencyCode, int? termDays);
        Task CreateUserInQBO(CustomerInfoViewModel mv);
        Task CreateUserInQBO(int id);
        CustomerInfo AddCustomerQBO(CreateCustomerInfoViewModel model);
        Task<CompanyQualityAnalyticsStatisticsViewModel> GetCompanyQualityAnalyticsStatistics(IQueryable<Order> orders, IQueryable<NCReport> ncrs, int companyId, UserMode mode);
    }
}
