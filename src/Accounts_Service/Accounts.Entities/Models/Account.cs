using SharedKernel.ValueObjects;

namespace Accounts.Entities.Models;

public class Account
{
    public Guid Id { get; private set; }
    public AccountNumber Number { get; private set; } = null!;
    public AccountProfile Profile { get; private set; } = null!;
    public AccountStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private Account() {}

    public static Account Create(
        string firstName,
        string lastName,
        string? description)
    {
        return new Account
        {
            Id = Guid.NewGuid(),
            Number = AccountNumber.Generate(),
            Profile = new AccountProfile(firstName, lastName, description),
            Status = AccountStatus.Active,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
    
    public void ChangeStatus(AccountStatus newStatus)
    {
        if (Status == newStatus)
            return;

        if (Status == AccountStatus.Closed)
            throw new InvalidOperationException("Closed account cannot change status");

        Status = newStatus;
    }

    public void Block()
    {
        ChangeStatus(AccountStatus.Blocked);
    }

    public void Activate()
    {
        ChangeStatus(AccountStatus.Active);
    }
}