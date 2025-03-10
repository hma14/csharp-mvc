using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service.Interfaces
{
    public interface IExpeditedShipmentRequestService
    {
        void Dispose();

        int AddExpeditedShipmentRequest(ExpeditedShipmentRequest entity);
        void UpdateExpeditedShipmentRequest(ExpeditedShipmentRequest entity);
        ExpeditedShipmentRequest FindExpeditedShipmentRequest(int id);
        IQueryable<ExpeditedShipmentRequest> FindExpeditedShipmentRequestsByCompanyId(int id);
        ExpeditedShipmentRequest FindExpeditedShipmentRequestByParams(int OrderId, int InitiateCompanyId);
        ExpeditedShipmentRequest FindExpeditedShipmentRequestByParams(int OrderId, int InitiateCompanyId, DateTime NewDesireShippingDate);
    }
}
