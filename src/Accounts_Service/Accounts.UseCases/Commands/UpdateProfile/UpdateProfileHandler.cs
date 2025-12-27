using Accounts.UseCases.Abstractions;

namespace Accounts.UseCases.Commands.UpdateProfile;

public class UpdateProfileHandler
{
    private readonly IAccountRepository _accounts;
    private readonly IUnitOfWork _uow;
    
    public UpdateProfileHandler(
        IAccountRepository accounts,
        IUnitOfWork uow)
    {
        _accounts = accounts;
        _uow = uow;
    }

    public async Task Handle(UpdateProfileCommand cmd, CancellationToken ct)
    {
        var account = await _accounts.GetByNumberAsync(cmd.AccountNumber, ct)
                      ?? throw new InvalidOperationException("Account not found");

        account.Profile.UpdateDescription(cmd.Description);
        await _uow.SaveChangesAsync(ct);
    }
}