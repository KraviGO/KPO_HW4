using Accounts.UseCases.Abstractions;

namespace Accounts.UseCases.Commands.AccountStatus;

public class ActivateAccountHandler
{
    private readonly IAccountRepository _accounts;
    private readonly IUnitOfWork _uow;

    public ActivateAccountHandler(
        IAccountRepository accounts,
        IUnitOfWork uow)
    {
        _accounts = accounts;
        _uow = uow;
    }

    public async Task Handle(ActivateAccountCommand cmd, CancellationToken ct)
    {
        var account = await _accounts.GetByNumberAsync(cmd.AccountNumber, ct)
                      ?? throw new InvalidOperationException("Account not found");

        account.Activate();

        await _uow.SaveChangesAsync(ct);
    }
}