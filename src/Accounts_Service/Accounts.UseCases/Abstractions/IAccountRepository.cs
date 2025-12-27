using Accounts.Entities.Models;
using SharedKernel.ValueObjects;

namespace Accounts.UseCases.Abstractions;

public interface IAccountRepository
{
    Task<Account?> GetByNumberAsync(AccountNumber number, CancellationToken ct);
    Task AddAsync(Account account, CancellationToken ct);
}