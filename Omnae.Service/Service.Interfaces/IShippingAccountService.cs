using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service.Interfaces
{
    public interface IShippingAccountService
    {
        void Dispose();
        int AddShippingAccount(ShippingAccount entity);
        void UpdateShippingAccount(ShippingAccount entity);
        void RemoveShippingAccount(ShippingAccount entity);
        List<ShippingAccount> FindShippingAccountList();
        ShippingAccount FindShippingAccountById(int Id);
        List<ShippingAccount> FindShippingAccountByCompanyId(int companyId);

        ShippingAccount FindById(int id);
        IQueryable<ShippingAccount> FindAllByCompanyId(int companyId);
        void Delete(ShippingAccount accountInDatabase);

        void SetShippingAccountAsDefault(ShippingAccount entity);
    }
}
