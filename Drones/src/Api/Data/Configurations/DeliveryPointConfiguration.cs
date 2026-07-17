using Drones.src.Api.DeliveryPoints.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Drones.src.Api.Data.Configurations
{
    public class DeliveryPointConfiguration : IEntityTypeConfiguration<DeliveryPoint>
    {
        public void Configure(EntityTypeBuilder<DeliveryPoint> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(p => p.Address)
                .HasMaxLength(512)
                .IsRequired();

            builder.HasMany(p => p.Orders)
                .WithOne(o => o.DeliveryPoint)
                .HasForeignKey(o => o.DeliveryPointId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
