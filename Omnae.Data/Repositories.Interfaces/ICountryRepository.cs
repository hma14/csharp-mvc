using Omnae.Model.Models;
using Omnae.Data.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Data.Repositories.Interfaces
{
    public interface ICountryRepository : IRepository<Country>
    {
        List<Country> GetCountries();
        Country GetCountryById(int id);
        Country GetCountryByCountryName(string countryName);
        Country GetCountryByCountryCode(string countryCode);
        Country GetCountryByCountryCodeOrName(string countryCodeOrName);
        IQueryable<Country> GetCountryList();
    }
}
