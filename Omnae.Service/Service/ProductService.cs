using Omnae.Service.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omnae.Common;
using Omnae.Model.Models;
using Omnae.Data.Repositories.Interfaces;
using Omnae.Data.Abstracts;
using Omnae.Model.Context;

namespace Omnae.Service.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository productRepository;
        private readonly IUnitOfWork unitOfWork;


        public ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            this.productRepository = productRepository;
            this.unitOfWork = unitOfWork;
        }

        public int AddProduct(Product entity)
        {
            return productRepository.AddProduct(entity);
        }

        public void Dispose()
        {
            productRepository.Dispose();
        }

        public Product FindProductByAdminId(string adminId)
        {
            return productRepository.GetProductByAdminId(adminId);
        }

        public List<Product> FindProductListByCustomerId(int customerId)
        {
            return productRepository.GetProductByCustomerId(customerId);
        }

        public Product FindProductById(int Id)
        {
            return productRepository.GetProductById(Id);
        }

        public List<Product> FindProductListByVendorId(int vendorId)
        {
            return productRepository.GetProductByVendorId(vendorId);
        }

        public List<Product> FindProductByCompanyId(int companyId)
        {
            return productRepository.GetProductByCompanyId(companyId);
        }

        public IQueryable<Product> FindProductList()
        {
            return productRepository.GetProductList();
        }

        public IQueryable<Product> FindProductListByCompanyId(int companyId)
        {
            return productRepository.GetProductListByCompanyId(companyId);
        }
        public IQueryable<Product> FindProductsByCustomerId(int customerId)
        {
            return productRepository.GetProductListByCustomerId(customerId);
        }
        public IQueryable<Product> FindProductsByVendorId(int vendorId)
        {
            return productRepository.GetProductListByVendorId(vendorId);
        }

        public void RemoveProduct(Product entity)
        {
            productRepository.RemoveProduct(entity);
        }

        public void UpdateProduct(Product entity)
        {
            productRepository.UpdateProduct(entity);
        }

        public Product FindProductByPartRevisionId(int partRevisionId)
        {
            return productRepository.GetProductByPartRevisionId(partRevisionId);
        }

        public void DetachProduct(Product entity)
        {
            productRepository.DetachProduct(entity);
        }

        public Product FindProductByPartNumberOfACustomer(int customerId, string partNumber, string partNumberRevision)
        {
            return productRepository.GetProductByPartNumberOfACustomer(customerId, partNumber, partNumberRevision);
        }
        public Product FindProductByPartNumber(int vendorId, int customerId, string partNumber,string partNumberRevision)
        {
            return productRepository.GetProductByPartNumber(vendorId, customerId, partNumber, partNumberRevision);

        }
        public Product FindProductByPartNumber(string partNumber, string partNumberRevision)
        {
            return productRepository.GetProductByPartNumber(partNumber, partNumberRevision);
        }

        public bool IsUsersProduct(Product product, ILogedUserContext UserContext, ICompanyService companyService, RFQBid bid = null)
        {
            var currentUserId = UserContext.UserId;
            var company = companyService.FindCompanyByUserId(currentUserId);

            return (product.CustomerId == company.Id || product.VendorId == company.Id || bid != null && bid.VendorId == UserContext.Company.Id);
        }

        public IQueryable<Product> HasUniqueProducts(int customerId, string partNumber, string partNumberRevision)
        {
            return productRepository.HasUniqueProducts(customerId, partNumber, partNumberRevision);

        }
    }

}
