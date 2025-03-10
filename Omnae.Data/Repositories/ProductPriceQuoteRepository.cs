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
    public class ProductPriceQuoteRepository : RepositoryBase<ProductPriceQuote>, IProductPriceQuoteRepository
    {
        public ProductPriceQuoteRepository(OmnaeContext dbContext) : base(dbContext)
        {

        }
        public int AddProductPriceQuote(ProductPriceQuote entity)
        {
            try
            {
                base.Add(entity);
                this.DbContext.Commit();
                return entity.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Dispose()
        {
            this.DbContext.Dispose();
        }

        public IQueryable<ProductPriceQuote> GetProductPriceQuotes(int productId, int vendorId)
        {
            return this.DbContext.ProductPriceQuotes
                .Include("PriceBreaks")
                .Include("Product")
                .Where(x => x.ProductId == productId && x.VendorId == vendorId);
        }

        public IQueryable<ProductPriceQuote> GetProductPriceQuotesById(int id)
        {
            return this.DbContext.ProductPriceQuotes
                .Include("PriceBreaks")
                .Include("Product")
                .Where(x => x.Id == id);
        }

        public IQueryable<ProductPriceQuote> GetProductPriceQuotesByProductId(int productId)
        {
            return this.DbContext.ProductPriceQuotes
                .Include("PriceBreaks")
                .Include("Product")
                .Where(x => x.ProductId == productId);
        }
        public IQueryable<ProductPriceQuote> GetProductPriceQuotesByVendorId(int vendorId)
        {
            return this.DbContext.ProductPriceQuotes
                .Include("PriceBreaks")
                .Include("Product")
                .Where(x => x.VendorId == vendorId);
        }

        public void UpdateProductPriceQuote(ProductPriceQuote entity)
        {
            base.Update(entity);
            this.DbContext.Commit();

        }
    }
}
