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
    public class ProductStateTrackingRepository : RepositoryBase<ProductStateTracking>, IProductStateTrackingRepository
    {
        public ProductStateTrackingRepository(OmnaeContext dbContext) : base(dbContext)
        {

        }
        public int AddProductStateTracking(ProductStateTracking entity)
        {
            base.Add(entity);
            this.DbContext.Commit();
            return entity.Id;
        }

        public void Dispose()
        {
            this.DbContext.Dispose();
        }

        public ProductStateTracking GetProductStateTrackingById(int id)
        {
            return this.DbContext.ProductStateTrackings.Where(x => x.Id == id).FirstOrDefault();
        }

        public List<ProductStateTracking> GetProductStateTrackingByProductId(int productId)
        {
            var productStateTrackings = this.DbContext.ProductStateTrackings
                .Include("Product")
                .Where(x => x.ProductId == productId);

            productStateTrackings = productStateTrackings.OrderBy(x => x.StateId);
            return productStateTrackings.ToList();
        }

        public IQueryable<ProductStateTracking> GetProductStateTrackingByState(int stateId)
        {
            return this.DbContext.ProductStateTrackings
                .Include("Product")
                .Where(x => x.StateId == stateId);
        }
    }
}
