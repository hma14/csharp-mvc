using Omnae.Data.Abstracts;
using Omnae.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omnae.Model.Models;

namespace Omnae.Data.Repositories
{
    public class ShippingProfileRepository : RepositoryBase<ShippingProfile>, IShippingProfileRepository
    {
        public ShippingProfileRepository(OmnaeContext dbContext) : base(dbContext)
        {
        }

        public void Add(ShippingProfile entity)
        {
            base.Add(entity);

            DbContext.SaveChanges();
        }

        public IQueryable<ShippingProfile> ListAllByCompanyId(int companyId)
        {
            return DbContext.ShippingProfiles.Where(s => s.CompanyId == companyId);
        }

        public void SetShippingProfileAsDefault(ShippingProfile entity)
        {
            DbContext.Database.ExecuteSqlCommand(@"UPDATE ShippingProfiles SET IsDefault = 0 WHERE CompanyId = @p0", entity.CompanyId);
            DbContext.Database.ExecuteSqlCommand(@"UPDATE ShippingProfiles SET IsDefault = 1 WHERE CompanyId = @p0 and Id = @p1", entity.CompanyId, entity.Id);
        }

        public void AdjustTheDefaultShippingProfile(int companyId, ShippingProfile shippingProfile)
        {
            //This will set the old active or the 1st ShippingProfile as the DefaultShippingProfile

            var sql = @"UPDATE t
	                        SET IsDefault=1
                        FROM (
	                        SELECT TOP 1 [p].*
	                        FROM [ShippingProfiles] AS [p]
	                        WHERE ([p].[IsActive] = 1) AND ([p].[CompanyId] = @p0)
	                        ORDER BY [p].[IsDefault] DESC, [p].[_createdAt]
	                        ) t";

            DbContext.Database.ExecuteSqlCommand(sql, companyId);
            DbContext.Entry(shippingProfile).Reload();
        }
    }
}
