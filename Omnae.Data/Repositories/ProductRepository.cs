using Omnae.Model.Models;
using Omnae.Data.Abstracts;
using Omnae.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Data.Repositories
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(OmnaeContext dbContext) : base(dbContext)
        {

        }
        public int AddProduct(Product entity)
        {
            //entity.Status = 1;
            base.Add(entity);
            this.DbContext.Commit();  // must call commit here otherwise, entity won't be added      
            return entity.Id;
        }

        public void Dispose()
        {
            this.DbContext.Dispose();
        }

        public Product GetProductByAdminId(string adminId)
        {
            return this.DbContext.Products.Include("Documents").Where(x => x.AdminId == adminId).FirstOrDefault();
        }

        public Product GetProductById(int Id)
        {
            return this.DbContext.Products
                .Include("Documents")
                .Include("PartRevision")
                .AsNoTracking().Where(x => x.Id == Id).FirstOrDefault();
        }

        public List<Product> GetProductByCustomerId(int customerId)
        {
            var products = this.DbContext.Products.Include("Documents").Include("PriceBreak").Where(x => x.CustomerId == customerId);
            var list = products.OrderByDescending(x => x.Id).ToList();
            return list;
        }
        public List<Product> GetProductByVendorId(int vendorId)
        {
            var products = this.DbContext.Products.Include("Documents").Where(x => x.VendorId == vendorId);
            var list = products.OrderByDescending(x => x.Id).ToList();
            return list;
        }

        public List<Product> GetProductByCompanyId(int companyId)
        {
            var products = this.DbContext.Products.Include("Documents").Include("PriceBreak").Where(x => x.VendorId == companyId || x.CustomerId == companyId);
            var list = products.OrderByDescending(x => x.Id).ToList();
            return list;
        }

        public IQueryable<Product> GetProductList()
        {
            return this.DbContext.Products.Include("Documents");
        }

        public IQueryable<Product> GetProductListByCompanyId(int companyId)
        {
            return this.DbContext.Products.Include("Documents").Where(u => u.CustomerCompany.Id == companyId || u.VendorCompany.Id == companyId);
        }
        public IQueryable<Product> GetProductListByCustomerId(int customerId)
        {
            return this.DbContext.Products.Include("Documents").Where(u => u.CustomerId == customerId);
        }
        public IQueryable<Product> GetProductListByVendorId(int vendorId)
        {
            return this.DbContext.Products.Include("Documents").Where(u => u.VendorId == vendorId);
        }

        public void RemoveProduct(Product entity)
        {
            var ent = this.DbContext.Products.Where(x => x.Id == entity.Id).FirstOrDefault();
            base.Delete(ent);
            this.DbContext.Commit();
        }

        public void UpdateProduct(Product entity)
        {
            base.Update(entity);
            this.DbContext.Commit();  // must call commit here otherwise, entity won't be added
        }

        public void DetachProduct(Product entity)
        {
            base.Detach(entity);
        }

        public Product GetProductByPartRevisionId(int partRevisionId)
        {
            return this.DbContext.Products.Include("Documents").Where(x => x.PartRevisionId == partRevisionId).FirstOrDefault();
        }

        public Product GetProductByPartNumberOfACustomer(int customerId, string partNumber, string partNumberRevision)
        {
            return DbContext.Products.Include("PriceBreak").Where(x => x.CustomerId == customerId && x.PartNumber == partNumber && x.PartNumberRevision == partNumberRevision).FirstOrDefault();
        }
        public Product GetProductByPartNumber(int vendorId, int customerId, string partNumber, string partNumberRevision)
        {
            return DbContext.Products.Include("PriceBreak").Where(x => x.CustomerId == customerId
                                                                       && x.VendorId == vendorId
                                                                       && x.PartNumber == partNumber 
                                                                       && x.PartNumberRevision == partNumberRevision).FirstOrDefault();
        }

        public Product GetProductByPartNumber(string partNumber, string partNumberRevision)
        {
            return DbContext.Products.Where(x => x.PartNumber == partNumber && x.PartNumberRevision == partNumberRevision).FirstOrDefault();
        }
        public IQueryable<Product> GetProductsByPartNumber(string partNumber)
        {
            return DbContext.Products.Where(x => x.PartNumber == partNumber);
        }

        public IQueryable<Product> HasUniqueProducts(int customerId, string partNumber, string partNumberRevision)
        {
            return DbContext.Products.Where(x => x.CustomerId == customerId && x.PartNumber == partNumber && x.PartNumberRevision == partNumberRevision);
        }
    }
}
