using Omnae.Model.Models;
using Omnae.Data.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Data.Repositories.Interfaces
{
    public interface IPriceBreakRepository : IRepository<PriceBreak>
    {
        void Dispose();
        int AddPriceBreak(PriceBreak entity);
        int RemovePriceBreak(PriceBreak entity);
        int UpdatePriceBreak(PriceBreak entity);
        void PriceBreakAddRange(List<PriceBreak> entities);
        PriceBreak GetPriceBreakById(int Id);
        List<PriceBreak> GetPriceBreakByProductId(int productId);
        List<PriceBreak> GetPriceBreakList();
        List<PriceBreak> GetPriceBreakByProductIdRFQBidId(int productId, int rfqbidId);
        List<PriceBreak> GetPriceBreakByProductIdRFQBidIdQty(int productId, int rfqbidId, decimal qty);
        List<PriceBreak> GetPriceBreakByProductIdQty(int productId, decimal qty);
        List<PriceBreak> GetPriceBreakByTaskIdProductId(int taskId, int productId);
        PriceBreak GetPriceBreakByProductIdQtyUnitPrice(int productId, decimal qty, decimal unitPrice);
        List<PriceBreak> GetPriceBreakByTaskIdProductIdQty(int taskId, int productId, decimal qty);
        List<PriceBreak> GetPriceBreakByTaskIdProductIdExactQty(int taskId, int productId, decimal qty);

        List<PriceBreak> GetPriceBreakByTaskId(int taskId);
        IQueryable<PriceBreak> GetPriceBreaksByTaskId(int taskId);
        IQueryable<PriceBreak> GetPriceBreaksByProductId(int productId);
        IQueryable<PriceBreak> GetAllPriceBreaksByTaskId(int taskId);
    }
}
