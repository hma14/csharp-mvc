using Omnae.Model.Models;
using Omnae.Data.Repositories.Interfaces;
using Omnae.Service.Service.Interfaces;
using System;
using System.Linq;
using Omnae.Model.Context;

namespace Omnae.Service.Service
{
    public class ShippingProfileService : IShippingProfileService
    {
        private readonly IShippingProfileRepository shippingProfileRepository;
        private readonly ILogedUserContext logedUserContext;
        private readonly IShippingRepository shippingRepository;
        private readonly IAddressRepository addressRepository;


        public ShippingProfileService(IShippingProfileRepository shippingProfileRepository, ILogedUserContext logedUserContext, IShippingRepository shippingRepository, IAddressRepository addressRepository)
        {
            this.shippingProfileRepository = shippingProfileRepository;
            this.logedUserContext = logedUserContext;
            this.shippingRepository = shippingRepository;
            this.addressRepository = addressRepository;
        }

        public int Add(ShippingProfile entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.CreatedByUserId = logedUserContext.UserId;
            entity.ModifiedAt = DateTime.UtcNow;
            entity.ModifiedByUserId = logedUserContext.UserId;

            shippingProfileRepository.Add(entity);

            if (entity.IsDefault)
            {
                SetShippingProfileAsDefault(entity);
            }
            else if (entity.CompanyId != null)
            {
                shippingProfileRepository.AdjustTheDefaultShippingProfile((int) entity.CompanyId, entity);
            }

            return entity.Id;
        }

        public ShippingProfile FindById(int id)
        {
            return shippingProfileRepository.GetById(id);
        }

        public void Update(ShippingProfile entity)
        {
            entity.ModifiedAt = DateTime.UtcNow;
            entity.ModifiedByUserId = logedUserContext.UserId;

            if (entity.Shipping.Address.Id == default)
            {
                entity.Shipping.AddressId = addressRepository.AddAddress(entity.Shipping.Address);
            }
            if (entity.Shipping.Id == default)
            {
                entity.ShippingId = shippingRepository.AddShipping(entity.Shipping);
            }
            
            shippingProfileRepository.Update(entity);

            if (entity.IsDefault)
            {
                SetShippingProfileAsDefault(entity);
            }
            else if (entity.CompanyId != null)
            {
                shippingProfileRepository.AdjustTheDefaultShippingProfile((int) entity.CompanyId, entity);
            }
        }

        public void SetShippingProfileAsDefault(ShippingProfile entity)
        {
            entity.IsDefault = true;
            shippingProfileRepository.SetShippingProfileAsDefault(entity);
        }

        public IQueryable<ShippingProfile> FindAllByCompanyId(int companyId)
        {
            return shippingProfileRepository.ListAllByCompanyId(companyId).Where(s => s.IsActive);
        }

        public void Delete(ShippingProfile profileInDatabase)
        {
            profileInDatabase.IsActive = false;

            shippingProfileRepository.Update(profileInDatabase);

            if (profileInDatabase.IsDefault && profileInDatabase.CompanyId != null)
            {
                shippingProfileRepository.AdjustTheDefaultShippingProfile((int) profileInDatabase.CompanyId, profileInDatabase);
            }
        }
    }
}
