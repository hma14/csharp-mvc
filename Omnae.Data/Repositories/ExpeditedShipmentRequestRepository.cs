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
    public class ExpeditedShipmentRequestRepository : RepositoryBase<ExpeditedShipmentRequest>, IExpeditedShipmentRequestRepository
    {
        public ExpeditedShipmentRequestRepository(OmnaeContext dbContext) : base (dbContext)
        {

        }
        public int AddExpeditedShipmentRequest(ExpeditedShipmentRequest entity)
        {
            base.Add(entity);
            this.DbContext.Commit();
            return entity.Id;
        }

        public void Dispose()
        {
            this.DbContext.Dispose();
        }

        public ExpeditedShipmentRequest GetExpeditedShipmentRequest(int id)
        {
            return this.DbContext.ExpeditedShipmentRequests
                        .Where(x => x.Id == id)
                        .FirstOrDefault();
        }

        public ExpeditedShipmentRequest GetExpeditedShipmentRequestByParams(int OrderId, int InitiateCompanyId)
        {
            return this.DbContext.ExpeditedShipmentRequests
                        .Where(x => x.OrderId == OrderId &&
                                    x.InitiateCompanyId == InitiateCompanyId)
                        .FirstOrDefault();
        }
        public ExpeditedShipmentRequest GetExpeditedShipmentRequestByParams(int OrderId, int InitiateCompanyId, DateTime NewDesireShippingDate)
        {
            return this.DbContext.ExpeditedShipmentRequests
                        .Where(x => x.OrderId == OrderId &&
                                    x.InitiateCompanyId == InitiateCompanyId &&
                                    x.NewDesireShippingDate == NewDesireShippingDate)
                        .FirstOrDefault();
        }

        public IQueryable<ExpeditedShipmentRequest> GetExpeditedShipmentRequestsByCompanyId(int id)
        {
            return this.DbContext.ExpeditedShipmentRequests
                        .Where(x => x.InitiateCompanyId == id);
        }

        public void UpdateExpeditedShipmentRequest(ExpeditedShipmentRequest entity)
        {
            base.Update(entity);
            this.DbContext.Commit();
        }
    }
}
