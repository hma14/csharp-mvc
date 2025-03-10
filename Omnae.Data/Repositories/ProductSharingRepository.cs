using Omnae.Data.Abstracts;
using Omnae.Data.Repositories.Interfaces;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Data.Repositories
{
    public class ProductSharingRepository :
        RepositoryBase<ProductSharing>, IProductSharingRepository
    {
        public ProductSharingRepository(OmnaeContext dbContext) : base (dbContext)
        {

        }
        public int AddProductSharing(ProductSharing entity)
        {
            base.Add(entity);
            this.DbContext.Commit();
            return entity.Id;
        }

        public void ModifyProductSharingStatus(ProductSharing entity, bool toRevoke)
        {
            entity.IsRevoked = toRevoke;
            this.DbContext.Commit();
        }

        public void Dispose()
        {
            this.DbContext.Dispose();
        }

        public IQueryable<Product> GetCompanySharingProductsByComanyId(int id)
        {
            return this.DbContext.ProductSharings
                        .Include("ProductSharingCompany")
                        .Include("ProductOwnerCompany")
                        .Include("Product")
                        .Where(x => x.SharingCompanyId == id && x.IsRevoked != true)
                        .Select(x => x.Product);
        }

        public IQueryable<ProductSharing> GetCustomersOfSharedProductByComanyId(int companyId, int productId)
        {
            return this.DbContext.ProductSharings
                        .Include("ProductSharingCompany")
                        .Where(x => x.OwnerCompanyId == companyId && x.ProductId == productId && x.IsRevoked != true);
        }

        public IQueryable<ProductSharing> QueryProductShares(int productId)
        {
            return DbContext.ProductSharings.Where(x => x.ProductId == productId && x.IsRevoked != true);
        }


        public ProductSharing GetProductSharingById(int id)
        {
            return DbContext.ProductSharings
                .Include("ProductSharingCompany")
                .Include("ProductOwnerCompany")
                .Include("Product")
                .Include("TaskData").FirstOrDefault(x => x.Id == id && x.IsRevoked != true);
        }
        public ProductSharing GetProductSharingByCompanyIdProductId(int companyId, int productId, bool ignoreIsRevoked)
        {
            return ignoreIsRevoked
                ? DbContext.ProductSharings
                    .Include("ProductSharingCompany")
                    .Include("ProductOwnerCompany")
                    .Include("Product")
                    .Include("TaskData")
                    .FirstOrDefault(x => x.SharingCompanyId == companyId && x.ProductId == productId)
                : DbContext.ProductSharings
                    .Include("ProductSharingCompany")
                    .Include("ProductOwnerCompany")
                    .Include("Product")
                    .Include("TaskData").FirstOrDefault(x =>
                        x.SharingCompanyId == companyId && x.ProductId == productId && x.IsRevoked != true);
        }

        public IQueryable<ProductSharing> GetProductSharingsByComanyId(int id)
        {
            return this.DbContext.ProductSharings
                        .Include("ProductSharingCompany")
                        .Include("ProductOwnerCompany")
                        .Include("Product")
                        .Where(x => x.SharingCompanyId == id && x.IsRevoked != true);
        }

        public IQueryable<Company> GetSharingCompaniesBySharedProductId(int productId)
        {
            return this.DbContext.ProductSharings
                        .Include("ProductSharingCompany")
                        .Include("ProductOwnerCompany")
                        .Include("Product")
                        .Include("TaskData")
                        .Where(x => x.ProductId == productId && x.IsRevoked != true && x.HasPermissionToOrder == true)
                        .Select(x => x.ProductSharingCompany);
        }
        public IQueryable<ProductSharing> GetSharingsByProductId(int productId)
        {
            return this.DbContext.ProductSharings
                        .Include("ProductSharingCompany")
                        .Include("ProductOwnerCompany")
                        .Include("Product")
                        .Include("TaskData")
                        .Where(x => x.ProductId == productId && x.IsRevoked != true && x.HasPermissionToOrder == true);
        }

        public ProductSharing UpdateProductSharing(ProductSharing entity)
        {
            base.Update(entity);
            this.DbContext.Commit();
            return entity;
        }

        public IQueryable<Product> GetProductsByComanyId(int companyId)
        {
            return this.DbContext.ProductSharings
                       .Include("Product")
                       .Include("TaskData")
                       .Where(x => x.SharingCompanyId == companyId && x.IsRevoked != true)
                       .Select(x => x.Product);
        }

        public void DeleteProductSharing(ProductSharing entity)
        {
            base.Delete(entity);
            this.DbContext.Commit();
        }

        public IQueryable<TaskData> GetProductSharingTaskDatasByComanyId(int companyId)
        {
            return this.DbContext.ProductSharings
                       .Include("Product")
                       .Include("TaskData")
                       .Where(x => x.SharingCompanyId == companyId && x.IsRevoked != true)
                       .Select(x => x.TaskData);
        }
    }
}
