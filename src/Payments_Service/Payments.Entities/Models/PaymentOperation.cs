using SharedKernel.ValueObjects;

namespace Payments.Entities.Models;

public class PaymentOperation
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public AccountNumber AccountNumber { get; private set; } = null!;
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? Reason { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private PaymentOperation() { }

    public static PaymentOperation ForOrder(
        Guid orderId,
        AccountNumber accountNumber,
        decimal amount,
        PaymentStatus status,
        string? reason = null)
    {
        if (orderId == Guid.Empty)
            throw new InvalidOperationException("OrderId is required");

        if (accountNumber is null)
            throw new InvalidOperationException("AccountNumber is required");

        if (amount <= 0)
            throw new InvalidOperationException("Amount must be positive");

        return new PaymentOperation
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            AccountNumber = accountNumber,
            Amount = amount,
            Status = status,
            Reason = reason,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public void MarkSucceeded() => Status = PaymentStatus.Succeeded;
    public void MarkFailed(string? reason = null)
    {
        Status = PaymentStatus.Failed;
        Reason = reason;
    }
}

public enum PaymentStatus
{
    Pending = 0,
    Succeeded = 1,
    Failed = 2
}
