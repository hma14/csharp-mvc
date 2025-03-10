using Omnae.Data.Abstracts;
using Omnae.Data.Repositories.Interfaces;
using Omnae.Model.Models;
using System.Collections.Generic;
using System.Linq;

namespace Omnae.Data.Repositories
{
    public class ShippingAccountRepository : RepositoryBase<ShippingAccount>, IShippingAccountRepository
    {
        public ShippingAccountRepository(OmnaeContext dbContext) : base(dbContext)
        {
        }

        public int AddShippingAccount(ShippingAccount entity)
        {
            base.Add(entity);
            this.DbContext.Commit();
            return entity.Id;
        }

        public void DeleteShippingAccount(ShippingAccount entity)
        {
            DbContext.ShippingAccounts.Attach(entity);
            base.Delete(entity);
            this.DbContext.Commit();
        }

        public void Dispose()
        {
            this.DbContext.Dispose();
        }

        public List<ShippingAccount> GetShippingAccountByCompanyId(int companyId)
        {
            var shippingAccount = this.DbContext.ShippingAccounts
                                                .AsNoTracking()
                                                .Where(x => x.CompanyId == companyId)
                                                .ToList();

            return shippingAccount;

        }

        public ShippingAccount FindById(int id)
        {
            var shippingAccount = this.DbContext.ShippingAccounts.Find(id);
            return shippingAccount;
        }

        public IQueryable<ShippingAccount> FindAllByCompanyId(int companyId)
        {
            var shippingAccount = this.DbContext.ShippingAccounts
                                                .AsNoTracking()
                                                .Where(x => x.CompanyId == companyId);

            return shippingAccount;
        }

        public void SetShippingAccountAsDefault(ShippingAccount entity)
        {
            DbContext.Database.ExecuteSqlCommand(@"UPDATE ShippingAccounts SET IsDefault = 0 WHERE CompanyId = @p0", entity.CompanyId);
            DbContext.Database.ExecuteSqlCommand(@"UPDATE ShippingAccounts SET IsDefault = 1 WHERE CompanyId = @p0 and Id = @p1", entity.CompanyId, entity.Id);
        }

        public ShippingAccount GetShippingAccountById(int Id)
        {
            var shippingAccount = this.DbContext.ShippingAccounts
                                                .AsNoTracking()
                                                .Where(x => x.Id == Id)
                                                .FirstOrDefault();

            return shippingAccount;
        }

        public List<ShippingAccount> GetShippingAccountList()
        {
            var shippingAccounts = this.DbContext.ShippingAccounts
                                                .AsNoTracking()
                                                .ToList();

            return shippingAccounts;
        }

        public void UpdateShippingAccount(ShippingAccount entity)
        {
            base.Update(entity);
            this.DbContext.Commit();
        }

        public void AdjustTheDefaultShippingAccount(int companyId, ShippingAccount shippingAccount)
        {
            //This will set the old active or the 1st ShippingAccounts as the DefaultShippingProfile

            var sql = @"UPDATE t
	                        SET IsDefault=1
                        FROM (
	                        SELECT TOP 1 [a].*
	                        FROM [ShippingAccounts] AS [a]
	                        WHERE ([a].[IsActive] = 1) AND ([a].[CompanyId] = @p0)
	                        ORDER BY [a].[IsDefault] DESC, [a].[_createdAt]
	                        ) t";

            DbContext.Database.ExecuteSqlCommand(sql, companyId);
            DbContext.Entry(shippingAccount).Reload();
        }
    }
}
