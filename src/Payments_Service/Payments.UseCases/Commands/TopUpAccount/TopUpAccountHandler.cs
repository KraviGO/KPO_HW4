using Payments.Entities.Models;
using Payments.UseCases.Abstractions;

namespace Payments.UseCases.Commands.TopUpAccount;

public class TopUpAccountHandler
{
    private readonly IPaymentAccountRepository _accounts;
    private readonly IUnitOfWork _uow;

    public TopUpAccountHandler(IPaymentAccountRepository accounts, IUnitOfWork uow)
    {
        _accounts = accounts;
        _uow = uow;
    }

    public async Task Handle(TopUpAccountCommand cmd, CancellationToken ct)
    {
        var account = await _accounts.GetByAccountNumberAsync(cmd.AccountNumber, ct);
        if (account == null)
        {
            account = PaymentAccount.Create(cmd.AccountNumber);
            await _accounts.AddAsync(account, ct);
        }

        account.TopUp(cmd.Amount);
        await _uow.SaveChangesAsync(ct);
    }
}
