using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service.Interfaces
{
    public interface IProductPriceQuoteService
    {
        void Dispose();
        int AddProductPriceQuote(ProductPriceQuote entity);
        IList<ProductPriceQuote> FindProductPriceQuotesById(int id);
        IQueryable<ProductPriceQuote> FindProductPriceQuotesByProductId(int productId);
        IQueryable<ProductPriceQuote> FindProductPriceQuotes(int productId, int vendorId);
        IQueryable<ProductPriceQuote> FindProductPriceQuotesByVendorId(int vendorId);
        void UpdateProductPriceQuote(ProductPriceQuote entity);
    }
}
