using Omnae.Model.Models;
using Omnae.Data.Abstracts;
using Omnae.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Data.Repositories
{
    public class PriceBreakRepository : RepositoryBase<PriceBreak>, IPriceBreakRepository
    {
        public PriceBreakRepository(OmnaeContext dbContext) : base(dbContext)
        {

        }
        public int AddPriceBreak(PriceBreak entity)
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

        public int RemovePriceBreak(PriceBreak entity)
        {
            base.Delete(entity);
            this.DbContext.Commit();
            return entity.ProductId;
        }

        public void Dispose()
        {
            this.DbContext.Dispose();
        }

        public PriceBreak GetPriceBreakById(int Id)
        {
            return this.DbContext.PriceBreaks.Where(x => x.Id == Id).FirstOrDefault();
        }

        public List<PriceBreak> GetPriceBreakByProductId(int productId)
        {
            var res = this.DbContext.PriceBreaks
                                    .Include("RFQBid")
                                    .Where(x => x.ProductId == productId && x.RFQBid.IsActive == true && x.UnitPrice != null)
                                    .OrderBy(x => x.Quantity).ToList();

            if (res.Count == 0)
            {
                res = this.DbContext.PriceBreaks
                                    .Where(x => x.ProductId == productId && x.UnitPrice != null)
                                    .OrderBy(x => x.Quantity).ToList();
            }
            if (res.Count == 0)
            {
                res = this.DbContext.PriceBreaks
                                    .Where(x => x.ProductId == productId)
                                    .OrderBy(x => x.Quantity).ToList();
            }
            return res;
        }

        public IQueryable<PriceBreak> GetPriceBreaksByProductId(int productId)
        {
            var res = this.DbContext.PriceBreaks
                                    .Include("RFQBid")
                                    .Where(x => x.ProductId == productId && x.RFQBid.IsActive == true && x.UnitPrice != null)
                                    .OrderBy(x => x.Quantity);

            if (res.Count() == 0)
            {
                res = this.DbContext.PriceBreaks
                                    .Where(x => x.ProductId == productId && x.UnitPrice != null)
                                    .OrderBy(x => x.Quantity);
            }
            if (res.Count() == 0)
            {
                res = this.DbContext.PriceBreaks
                                    .Where(x => x.ProductId == productId)
                                    .OrderBy(x => x.Quantity);
            }
            return res;
        }


        public List<PriceBreak> GetPriceBreakList()
        {
            return this.DbContext.PriceBreaks.ToList();
        }

        public int UpdatePriceBreak(PriceBreak entity)
        {
            base.Update(entity);
            this.DbContext.Commit();
            return entity.Id;
        }

        public List<PriceBreak> GetPriceBreakByProductIdRFQBidId(int productId, int rfqbidId)
        {
            return this.DbContext.PriceBreaks.Where(x => x.ProductId == productId && x.RFQBidId == rfqbidId).OrderBy(x => x.Quantity).ToList();
        }
        public List<PriceBreak> GetPriceBreakByProductIdRFQBidIdQty(int productId, int rfqbidId, decimal qty)
        {
            return this.DbContext.PriceBreaks.Where(x => x.ProductId == productId &&
                                                         x.RFQBidId == rfqbidId &&
                                                         x.UnitPrice > 0 &&
                                                         x.Quantity <= qty)
                                             .OrderBy(x => x.Quantity).ToList();
        }

        public List<PriceBreak> GetPriceBreakByProductIdQty(int productId, decimal qty)
        {
            return DbContext.PriceBreaks
                .Where(x => x.ProductId == productId && x.Quantity <= qty)
                .OrderBy(x => x.Quantity)
                .ToList();
        } 
        public List<PriceBreak> GetPriceBreakByTaskIdProductId(int taskId, int productId)
        {
            return DbContext.PriceBreaks.Where(x => x.TaskId == taskId && x.ProductId == productId).ToList();
        }



        public PriceBreak GetPriceBreakByProductIdQtyUnitPrice(int productId, decimal qty, decimal unitPrice)
        {
            return this.DbContext.PriceBreaks.Where(x => x.ProductId == productId && x.Quantity == qty && x.UnitPrice == unitPrice).FirstOrDefault();
        }

        public List<PriceBreak> GetPriceBreakByTaskIdProductIdQty(int taskId, int productId, decimal qty)
        {
            return DbContext.PriceBreaks.Where(x => x.TaskId == taskId && x.ProductId == productId && x.Quantity <= qty && x.VendorUnitPrice > 0).OrderBy(x => x.Quantity).ToList();
        }
        public List<PriceBreak> GetPriceBreakByTaskIdProductIdExactQty(int taskId, int productId, decimal qty)
        {
            return DbContext.PriceBreaks.Where(x => x.TaskId == taskId && x.ProductId == productId && x.Quantity == qty).ToList();
        }


        public void PriceBreakAddRange(List<PriceBreak> entities)
        {
            DbContext.PriceBreaks.AddRange(entities);
        }

        public List<PriceBreak> GetPriceBreakByTaskId(int taskId)
        {
            return DbContext.PriceBreaks.Where(x => x.TaskId == taskId).ToList();
        }
        public IQueryable<PriceBreak> GetPriceBreaksByTaskId(int taskId)
        {
            return DbContext.PriceBreaks.Where(x => x.TaskId == taskId && x.UnitPrice >= 0);
        }
        public IQueryable<PriceBreak> GetAllPriceBreaksByTaskId(int taskId)
        {
            return DbContext.PriceBreaks.Where(x => x.TaskId == taskId);
        }

    }
}
