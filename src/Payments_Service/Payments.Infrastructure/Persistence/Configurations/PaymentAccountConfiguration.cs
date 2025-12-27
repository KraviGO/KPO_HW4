using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payments.Entities.Models;

namespace Payments.Infrastructure.Persistence.Configurations;

public class PaymentAccountConfiguration : IEntityTypeConfiguration<PaymentAccount>
{
    public void Configure(EntityTypeBuilder<PaymentAccount> b)
    {
        b.ToTable("payment_accounts");
        b.HasKey(x => x.Id);

        b.OwnsOne(x => x.AccountNumber, nb =>
        {
            nb.Property(p => p.Value)
                .HasColumnName("account_number")
                .IsRequired();

            nb.HasIndex(p => p.Value).IsUnique();
        });

        b.Property(x => x.Balance).IsRequired();
        b.Property(x => x.Version)
            .IsRequired()
            .IsConcurrencyToken();
    }
}
