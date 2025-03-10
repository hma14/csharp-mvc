using Omnae.Service.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Omnae.Model.Models;
using Omnae.Data.Repositories.Interfaces;
using Omnae.Data.Abstracts;

namespace Omnae.Service.Service
{
    public class PriceBreakService : IPriceBreakService
    {
        private readonly IPriceBreakRepository priceBreakRepository;
        private readonly IUnitOfWork unitOfWork;

        public PriceBreakService(IPriceBreakRepository priceBreakRepository, IUnitOfWork unitOfWork)
        {
            this.priceBreakRepository = priceBreakRepository;
            this.unitOfWork = unitOfWork;
        }

        public int AddPriceBreak(PriceBreak entity)
        {
            try
            {
                return priceBreakRepository.AddPriceBreak(entity);
                //var ent = priceBreakRepository.GetPriceBreakByProductId(entity.ProductId).Where(x => x.Quantity == entity.Quantity && x.RFQBidId == entity.RFQBidId).FirstOrDefault();
                //if (ent == null)
                //{
                //    return priceBreakRepository.AddPriceBreak(entity);
                //}
                //else
                //{
                //    return priceBreakRepository.UpdatePriceBreak(entity);
                //}

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void PriceBreakAddRange(List<PriceBreak> entities)
        {
            try
            {
                priceBreakRepository.PriceBreakAddRange(entities);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int DeletePriceBreak(PriceBreak entity)
        {
            return priceBreakRepository.RemovePriceBreak(entity);
        }

        public void Dispose()
        {
            priceBreakRepository.Dispose();
        }

        public decimal? FindMinimumOrderQuantity(int productId)
        {
            List<PriceBreak> list = FindPriceBreakByProductId(productId);
            if (list.Count > 0)
            {
                return list.FirstOrDefault().Quantity;
            }
            else
            {
                return null;
            }
        }

        public decimal? FindLowestUnitPrice(int productId)
        {
            return FindPriceBreakByProductId(productId).LastOrDefault().UnitPrice;
        }

        public int? FindLowestUnitPricePricePriceBreakId(int productId, int? rfqBidId = null)
        {
            List<PriceBreak> list;
            if (rfqBidId != null)
            {
                list = FindPriceBreakByProductId(productId).Where(x => x.RFQBidId == rfqBidId).ToList();
            }
            else
            {
                list = FindPriceBreakByProductId(productId);
            }

            if (list.Count > 0)
            {
                return list.LastOrDefault().Id;
            }
            else
            {
                return null;
            }
        }
        public int? FindNextLowestUnitPricePriceBreakId(int productId)
        {
            List<PriceBreak> list = FindPriceBreakByProductId(productId);
            if (list.Count > 2)
            {
                return list.Take(list.Count - 2).LastOrDefault().Id;
            }
            else if (list.Count == 2)
            {
                return list.First().Id;
            }
            else
            {
                return null;
            }
        }

        public PriceBreak FindPriceBreakById(int Id)
        {
            return priceBreakRepository.GetPriceBreakById(Id);
        }

        public List<PriceBreak> FindPriceBreakByProductId(int productId)
        {
            return priceBreakRepository.GetPriceBreakByProductId(productId);
        }
        public IQueryable<PriceBreak> FindPriceBreaksByProductId(int productId)
        {
            return priceBreakRepository.GetPriceBreaksByProductId(productId);
        }
        public List<PriceBreak> FindPriceBreakByProductIdRFQBidId(int productId, int rfqbidId)
        {
            return priceBreakRepository.GetPriceBreakByProductIdRFQBidId(productId, rfqbidId);
        }
        public List<PriceBreak> FindPriceBreakByProductIdRFQBidIdQty(int productId, int rfqbidId, decimal qty)
        {
            return priceBreakRepository.GetPriceBreakByProductIdRFQBidIdQty(productId, rfqbidId, qty);
        }

        public PriceBreak FindPriceBreakByProductIdQty(int productId, decimal qty)
        {
            var pb = priceBreakRepository.GetPriceBreakByProductIdQty(productId, qty)
                                       .Where(x => x.RFQBidId != null && x.RFQBidId > 0 && x.RFQBid.IsActive == true)
                                       .Where(x => x.UnitPrice != null && x.UnitPrice > 0).LastOrDefault();

            if (pb == null)
            {
                pb = priceBreakRepository.GetPriceBreakByProductIdQty(productId, qty)
                                       .Where(x => x.RFQBidId == null)
                                       .Where(x => x.UnitPrice != null && x.UnitPrice > 0).LastOrDefault();
            }
            return pb;
        }

        public List<PriceBreak> FindPriceBreakByTaskIdProductId(int taskId, int productId)
        {
            return priceBreakRepository.GetPriceBreakByTaskIdProductId(taskId, productId);
        }

        public List<PriceBreak> FindPriceBreakByTaskId(int taskId)
        {
            return priceBreakRepository.GetPriceBreakByTaskId(taskId);
        }

        public IQueryable<PriceBreak> FindPriceBreaksByTaskId(int taskId)
        {
            return priceBreakRepository.GetPriceBreaksByTaskId(taskId);
        }

        public IQueryable<PriceBreak> FindAllPriceBreaksByTaskId(int taskId)
        {
            return priceBreakRepository.GetAllPriceBreaksByTaskId(taskId);
        }

        public PriceBreak FindPriceBreakByProductIdQtyUnitPrice(int productId, decimal qty, decimal unitPrice)
        {
            return priceBreakRepository.GetPriceBreakByProductIdQtyUnitPrice(productId, qty, unitPrice);
        }
        public PriceBreak FindClosestPriceBreakByProductIdRFQBidIdQty(int productId, decimal qty, int? rfqBidId = null)
        {
            PriceBreak pb = null;
            if (rfqBidId != null)
            {
                pb = priceBreakRepository.GetPriceBreakByProductIdRFQBidIdQty(productId, rfqBidId.Value, qty).LastOrDefault();
                return pb;
            }

            pb = priceBreakRepository.GetPriceBreakByProductId(productId)
                .OrderBy(x => x.Quantity)
                .Where(q => q.Quantity >= qty).FirstOrDefault();
            return pb;

        }

        public List<PriceBreak> FindPriceBreakList()
        {
            return priceBreakRepository.GetPriceBreakList().OrderBy(x => x.Quantity).ToList();
        }

        public decimal? FindUnitPriceGivenQuantity(int productId, int quantity)
        {
            var pbreaks = FindPriceBreakByProductId(productId);
            if (pbreaks == null)
                return 0;
            var list1 = pbreaks.Where(a => a.Quantity >= quantity).OrderBy(x => x.Quantity);
            if (list1 == null || list1.Count() == 0)
            {
                var list2 = pbreaks.Where(a => a.Quantity > quantity).OrderBy(x => x.Quantity);
                return list2.FirstOrDefault().UnitPrice;
            }
            return list1.LastOrDefault()?.UnitPrice;
        }

        public PriceBreak FindPriceBreakByTaskIdProductIdQty(int taskId, int productId, decimal qty)
        {
            return priceBreakRepository.GetPriceBreakByTaskIdProductIdQty(taskId, productId, qty).LastOrDefault();
        }
        public PriceBreak FindPriceBreakByTaskIdProductIdExactQty(int taskId, int productId, decimal qty)
        {
            return priceBreakRepository.GetPriceBreakByTaskIdProductIdExactQty(taskId, productId, qty).FirstOrDefault();
        }

        //public decimal? FindUnitPriceProductIdRFQBidIdQty(int productId, int rfqbidId, int quantity)
        //{
        //    //var pbreaks = FindPriceBreakByProductIdRFQBidId(productId, rfqbidId);
        //    var pbreaks = FindPriceBreakByProductIdQty(productId, quantity);

        //    if (pbreaks == null)
        //        return 0;
        //    var list1 = pbreaks.Where(a => a.Quantity <= quantity).OrderBy(x => x.Quantity);
        //    if (list1 == null || list1.Count() == 0)
        //    {
        //        var list2 = pbreaks.Where(a => a.Quantity > quantity).OrderBy(x => x.Quantity);
        //        return list2.FirstOrDefault().UnitPrice;
        //    }
        //    return list1.LastOrDefault().UnitPrice;
        //}

        public int ModifyPriceBreak(PriceBreak entity)
        {
            return priceBreakRepository.UpdatePriceBreak(entity);
        }


    }
}
