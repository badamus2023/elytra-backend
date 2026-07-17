using Drones.src.Api.Dispatches.Entities;
using Drones.src.Api.Orders.Entities;
using Drones.src.Api.Payments.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Drones.src.Api.Data.Configurations
{
    public class OrderConfiguration: IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");

            builder.Property(o => o.Status).HasConversion<string>();

            builder.HasMany(o => o.Items).WithOne(i => i.Order).HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(o => o.Payment)
                .WithOne(p => p.Order)
                    .HasForeignKey<Payment>(p => p.OrderId)
                        .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.Dispatch)
                .WithOne(d => d.Order)
                    .HasForeignKey<Dispatch>(d => d.OrderId)
                        .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
