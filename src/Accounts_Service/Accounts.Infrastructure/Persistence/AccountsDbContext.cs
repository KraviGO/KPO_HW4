using Accounts.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Infrastructure.Persistence;

public class AccountsDbContext : DbContext
{
    public DbSet<Account> Accounts => Set<Account>();

    public AccountsDbContext(DbContextOptions<AccountsDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccountsDbContext).Assembly);
    }
}