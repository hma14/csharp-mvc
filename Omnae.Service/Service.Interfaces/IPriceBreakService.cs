using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service.Interfaces
{
    public interface IPriceBreakService
    {
        void Dispose();
        int AddPriceBreak(PriceBreak entity);
        void PriceBreakAddRange(List<PriceBreak> entities);
        PriceBreak FindPriceBreakById(int Id);
        List<PriceBreak> FindPriceBreakByProductId(int productId);
        List<PriceBreak> FindPriceBreakList();
        List<PriceBreak> FindPriceBreakByProductIdRFQBidId(int productId, int rfqbidId);
        List<PriceBreak> FindPriceBreakByProductIdRFQBidIdQty(int productId, int rfqbidId, decimal qty);
        PriceBreak FindPriceBreakByProductIdQty(int productId, decimal qty);
        List<PriceBreak> FindPriceBreakByTaskIdProductId(int taskId, int productId);
        PriceBreak FindPriceBreakByTaskIdProductIdQty(int taskId, int productId, decimal qty);
        PriceBreak FindPriceBreakByTaskIdProductIdExactQty(int taskId, int productId, decimal qty);
        PriceBreak FindPriceBreakByProductIdQtyUnitPrice(int productId, decimal qty, decimal unitPrice);
        decimal? FindUnitPriceGivenQuantity(int productId, int quantity);
        

        int DeletePriceBreak(PriceBreak entity);
        int ModifyPriceBreak(PriceBreak entity);
        decimal? FindLowestUnitPrice(int productId);
        int? FindLowestUnitPricePricePriceBreakId(int productId, int? rfqBidId = null);
        int? FindNextLowestUnitPricePriceBreakId(int productId);
        decimal? FindMinimumOrderQuantity(int productId);
        //decimal? FindUnitPriceProductIdRFQBidIdQty(int productId, int rfqbidId, int quantity);
        PriceBreak FindClosestPriceBreakByProductIdRFQBidIdQty(int productId, decimal qty, int? rfqBidId);

        List<PriceBreak> FindPriceBreakByTaskId(int taskId);
        IQueryable<PriceBreak> FindPriceBreaksByTaskId(int taskId);
        IQueryable<PriceBreak> FindPriceBreaksByProductId(int productId);
        IQueryable<PriceBreak> FindAllPriceBreaksByTaskId(int taskId);
    }
}
