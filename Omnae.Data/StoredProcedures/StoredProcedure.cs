using Omnae.Data.Abstracts;
using Omnae.Model.Models;
using System.Collections.Generic;
using System;
using Omnae.Model.ViewModels;

namespace Omnae.Data.StoredProcedures
{
    public class StoredProcedure : RepositoryBase<TaskOrder>, IStoredProcedure
    {
        public StoredProcedure(OmnaeContext dbContext) : base(dbContext)
        {

        }

        public void Dispose()
        {
            this.DbContext.Dispose();
        }

        //public   IEnumerable<TaskOrder> ExecSpGetCustomerOrders(string query, params object[] parameters)
        //{
        //    // return this.DbContext.Database.SqlQuery<TaskOrder>(query, parameters);
        //    return ExecStoredProcedure<TaskOrder>(query, parameters);
        //}

        public IEnumerable<TaskOrder> ExecSpGetCustomerOrders<TaskOrder>(string query, params object[] parameters)
        {
            return ExecStoredProcedure<TaskOrder>(query, parameters);
        }

        //public IEnumerable<ProductListViewModel> ExecSpGetUserProducts(string query, params object[] parameters)
        //{
        //    return ExecStoredProcedure<ProductListViewModel>(query, parameters);
        //}

        public IEnumerable<ProductListViewModel> ExecSpGetUserProducts<ProductListViewModel>(string query, params object[] parameters)
        {
            return ExecStoredProcedure<ProductListViewModel>(query, parameters);
        }
    }
}
