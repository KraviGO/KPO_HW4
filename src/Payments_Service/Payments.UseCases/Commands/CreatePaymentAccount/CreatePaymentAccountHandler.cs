using Payments.Entities.Models;
using Payments.UseCases.Abstractions;

namespace Payments.UseCases.Commands.CreatePaymentAccount;

public class CreatePaymentAccountHandler
{
    private readonly IPaymentAccountRepository _accounts;
    private readonly IUnitOfWork _uow;

    public CreatePaymentAccountHandler(IPaymentAccountRepository accounts, IUnitOfWork uow)
    {
        _accounts = accounts;
        _uow = uow;
    }

    public async Task Handle(CreatePaymentAccountCommand cmd, CancellationToken ct)
    {
        var existing = await _accounts.GetByAccountNumberAsync(cmd.AccountNumber, ct);
        if (existing != null)
            return;

        await _accounts.AddAsync(PaymentAccount.Create(cmd.AccountNumber), ct);
        await _uow.SaveChangesAsync(ct);
    }
}
