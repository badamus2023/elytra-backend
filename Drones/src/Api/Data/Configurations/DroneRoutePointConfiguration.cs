using Drones.src.Api.Drones.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Drones.src.Api.Data.Configurations
{
    public class DroneRoutePointConfiguration : IEntityTypeConfiguration<DroneRoutePoint>
    {
        public void Configure(EntityTypeBuilder<DroneRoutePoint> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => new { x.DispatchId, x.RecordedAt });
            builder.HasOne(x => x.Dispatch).WithMany(x => x.RoutePoints)
                .HasForeignKey(x => x.DispatchId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
