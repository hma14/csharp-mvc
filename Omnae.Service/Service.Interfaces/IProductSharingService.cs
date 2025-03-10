using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service.Interfaces
{
    public interface IProductSharingService
    {
        void Dispose();
        int AddProductSharing(ProductSharing entity);
        ProductSharing UpdateProductSharing(ProductSharing entity);
        ProductSharing Find(int id);
        IQueryable<ProductSharing> FindProductSharingsByComanyId(int companyId);
        IQueryable<Product> FindCompanySharingProductsByComanyId(int companyId);
        IQueryable<Company> FindSharingCompaniesBySharedProductId(int productId);
        void ModifyProductSharingStatus(ProductSharing entity, bool toRevoke);
        ProductSharing FindProductSharingByCompanyIdProductId(int companyId, int productId, bool ignoreIsRevoked = false);
        IQueryable<Product> FindProductsByComanyId(int companyId);
        void RemoveProductSharing(ProductSharing entity);
        IQueryable<TaskData> FindProductSharingTaskDatasByComanyId(int companyId);
        IQueryable<ProductSharing> FindCustomersOfSharedProductByComanyId(int companyId, int productId);
        IQueryable<ProductSharing> QueryProductShares(int productId);
        IQueryable<ProductSharing> FindSharingsByProductId(int productId);
    }
}
