using System.Linq;
using AutoMapper;
using Omnae.BusinessLayer.Identity;
using Omnae.BusinessLayer.Models;
using Omnae.Model.Models;
using Omnae.Service.Service;
using Omnae.Service.Service.Interfaces;
using Omnae.Service.Service.Model;
using Serilog;

namespace Omnae.BusinessLayer
{
    public class CompanyBL
    {
        private ILogger Log { get; }
        private IMapper Mapper { get; }

        private IShippingService ShippingService { get; }
        private IAddressService AddressService { get; }
        private ICompanyService CompanyService { get; }
        private UserContactService UserContactService { get; }
        private ApplicationUserManager UserManager { get; }
        private ICompanyBankInfoService CompanyBankInfoService { get; }
        private readonly IStateProvinceService StateProvinceService;
        private readonly ICountryService CountryService;

        public CompanyBL(ILogger log, IMapper mapper, IAddressService addressService, ICompanyService companyService, UserContactService userContactService, ApplicationUserManager userManager, IShippingService shippingService, ICompanyBankInfoService companyBankInfoService, IStateProvinceService stateProvinceService, ICountryService countryService)
        {
            Log = log;
            Mapper = mapper;
            AddressService = addressService;
            CompanyService = companyService;
            UserContactService = userContactService;
            UserManager = userManager;
            ShippingService = shippingService;
            CompanyBankInfoService = companyBankInfoService;
            StateProvinceService = stateProvinceService;
            CountryService = countryService;
        }

        public void UpdateCompanyAndAddresses(Company entity)
        {
            entity.AllAddresses?.Clear();

            var address = entity.Address;
            var shippingAddress = entity.Shipping?.Address;
            var billAddress = entity.BillAddress;
            var mainCompanyAddress = entity.MainCompanyAddress;

            if (address != null)
            {
                AddressService.Update(address);
                AddressService.UpdateAddressesForTheCompany(address?.Id, entity.Id, AddressType.Main);
                entity.AddressId = address.Id;
            }
            if (shippingAddress != null)
            {
                AddressService.Update(shippingAddress);
                AddressService.UpdateAddressesForTheCompany(shippingAddress?.Id, entity.Id, AddressType.Shipping);
                entity.Shipping.AddressId = shippingAddress.Id;
            }
            if (entity.Shipping != null)
            {
                entity.Shipping.CompanyId = entity.Id;
                ShippingService.UpdateShipping(entity.Shipping);
            }
            if (billAddress != null)
            {
                AddressService.Update(billAddress);
                AddressService.UpdateAddressesForTheCompany(billAddress?.Id, entity.Id, AddressType.Billing);
                entity.BillAddressId = billAddress.Id;
            }
            if (mainCompanyAddress != null)
            {
                AddressService.Update(mainCompanyAddress);
                AddressService.UpdateAddressesForTheCompany(mainCompanyAddress?.Id, entity.Id, AddressType.Mailing);
                entity.MainCompanyAddress_Id = mainCompanyAddress.Id;
            }

            CompanyService.UpdateCompany(entity);
        }

        public UserContactInformationModel GetAdminUser(int companyId)
        {
            return UserContactService.GetAllActiveUserConnectFromCompany(companyId).FirstOrDefault(); //TODO TODO, Add Filter to Get only the ADMIN
        }

        public CompanyBankInfo CreateCompanyBankInfo(CompanyBankInfoViewModel cbi)
        {
            var model = Mapper.Map<CompanyBankInfo>(cbi);
            var stateProvinceId = StateProvinceService.FindStateProvinceByName(cbi.StateOrProvinceName)?.Id;
            if (stateProvinceId == null)
            {
                var newProvince = new StateProvince
                {
                    Name = cbi.StateOrProvinceName,

                };
                stateProvinceId = StateProvinceService.AddStateProvince(newProvince);
            }
            var addr = new Address
            {
                AddressLine1 = cbi.AddressLine1,
                AddressLine2 = cbi.AddressLine2,
                City = cbi.City,
                StateProvinceId = stateProvinceId,
                CountryId = cbi.CountryId,
                ZipCode = cbi.ZipCode,
                PostalCode = cbi.PostalCode,
            };
            model.BankAddressId = AddressService.AddAddress(addr);

            // store to db
            CompanyBankInfoService.AddCompanyBankInfo(model);

            model.BankAddress.StateProvince = StateProvinceService.FindStateProvinceById(stateProvinceId.Value);
            model.BankAddress.Country = CountryService.FindCountryById(cbi.CountryId);

            return model;
        }

        public CompanyBankInfo UpdateCompanyBankInfo(int id, CompanyBankInfoViewModel cbi)
        {
            var original = CompanyBankInfoService.FindCompanyBankInfoById(id);
            var stateProvinceId = StateProvinceService.FindStateProvinceByName(cbi.StateOrProvinceName)?.Id;
            if (stateProvinceId == null)
            {
                var newProvince = new StateProvince
                {
                    Name = cbi.StateOrProvinceName,

                };
                stateProvinceId = StateProvinceService.AddStateProvince(newProvince);
            }
            var updated = Mapper.Map<CompanyBankInfo>(cbi);
            updated.BankAddress = original.BankAddress;
            updated.BankAddress.AddressLine1 = cbi.AddressLine1;
            updated.BankAddress.AddressLine2 = cbi.AddressLine2;
            updated.BankAddress.City = cbi.City;
            updated.BankAddress.StateProvinceId = stateProvinceId;
            updated.BankAddress.CountryId = cbi.CountryId;
            updated.BankAddress.ZipCode = cbi.ZipCode;
            updated.BankAddress.PostalCode = cbi.PostalCode;

            updated.BankAddress = AddressService.Update(updated.BankAddress);
            updated.BankAddressId = original.BankAddressId;
            updated.BankAddress.Country = CountryService.FindCountryById(cbi.CountryId);

            updated.Id = original.Id;
            return CompanyBankInfoService.UpdateCompanyBankInfo(updated);
        }


    }
}