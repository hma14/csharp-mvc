using Omnae.Model.Models;
using Omnae.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service.Interfaces
{
    public interface IStoredProcedureService
    {
        void Dispose();

        List<TaskOrder> SpGetCustomerOrders(string query, params object[] parameters);
        List<ProductListViewModel> SpGetUserProducts(string query, params object[] parameters);
    }
}
