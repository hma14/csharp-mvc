using Omnae.Data.Abstracts;
using Omnae.Data.StoredProcedures;
using Omnae.Model.Models;
using Omnae.Service.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omnae.Model.ViewModels;

namespace Omnae.Service.Service
{
    public class StoredProcedureService : IStoredProcedureService
    {
        private readonly IStoredProcedure storedProcedure;
        private readonly IUnitOfWork unitOfWork;

        public StoredProcedureService(IStoredProcedure storedProcedure, IUnitOfWork unitOfWork)
        {
            this.storedProcedure = storedProcedure;
            this.unitOfWork = unitOfWork;
        }

        public void Dispose()
        {
            storedProcedure.Dispose();
        }

        List<TaskOrder> IStoredProcedureService.SpGetCustomerOrders(string query, params object[] parameters)
        {
            return storedProcedure.ExecSpGetCustomerOrders<TaskOrder>(query, parameters).ToList();
        }

        public List<ProductListViewModel> SpGetUserProducts(string query, params object[] parameters)
        {
            return storedProcedure.ExecSpGetUserProducts<ProductListViewModel>(query, parameters).ToList();
        }
    }
}
