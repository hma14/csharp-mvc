using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Data.Configuration
{
    public class AddressConfiguration : EntityTypeConfiguration<Address>
    {
        public AddressConfiguration()
        {
            HasOptional<Country>(f => f.Country)
               .WithMany()
               .HasForeignKey(f => f.CountryId)
               .WillCascadeOnDelete(true);           
        }
    }
}
