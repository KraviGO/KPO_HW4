using Accounts.Entities.Models;
using Accounts.Infrastructure.Persistence;
using Accounts.UseCases.Abstractions;
using Microsoft.EntityFrameworkCore;
using SharedKernel.ValueObjects;

namespace Accounts.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly AccountsDbContext _db;

    public AccountRepository(AccountsDbContext db)
    {
        _db = db;
    }

    public Task<Account?> GetByNumberAsync(AccountNumber number, CancellationToken ct)
    {
        return _db.Accounts
            .FirstOrDefaultAsync(x => x.Number.Value == number.Value, ct);
    }

    public async Task AddAsync(Account account, CancellationToken ct)
    {
        await _db.Accounts.AddAsync(account, ct);
    }
}