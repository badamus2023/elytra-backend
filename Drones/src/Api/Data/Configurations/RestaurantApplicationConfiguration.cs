using Drones.src.Api.Restaurants.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Drones.src.Api.Data.Configurations
{
    public class RestaurantApplicationConfiguration : IEntityTypeConfiguration<RestaurantApplication>
    {
        public void Configure(EntityTypeBuilder<RestaurantApplication> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Status).HasConversion<string>();
            builder.Property(x => x.CompanyName).HasMaxLength(256).IsRequired();
            builder.Property(x => x.TaxId).HasMaxLength(32).IsRequired();
            builder.HasIndex(x => x.TaxId).IsUnique();
            builder.HasOne(x => x.User).WithMany(x => x.RestaurantApplications)
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Restaurant).WithMany()
                .HasForeignKey(x => x.RestaurantId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
