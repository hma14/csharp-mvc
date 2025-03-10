using Omnae.Data.Abstracts;
using Omnae.Data.Repositories.Interfaces;
using Omnae.Model.Models;
using Omnae.Service.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service
{
    public class ProductSharingService : IProductSharingService
    {
        private readonly IProductSharingRepository repository;
        private readonly IUnitOfWork unitOfWork;

        public ProductSharingService(IProductSharingRepository repository, IUnitOfWork unitOfWork)
        {
            this.repository = repository;
            this.unitOfWork = unitOfWork;
        }
        public int AddProductSharing(ProductSharing entity)
        {
            return repository.AddProductSharing(entity);
        }

        public void Dispose()
        {
            repository.Dispose();
        }      

        public IQueryable<Product> FindCompanySharingProductsByComanyId(int companyId)
        {
            return repository.GetCompanySharingProductsByComanyId(companyId);
        }

        public ProductSharing FindProductSharingByCompanyIdProductId(int companyId, int productId, bool ignoreIsRevoked = false)
        {
            return repository.GetProductSharingByCompanyIdProductId(companyId, productId, ignoreIsRevoked);
        }

        public ProductSharing Find(int id)
        {
            return repository.GetProductSharingById(id);
        }

        public IQueryable<ProductSharing> FindCustomersOfSharedProductByComanyId(int companyId, int productId)
        {
            return repository.GetCustomersOfSharedProductByComanyId(companyId,productId);
        }

        public IQueryable<ProductSharing> QueryProductShares(int productId)
        {
            return repository.QueryProductShares(productId);
        }

        public IQueryable<Product> FindProductsByComanyId(int companyId)
        {
            return repository.GetProductsByComanyId(companyId);
        }
        public IQueryable<ProductSharing> FindProductSharingsByComanyId(int companyId)
        {
            return repository.GetProductSharingsByComanyId(companyId);
        }

        public IQueryable<Company> FindSharingCompaniesBySharedProductId(int productId)
        {
            return repository.GetSharingCompaniesBySharedProductId(productId);
        }
        public IQueryable<TaskData> FindProductSharingTaskDatasByComanyId(int companyId)
        {
            return repository.GetProductSharingTaskDatasByComanyId(companyId);
        }

        public void ModifyProductSharingStatus(ProductSharing entity, bool toRevoke)
        {
            repository.ModifyProductSharingStatus(entity, toRevoke);
        }

        public ProductSharing UpdateProductSharing(ProductSharing entity)
        {
            return repository.UpdateProductSharing(entity);
        }

        public void RemoveProductSharing(ProductSharing entity)
        {
            repository.DeleteProductSharing(entity);
        }
        public IQueryable<ProductSharing> FindSharingsByProductId(int productId)
        {
            return repository.GetSharingsByProductId(productId);
        }
    }
}
