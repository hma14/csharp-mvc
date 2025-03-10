using Omnae.Data.Abstracts;
using Omnae.Data.Repositories.Interfaces;
using Omnae.Model.Models;
using Omnae.Service.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Omnae.Service.Service
{
    public class ShippingAccountService : IShippingAccountService
    {
        private readonly IShippingAccountRepository shippingAccountRepository;
        private readonly IUnitOfWork unitOfWork;

        public ShippingAccountService(IShippingAccountRepository shippingAccountRepository, IUnitOfWork unitOfWork)
        {
            this.shippingAccountRepository = shippingAccountRepository;
            this.unitOfWork = unitOfWork;
        }

        public int AddShippingAccount(ShippingAccount entity)
        {
            var id = shippingAccountRepository.AddShippingAccount(entity);

            if (entity.IsDefault)
            {
                SetShippingAccountAsDefault(entity);
            }
            else
            {
                shippingAccountRepository.AdjustTheDefaultShippingAccount(entity.CompanyId, entity);
            }

            return id;
        }

        public void Dispose()
        {
            shippingAccountRepository.Dispose();
        }

        public List<ShippingAccount> FindShippingAccountByCompanyId(int companyId)
        {
            return shippingAccountRepository.GetShippingAccountByCompanyId(companyId);
        }

        public ShippingAccount FindById(int id)
        {
            return shippingAccountRepository.FindById(id);
        }

        public IQueryable<ShippingAccount> FindAllByCompanyId(int companyId)
        {
            return shippingAccountRepository.FindAllByCompanyId(companyId)
                                            .Where(p => p.IsActive);
        }

        public void Delete(ShippingAccount accountInDatabase)
        {
            accountInDatabase.IsActive = false;

            shippingAccountRepository.UpdateShippingAccount(accountInDatabase);

            if (accountInDatabase.IsDefault)
            {
                shippingAccountRepository.AdjustTheDefaultShippingAccount(accountInDatabase.CompanyId, accountInDatabase);
            }
        }

        public ShippingAccount FindShippingAccountById(int Id)
        {
            return shippingAccountRepository.GetShippingAccountById(Id);
        }

        public List<ShippingAccount> FindShippingAccountList()
        {
            return shippingAccountRepository.GetShippingAccountList();
        }

        public void RemoveShippingAccount(ShippingAccount entity)
        {
            shippingAccountRepository.DeleteShippingAccount(entity);
        }

        public void UpdateShippingAccount(ShippingAccount entity)
        {
            shippingAccountRepository.UpdateShippingAccount(entity);

            if (entity.IsDefault)
            {
                SetShippingAccountAsDefault(entity);
            }
            else 
            {
                shippingAccountRepository.AdjustTheDefaultShippingAccount(entity.CompanyId, entity);
            }
        }

        public void SetShippingAccountAsDefault(ShippingAccount entity)
        {
            entity.IsDefault = true;
            shippingAccountRepository.SetShippingAccountAsDefault(entity);
        }
    }
}
