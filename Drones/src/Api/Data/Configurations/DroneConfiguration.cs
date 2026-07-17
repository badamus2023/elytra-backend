using Drones.src.Api.Drones.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Drones.src.Api.Data.Configurations
{
    public class DroneConfiguration : IEntityTypeConfiguration<Drone>
    {
        public void Configure(EntityTypeBuilder<Drone> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Name)
                .HasMaxLength(128)
                .IsRequired();

            builder.Property(d => d.Status)
                .HasConversion<string>();
        }
    }
}
