using Drones.src.Api.Payments.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Drones.src.Api.Data.Configurations
{
    public class PaymentConfiguration: IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder) 
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.Status)
                .HasConversion<string>();

            builder.Property(p => p.Provider)
                .HasConversion<string>();

            builder.Property(p => p.Currency)
                .HasMaxLength(3);
        }

    }
}
