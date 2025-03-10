using Omnae.Model.Models;
using System.Data.Entity.ModelConfiguration;

namespace Omnae.Data.Configuration
{
    public class TaskDataConfiguration : EntityTypeConfiguration<TaskData>
    {
        public TaskDataConfiguration()
        {           

            HasOptional<Product>(f => f.Product)
                .WithMany()
                .HasForeignKey(f => f.ProductId)
                .WillCascadeOnDelete(true);

        }
    }
}
