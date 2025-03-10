using Omnae.Data.Abstracts;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Data.Repositories.Interfaces
{
    public interface IProductPriceQuoteRepository : IRepository<ProductPriceQuote>
    {
        void Dispose();
        int AddProductPriceQuote(ProductPriceQuote entity);
        IQueryable<ProductPriceQuote> GetProductPriceQuotesById(int id);
        IQueryable<ProductPriceQuote> GetProductPriceQuotesByProductId(int productId);
        IQueryable<ProductPriceQuote> GetProductPriceQuotes(int productId, int vendorId);
        IQueryable<ProductPriceQuote> GetProductPriceQuotesByVendorId(int vendorId);
        void UpdateProductPriceQuote(ProductPriceQuote entity);

    }
}
