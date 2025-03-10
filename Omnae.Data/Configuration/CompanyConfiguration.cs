using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Data.Configuration
{
    public class CompanyConfiguration : EntityTypeConfiguration<Company>
    {
        public CompanyConfiguration()
        {
            HasOptional<Address>(f => f.Address)
               .WithMany()
               .HasForeignKey(f => f.AddressId)
               .WillCascadeOnDelete(true);

            HasOptional<Shipping>(f => f.Shipping)
                .WithMany()
                .HasForeignKey(f => f.ShippingId)
                .WillCascadeOnDelete(true);
        }
    }
}
