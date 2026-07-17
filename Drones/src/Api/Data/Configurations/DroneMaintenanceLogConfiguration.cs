using Drones.src.Api.Drones.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Drones.src.Api.Data.Configurations
{
    public class DroneMaintenanceLogConfiguration: IEntityTypeConfiguration<DroneMaintenanceLog>
    {
        public void Configure(EntityTypeBuilder<DroneMaintenanceLog> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Type)
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(m => m.Notes)
                .HasMaxLength(1024)
                .IsRequired();

            builder.HasOne(m => m.Drone)
                .WithMany(d => d.MaintenanceLogs)
                .HasForeignKey(m => m.DroneId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
