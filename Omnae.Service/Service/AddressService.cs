using Omnae.Model.Models;
using Omnae.Data.Abstracts;
using Omnae.Data.Repositories.Interfaces;
using Omnae.Service.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service
{
    public class AddressService : IAddressService
    {        
        private readonly IStateProvinceRepository stateProvinceRepository;
        private readonly IAddressRepository addressRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ICountryService countryService;

        public AddressService(IAddressRepository addressRepository, IUnitOfWork unitOfWork, ICountryService countryService, IStateProvinceRepository stateProvinceRepository)
        {
            this.addressRepository = addressRepository;
            this.unitOfWork = unitOfWork;
            this.countryService = countryService;
            this.stateProvinceRepository = stateProvinceRepository;
        }

        public void Dispose()
        {
            addressRepository.Dispose();
        }

        public int AddAddress(Address entity)
        {
            return addressRepository.AddAddress(entity);
        }

        public Address FindAddressById(int id)
        {
            return addressRepository.GetAddressById(id);
        }

        public Address Update(Address entity)
        {
            addressRepository.UpdateAddress(entity);
            return entity;
        }

        public void FixIds(Address addr)
        {
            if (addr == null)
                return;

            FixCountryIds(addr);
            FixStateProvinceIds(addr);
        }

        private void FixStateProvinceIds(Address addr)
        {
            if (addr == null || string.IsNullOrWhiteSpace(addr.Country?.CountryName) || string.IsNullOrWhiteSpace(addr.StateProvince?.Name))
                return;

            if (addr.Country.CountryCode != "CA" && addr.Country.CountryCode != "US")
                return;

            var stateProvince = stateProvinceRepository.FindStateProvinceByName(addr.StateProvince.Name);
            if (stateProvince == null)
                return;

            addr.StateProvince = stateProvince;
            addr.StateProvinceId = stateProvince.Id;
        }

        public void FixCountryIds(Address addr)
        {
            if (addr == null || string.IsNullOrWhiteSpace(addr.Country?.CountryName))
                return;

            var newCountry = countryService.FindCountryByCountryName(addr.Country.CountryName);
            if (newCountry == null)
                throw new ValidationException("Invalid Country Name");


            addr.Country = newCountry;
            addr.CountryId = newCountry.Id;
        }

        public void UpdateAddressesForTheCompany(int? addressId, int companyId, AddressType addressType)
        {
            if(addressId == null)
                return;

            var addrId = (int) addressId;
            var addr = addressRepository.GetAddressById(addrId);
            addr.CompanyId = companyId;

            switch (addressType)
            {
                case AddressType.Main:
                {
                    addr.isMainAddress = true;
                    break;
                }
                case AddressType.Shipping:
                {
                    addr.isShipping = true;
                    break;
                }
                case AddressType.Billing:
                {
                    addr.isBilling = true;
                    break;
                }
                case AddressType.Mailing:
                {
                    addr.isMailingAddress = true;
                    break;
                }
            }

            addressRepository.Update(addr);
        }

        public IQueryable<Address> FindAllFromCompanies(int id)
        {
            return addressRepository.FindAllFromCompanies(id);
        }
    }

    public enum AddressType
    {
        Main,
        Shipping,
        Billing,
        Mailing
    }
}
