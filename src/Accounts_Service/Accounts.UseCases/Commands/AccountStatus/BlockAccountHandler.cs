using Accounts.UseCases.Abstractions;

namespace Accounts.UseCases.Commands.AccountStatus;

public class BlockAccountHandler
{
    private readonly IAccountRepository _accounts;
    private readonly IUnitOfWork _uow;

    public BlockAccountHandler(
        IAccountRepository accounts,
        IUnitOfWork uow)
    {
        _accounts = accounts;
        _uow = uow;
    }

    public async Task Handle(BlockAccountCommand cmd, CancellationToken ct)
    {
        var account = await _accounts.GetByNumberAsync(cmd.AccountNumber, ct)
                      ?? throw new InvalidOperationException("Account not found");

        account.Block();

        await _uow.SaveChangesAsync(ct);
    }
}