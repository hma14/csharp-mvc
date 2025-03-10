using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Data.Repositories.Interfaces
{
    public interface IExpeditedShipmentRequestRepository
    {
        void Dispose();

        int AddExpeditedShipmentRequest(ExpeditedShipmentRequest entity);
        void UpdateExpeditedShipmentRequest(ExpeditedShipmentRequest entity);
        ExpeditedShipmentRequest GetExpeditedShipmentRequest(int id);
        ExpeditedShipmentRequest GetExpeditedShipmentRequestByParams(int OrderId, int InitiateCompanyId);
        ExpeditedShipmentRequest GetExpeditedShipmentRequestByParams(int OrderId, int InitiateCompanyId, DateTime NewDesireShippingDate);
        IQueryable<ExpeditedShipmentRequest> GetExpeditedShipmentRequestsByCompanyId(int id);

    }
}
