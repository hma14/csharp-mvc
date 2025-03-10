using Omnae.Data.Abstracts;
using Omnae.Data.Repositories.Interfaces;
using Omnae.Model.Models;
using Omnae.Service.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service
{
    public class ExpeditedShipmentRequestService : IExpeditedShipmentRequestService
    {
        private readonly IExpeditedShipmentRequestRepository repository;
        private readonly IUnitOfWork unitOfWork;

        public ExpeditedShipmentRequestService(IExpeditedShipmentRequestRepository repository, IUnitOfWork unitOfWork)
        {
            this.repository = repository;
            this.unitOfWork = unitOfWork;
        }

        public int AddExpeditedShipmentRequest(ExpeditedShipmentRequest entity)
        {
            return repository.AddExpeditedShipmentRequest(entity);
        }

        public void Dispose()
        {
           repository.Dispose();
        }

        public ExpeditedShipmentRequest FindExpeditedShipmentRequest(int id)
        {
            return repository.GetExpeditedShipmentRequest(id);
        }

        public ExpeditedShipmentRequest FindExpeditedShipmentRequestByParams(int OrderId, int InitiateCompanyId)
        {
            return repository.GetExpeditedShipmentRequestByParams(OrderId, InitiateCompanyId);
        }
        public ExpeditedShipmentRequest FindExpeditedShipmentRequestByParams(int OrderId, int InitiateCompanyId, DateTime NewDesireShippingDate)
        {
            return repository.GetExpeditedShipmentRequestByParams(OrderId, InitiateCompanyId, NewDesireShippingDate);
        }

        public IQueryable<ExpeditedShipmentRequest> FindExpeditedShipmentRequestsByCompanyId(int id)
        {
            return repository.GetExpeditedShipmentRequestsByCompanyId(id);
        }

        public void UpdateExpeditedShipmentRequest(ExpeditedShipmentRequest entity)
        {
            repository.UpdateExpeditedShipmentRequest(entity);
        }

    }
}
