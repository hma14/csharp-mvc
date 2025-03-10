using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service.Interfaces
{
    public interface ICountryService
    {
        List<Country> FindAllCountries();
        Country FindCountryById(int id);
        Country FindCountryByCountryName(string countryName);
        Country FindCountryByCountryCode(string countryCode);
        Country FindCountryByCountryCodeOrName(string countryCodeOrName);
        IQueryable<Country> FindAllCountryList();
    }        
}
