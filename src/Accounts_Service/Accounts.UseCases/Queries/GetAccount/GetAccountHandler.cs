using Accounts.Entities.Models;
using Accounts.UseCases.Abstractions;

namespace Accounts.UseCases.Queries.GetAccount;

public class GetAccountHandler
{
    private readonly IAccountRepository _accounts;

    public GetAccountHandler(IAccountRepository accounts)
    {
        _accounts = accounts;
    }

    public async Task<Account?> Handle(GetAccountQuery q, CancellationToken ct)
    {
        return await _accounts.GetByNumberAsync(q.AccountNumber, ct);
    }
}