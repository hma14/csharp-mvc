using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omnae.Model.ViewModels;

namespace Omnae.Data.StoredProcedures
{
    public interface IStoredProcedure
    {
        void Dispose();

        IEnumerable<TaskOrder> ExecSpGetCustomerOrders<TaskOrder>(string query, params object[] parameters);

        IEnumerable<ProductListViewModel> ExecSpGetUserProducts<ProductListViewModel>(string query, params object[] parameters);
    }
}
