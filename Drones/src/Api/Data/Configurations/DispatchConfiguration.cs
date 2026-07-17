using Drones.src.Api.Dispatches.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Drones.src.Api.Data.Configurations
{
    public class DispatchConfiguration : IEntityTypeConfiguration<Dispatch>
    {
        public void Configure(EntityTypeBuilder<Dispatch> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Status)
                .HasConversion<string>();

            builder.HasOne(d => d.Drone)
                .WithMany(dr => dr.Dispatches)
                .HasForeignKey(d => d.DroneId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
