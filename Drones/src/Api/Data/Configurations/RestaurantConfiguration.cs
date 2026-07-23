using Drones.src.Api.Restaurants.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Drones.src.Api.Data.Configurations
{
    public class RestaurantConfiguration: IEntityTypeConfiguration<Restaurant>
    {
        public void Configure(EntityTypeBuilder<Restaurant> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(r => r.Address)
                .HasMaxLength(512)
                .IsRequired();

            builder.Property(r => r.Description)
                .HasMaxLength(1024);

            builder.Property(r => r.ImageUrl)
                .HasMaxLength(512);

            builder.HasOne(r => r.OwnerUser)
                .WithMany(u => u.OwnedRestaurants)
                .HasForeignKey(r => r.OwnerUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(r => r.Categories)
                .WithOne(c => c.Restaurant)
                .HasForeignKey(c => c.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.Orders)
                .WithOne(o => o.Restaurant)
                .HasForeignKey(o => o.RestaurantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(r => r.Reviews)
                .WithOne(rv => rv.Restaurant)
                .HasForeignKey(rv => rv.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
