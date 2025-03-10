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
    public class QboTokensRepository : RepositoryBase<QboTokens>, IQboTokensRepository
    {
        public QboTokensRepository(OmnaeContext dbContext) : base(dbContext)
        {

        }

        public int AddQboTokens(QboTokens entity)
        {
            base.Add(entity);
            this.DbContext.Commit();
            return entity.Id;
        }

        public void Dispose()
        {
            this.DbContext.Dispose();
        }

        public QboTokens GetQboTokens(int Id)
        {
            return this.DbContext.QboTokens.Where(x => x.Id == Id).FirstOrDefault();
        }

        public QboTokens GetQboTokens()
        {
            return this.DbContext.QboTokens.FirstOrDefault();
        }

        public List<QboTokens> GetQboTokensList()
        {
            return this.DbContext.QboTokens.ToList();
        }

        public void RemoveQboTokens(QboTokens entity)
        {
            var entry = this.DbContext.Entry(entity);
            if (entry.State == System.Data.Entity.EntityState.Detached)
                this.DbContext.QboTokens.Attach(entity);
            base.Delete(entity);
            this.DbContext.Commit();
        }

        public void UpdateQboTokens(QboTokens entity)
        {
            base.Update(entity);
            this.DbContext.Commit();
        }
    }
}
