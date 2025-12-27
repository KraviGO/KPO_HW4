using Accounts.Entities.Models;
using Accounts.UseCases.Abstractions;
using SharedKernel.ValueObjects;

namespace Accounts.UseCases.Commands.CreateAccount;

public class CreateAccountHandler
{
    private readonly IAccountRepository _accounts;
    private readonly IUnitOfWork _uow;

    public CreateAccountHandler(
        IAccountRepository accounts,
        IUnitOfWork uow)
    {
        _accounts = accounts;
        _uow = uow;
    }

    public async Task<AccountNumber> Handle(CreateAccountCommand cmd, CancellationToken ct)
    {
        var account = Account.Create(
            cmd.FirstName,
            cmd.LastName,
            cmd.Description);

        await _accounts.AddAsync(account, ct);
        await _uow.SaveChangesAsync(ct);

        return account.Number;
    }
}