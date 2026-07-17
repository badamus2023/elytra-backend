using Drones.src.Api.Orders.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Drones.src.Api.Data.Configurations
{
    public class OrderItemConfiguration: IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder) 
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");

            builder.Property(i => i.ProductName).HasMaxLength(256).IsRequired();

            builder.Property(i => i.ProductId)
            .IsRequired();
        }
    }
}
