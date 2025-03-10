using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Data.Repositories.Interfaces
{
    public interface IProductSharingRepository
    {
        void Dispose();

        int AddProductSharing(ProductSharing entity);
        ProductSharing UpdateProductSharing(ProductSharing entity);
        ProductSharing GetProductSharingById(int id);
        IQueryable<ProductSharing> GetProductSharingsByComanyId(int id);
        IQueryable<Product> GetCompanySharingProductsByComanyId(int id);
        IQueryable<Company> GetSharingCompaniesBySharedProductId(int productId);
        void ModifyProductSharingStatus(ProductSharing entity, bool toRevoke);
        ProductSharing GetProductSharingByCompanyIdProductId(int companyId, int productId, bool ignoreIsRevoked);
        IQueryable<Product> GetProductsByComanyId(int companyId);
        void DeleteProductSharing(ProductSharing entity);
        IQueryable<TaskData> GetProductSharingTaskDatasByComanyId(int companyId);
        IQueryable<ProductSharing> GetCustomersOfSharedProductByComanyId(int companyId, int productId);
        IQueryable<ProductSharing> QueryProductShares(int productId);
        IQueryable<ProductSharing> GetSharingsByProductId(int productId);
    }
}
