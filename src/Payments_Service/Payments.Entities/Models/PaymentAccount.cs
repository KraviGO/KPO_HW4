using SharedKernel.ValueObjects;

namespace Payments.Entities.Models;

public class PaymentAccount
{
    public Guid Id { get; private set; }
    public AccountNumber AccountNumber { get; private set; } = null!;
    public decimal Balance { get; private set; }
    public long Version { get; private set; }

    private PaymentAccount() { }

    public static PaymentAccount Create(AccountNumber number)
    {
        if (number is null)
            throw new InvalidOperationException("AccountNumber is required");

        return new PaymentAccount
        {
            Id = Guid.NewGuid(),
            AccountNumber = number,
            Balance = 0m,
            Version = 0
        };
    }

    public void TopUp(decimal amount)
    {
        if (amount <= 0)
            throw new InvalidOperationException("Top up amount must be positive");

        Balance += amount;
    }

    public bool TryWithdraw(decimal amount)
    {
        if (amount <= 0)
            throw new InvalidOperationException("Withdraw amount must be positive");

        if (Balance < amount)
            return false;

        Balance -= amount;
        return true;
    }
}
