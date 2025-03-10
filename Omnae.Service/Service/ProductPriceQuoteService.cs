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
    public class ProductPriceQuoteService : IProductPriceQuoteService
    {
        private readonly IProductPriceQuoteRepository repository;
        private readonly IUnitOfWork unitOfWork;

        public ProductPriceQuoteService(IProductPriceQuoteRepository repository, IUnitOfWork unitOfWork)
        {
            this.repository = repository;
            this.unitOfWork = unitOfWork;
        }

        public int AddProductPriceQuote(ProductPriceQuote entity)
        {
            return repository.AddProductPriceQuote(entity);
        }

        public void Dispose()
        {
            repository.Dispose();
        }

        public IQueryable<ProductPriceQuote> FindProductPriceQuotes(int productId, int vendorId)
        {
            return repository.GetProductPriceQuotes(productId, vendorId);
        }

        public IList<ProductPriceQuote> FindProductPriceQuotesById(int id)
        {
            return repository.GetProductPriceQuotesById(id).ToList();
        }

        public IQueryable<ProductPriceQuote> FindProductPriceQuotesByProductId(int productId)
        {
            return repository.GetProductPriceQuotesByProductId(productId);
        }
        public IQueryable<ProductPriceQuote> FindProductPriceQuotesByVendorId(int vendorId)
        {
            return repository.GetProductPriceQuotesByVendorId(vendorId);
        }

        public void UpdateProductPriceQuote(ProductPriceQuote entity)
        {
            repository.UpdateProductPriceQuote(entity);
        }
    }
}
