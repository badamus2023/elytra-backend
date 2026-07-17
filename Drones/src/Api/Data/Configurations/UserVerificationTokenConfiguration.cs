using Drones.src.Api.Auth.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Drones.src.Api.Data.Configurations
{
    public class UserVerificationTokenConfiguration : IEntityTypeConfiguration<UserVerficationToken>
    {
        public void Configure(EntityTypeBuilder<UserVerficationToken> builder)
        {
            builder.HasKey(vt => vt.Id);

            builder.HasIndex(vt => vt.Token).IsUnique();

            builder.Property(vt => vt.Token)
                .HasMaxLength(128)
                .IsRequired();

            builder.HasOne(vt => vt.User)
                .WithOne(u => u.VerificationToken)
                .HasForeignKey<UserVerficationToken>(vt => vt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
