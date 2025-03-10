using Omnae.Model.Context;
using Omnae.Model.Models;
using System.Collections.Generic;
using System.Linq;

namespace Omnae.Service.Service.Interfaces
{
    public interface IProductService
    {
        void Dispose();
        int AddProduct(Product entity);
        List<Product> FindProductListByCustomerId(int customerId);
        List<Product> FindProductListByVendorId(int vendorId);
        Product FindProductByAdminId(string adminId);
        Product FindProductById(int Id);
        IQueryable<Product> FindProductList();
        IQueryable<Product> FindProductListByCompanyId(int companyId);
        IQueryable<Product> FindProductsByCustomerId(int customerId);
        IQueryable<Product> FindProductsByVendorId(int vendorId);
        void UpdateProduct(Product entity);
        void RemoveProduct(Product entity);
        Product FindProductByPartRevisionId(int partRevisionId);
        void DetachProduct(Product entity);

        Product FindProductByPartNumberOfACustomer(int customerId, string partNumber, string partNumberRevision);
        Product FindProductByPartNumber(int vendorId, int customerId, string partNumber, string partNumberRevision);
        Product FindProductByPartNumber(string partNumber, string partNumberRevision);

        bool IsUsersProduct(Product product, ILogedUserContext UserContext, ICompanyService companyService, RFQBid bid = null);

        IQueryable<Product> HasUniqueProducts(int customerId, string partNumber, string partNumberRevision);
    }
}
