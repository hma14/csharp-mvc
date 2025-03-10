using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Data.Configuration
{
    public class OrderConfiguration : EntityTypeConfiguration<Order>
    {
        public OrderConfiguration()
        {
            HasRequired(f => f.Product)
                    .WithMany()
                    .HasForeignKey(f => f.ProductId)
                    .WillCascadeOnDelete(true);

            //HasOptional<TaskData>(o => o.TaskData)
            //        .WithMany(t => t.Orders)
            //        .HasForeignKey<int>(o => o.TaskId)
            //        .WillCascadeOnDelete(false);
        }

    }
}
