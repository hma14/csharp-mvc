using Omnae.Model.Models;
using Omnae.Data.Abstracts;
using Omnae.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Data.Repositories
{
    public class CountryRepository : RepositoryBase<Country>, ICountryRepository
    {
        public CountryRepository(OmnaeContext dbContext) : base(dbContext)
        {
        }

        public List<Country> GetCountries()
        {
            return this.DbContext.Countries.ToList();
        }
        public IQueryable<Country> GetCountryList()
        {
            return this.DbContext.Countries;
        }

        public Country GetCountryByCountryName(string countryName)
        {
            return this.DbContext.Countries.FirstOrDefault(x => x.CountryName.Equals(countryName, StringComparison.CurrentCultureIgnoreCase)); 
        }

        public Country GetCountryByCountryCode(string countryCode)
        {
            return this.DbContext.Countries.FirstOrDefault(x => x.CountryCode.Equals(countryCode, StringComparison.CurrentCultureIgnoreCase));
        }

        public Country GetCountryByCountryCodeOrName(string countryCodeOrName)
        {
            return this.DbContext.Countries.FirstOrDefault(x => x.CountryCode.Equals(countryCodeOrName, StringComparison.CurrentCultureIgnoreCase) 
                                                                || x.CountryName.Equals(countryCodeOrName, StringComparison.CurrentCultureIgnoreCase));
        }

        public Country GetCountryById(int id)
        {
            return this.DbContext.Countries.Where(x => x.Id == id).FirstOrDefault();
        }

    }
}
