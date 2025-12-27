using Accounts.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounts.Infrastructure.Persistence.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> b)
    {
        b.ToTable("accounts");

        b.HasKey(x => x.Id);

        b.OwnsOne(x => x.Number, nb =>
        {
            nb.Property(n => n.Value)
                .HasColumnName("account_number")
                .IsRequired();

            nb.HasIndex(n => n.Value)
                .IsUnique();
        });

        b.OwnsOne(x => x.Profile, pb =>
        {
            pb.Property(p => p.FirstName)
                .HasColumnName("first_name")
                .HasMaxLength(100)
                .IsRequired();

            pb.Property(p => p.LastName)
                .HasColumnName("last_name")
                .HasMaxLength(100)
                .IsRequired();

            pb.Property(p => p.Description)
                .HasColumnName("description")
                .HasMaxLength(500);
        });

        b.Property(x => x.Status)
            .IsRequired();

        b.Property(x => x.CreatedAt)
            .IsRequired();
    }
}