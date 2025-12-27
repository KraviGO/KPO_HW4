using Payments.UseCases.Abstractions;

namespace Payments.UseCases.Queries.GetBalance;

public class GetBalanceHandler
{
    private readonly IPaymentAccountRepository _accounts;

    public GetBalanceHandler(IPaymentAccountRepository accounts)
    {
        _accounts = accounts;
    }

    public async Task<decimal> Handle(GetBalanceQuery query, CancellationToken ct)
    {
        var account = await _accounts.GetByAccountNumberAsync(query.AccountNumber, ct)
                      ?? throw new InvalidOperationException("Account not found");

        return account.Balance;
    }
}
