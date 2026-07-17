using Drones.src.Api.Auth.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Drones.src.Api.Data.Configurations
{
    public class UserConfiguration: IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.Email).HasMaxLength(256).IsRequired();

            builder.Property(u => u.PasswordHash).IsRequired();

            builder.Property(u => u.CreatedAt).IsRequired();
        }
    }
}
