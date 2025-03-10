using Omnae.Model.Models;
using System.Data.Entity.ModelConfiguration;

namespace Omnae.Data.Configuration
{
    public class ProductConfiguration : EntityTypeConfiguration<Product>
    {
        public ProductConfiguration()
        {
            HasOptional<PriceBreak>(p => p.PriceBreak)
                .WithMany()
                .HasForeignKey(f => f.PriceBreakId)
                .WillCascadeOnDelete(false);

            HasOptional<Company>(f => f.CustomerCompany)
                .WithMany()
                .HasForeignKey(f => f.CustomerId)
                .WillCascadeOnDelete(false);

            HasOptional<Company>(f => f.VendorCompany)
                .WithMany()
                .HasForeignKey(f => f.VendorId)
                .WillCascadeOnDelete(false);
        }
    }
}
