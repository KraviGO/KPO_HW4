using Accounts.Infrastructure.Persistence;
using Accounts.UseCases.Abstractions;

namespace Accounts.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly AccountsDbContext _db;

    public UnitOfWork(AccountsDbContext db)
    {
        _db = db;
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
    {
        return _db.SaveChangesAsync(ct);
    }
}