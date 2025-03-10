using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service.Interfaces
{
    public interface IAddressService
    {
        void Dispose();

        int AddAddress(Address entity);
        Address FindAddressById(int id);
        Address Update(Address entity);
        void UpdateAddressesForTheCompany(int? addressId, int companyId, AddressType addressType);
        IQueryable<Address> FindAllFromCompanies(int id);

        void FixIds(Address addr);
    }
}
