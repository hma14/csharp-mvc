using Omnae.Model.Models;
using Omnae.Data.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Data.Repositories.Interfaces
{
    public interface IAddressRepository : IRepository<Address>
    {

        void Dispose();

        int AddAddress(Address entity);
        Address GetAddressById(int id);
        IQueryable<Address> FindAllFromCompanies(int id);
        void UpdateAddress(Address entity);
    }
}
