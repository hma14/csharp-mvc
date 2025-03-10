using Omnae.Data.Abstracts;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Data.Repositories.Interfaces
{
    public interface IShippingAccountRepository : IRepository<ShippingAccount>
    {
        void Dispose();
        int AddShippingAccount(ShippingAccount entity);
        void UpdateShippingAccount(ShippingAccount entity);
        void DeleteShippingAccount(ShippingAccount entity);
        List<ShippingAccount> GetShippingAccountList();
        ShippingAccount GetShippingAccountById(int Id);
        List<ShippingAccount> GetShippingAccountByCompanyId(int companyId);

        ShippingAccount FindById(int id);
        IQueryable<ShippingAccount> FindAllByCompanyId(int companyId);
        void SetShippingAccountAsDefault(ShippingAccount entity);
        void AdjustTheDefaultShippingAccount(int companyId, ShippingAccount shippingAccount);
    }
}
