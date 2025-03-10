using Omnae.Model.Models;
using Omnae.Data.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Data.Repositories.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        void Dispose();
        int AddProduct(Product entity);
        List<Product> GetProductByCustomerId(int customerId);
        List<Product> GetProductByVendorId(int vendorId);
        Product GetProductByAdminId(string adminId);
        Product GetProductById(int Id);
        IQueryable<Product> GetProductList();
        IQueryable<Product> GetProductListByCompanyId(int companyId);
        IQueryable<Product> GetProductListByCustomerId(int customerId);
        IQueryable<Product> GetProductListByVendorId(int vendorId);
        List<Product> GetProductByCompanyId(int companyId);
        void UpdateProduct(Product entity);
        void RemoveProduct(Product entity);
        Product GetProductByPartRevisionId(int partRevisionId);
        void DetachProduct(Product entity);
        
        Product GetProductByPartNumberOfACustomer(int customerId, string partNumber, string partNumberRevision);
        Product GetProductByPartNumber(int vendorId, int customerId, string partNumber, string partNumberRevision);
        Product GetProductByPartNumber(string partNumber, string partNumberRevision);
        IQueryable<Product> HasUniqueProducts(int customerId, string partNumber, string partNumberRevision);
    }
}
