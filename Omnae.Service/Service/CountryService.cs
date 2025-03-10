using Omnae.Service.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omnae.Model.Models;
using Omnae.Data.Abstracts;
using Omnae.Data.Repositories.Interfaces;

namespace Omnae.Service.Service
{
    public class CountryService : ICountryService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ICountryRepository countryRepository;

        public CountryService(ICountryRepository countryRepository, IUnitOfWork unitOfWork)
        {
            this.countryRepository = countryRepository;
            this.unitOfWork = unitOfWork;
        }

        public List<Country> FindAllCountries()
        {
            return countryRepository.GetCountries();
        }
        public IQueryable<Country> FindAllCountryList()
        {
            return countryRepository.GetCountryList();
        }

        public Country FindCountryById(int id)
        {
            return countryRepository.GetCountryById(id);
        }

        public Country FindCountryByCountryName(string countryName)
        {
            return countryRepository.GetCountryByCountryName(countryName);
        }

        public Country FindCountryByCountryCode(string countryCode)
        {
            return countryRepository.GetCountryByCountryCode(countryCode);
        }

        public Country FindCountryByCountryCodeOrName(string countryCodeOrName)
        {
            return countryRepository.GetCountryByCountryCodeOrName(countryCodeOrName);
        }
    }
}
