using Omnae.Common;
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
    public class ApprovedCapabilityRepository : RepositoryBase<ApprovedCapability>, IApprovedCapabilityRepository
    {
        public ApprovedCapabilityRepository(OmnaeContext dbContext) : base(dbContext)
        {

        }

        public int AddApprovedCapability(ApprovedCapability entity)
        {
            base.Add(entity);
            this.DbContext.Commit();
            return entity.Id;
        }

        public void Dispose()
        {
            this.DbContext.Dispose();
        }

        public List<ApprovedCapability> GetApprovedCapabilities()
        {
            var entities =  this.DbContext.ApprovedCapabilities
                .AsNoTracking()
                .Include("Company")
                .ToList();

            return entities;
        }

        public List<ApprovedCapability> GetApprovedCapabilitiesByParams(BUILD_TYPE buildType, MATERIALS_TYPE materialType)
        {
            var entities = this.DbContext.ApprovedCapabilities
               .AsNoTracking()
               .Include("Company")
               .Where(x => x.BuildType == buildType && x.MaterialType == materialType)
               .ToList();

            return entities;
        }

        public List<ApprovedCapability> GetApprovedCapabilitiesByParams(BUILD_TYPE buildType, MATERIALS_TYPE materialType, int process)
        {
            var entities = this.DbContext.ApprovedCapabilities
              .AsNoTracking()
              .Include("Company")
              .Where(x => x.BuildType == buildType && x.MaterialType == materialType || buildType == BUILD_TYPE.Process)
              .Where(x => x.MaterialType == MATERIALS_TYPE.PrecisionMetals ? (int)x.MetalProcess == process : true)
              .Where(x => x.MaterialType == MATERIALS_TYPE.PrecisionPlastics ? (int)x.PlasticsProcess == process : true)
              .Where(x => x.BuildType == BUILD_TYPE.Process ? (int)x.ProcessType == process : true)
              .ToList();

            return entities;
        }

        public List<ApprovedCapability> GetApprovedCapabilitiesByParams(BUILD_TYPE buildType, Process_Type processType)
        {
            var entities = this.DbContext.ApprovedCapabilities
              .AsNoTracking()
              .Include("Company")
              .Where(x => x.BuildType == buildType && x.ProcessType == processType)
              .ToList();

            return entities;
        }

        public ApprovedCapability GetApprovedCapabilityById(int Id)
        {
            var entity = this.DbContext.ApprovedCapabilities
               .AsNoTracking()
               .Include("Company")
               .Where(x => x.Id == Id).FirstOrDefault();

            return entity;
        }

        public List<ApprovedCapability> GetApprovedCapabilityByVendorId(int vendorId)
        {
            var entities = this.DbContext.ApprovedCapabilities
               .AsNoTracking()
               .Include("Company")
               .Where(x => x.VendorId == vendorId)
               .ToList();

            return entities;
        }

        public void RemoveApprovedCapability(ApprovedCapability entity)
        {
            var entry = this.DbContext.Entry(entity);
            if (entry.State == System.Data.Entity.EntityState.Detached)
            {
                DbContext.ApprovedCapabilities.Attach(entity);
            }

            base.Delete(entity);
            this.DbContext.Commit();
        }

        public void UpdateApprovedCapability(ApprovedCapability entity)
        {
            base.Update(entity);
            this.DbContext.Commit();
        }
    }
}
