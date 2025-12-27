using SharedKernel.ValueObjects;

namespace Orders.Entities.Models;

public class Order
{
    public Guid Id { get; private set; }

    public string PublicId { get; private set; } = null!;

    public AccountNumber AccountNumber { get; private set; } = null!;
    public decimal Amount { get; private set; }
    public string Description { get; private set; } = null!;
    public OrderStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public Order(AccountNumber accountNumber, decimal amount, string description)
    {
        if (accountNumber is null)
            throw new InvalidOperationException("AccountNumber is required");

        if (amount <= 0)
            throw new InvalidOperationException("Order amount must be positive");

        if (string.IsNullOrWhiteSpace(description))
            throw new InvalidOperationException("Description is required");

        Id = Guid.NewGuid();
        PublicId = GeneratePublicId();
        AccountNumber = accountNumber;
        Amount = amount;
        Description = description.Trim();
        Status = OrderStatus.New;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public void MarkFinished()
    {
        if (Status != OrderStatus.New)
            throw new InvalidOperationException("Only new order can be finished");

        Status = OrderStatus.Finished;
    }

    public void MarkCancelled()
    {
        if (Status != OrderStatus.New)
            throw new InvalidOperationException("Only new order can be cancelled");

        Status = OrderStatus.Cancelled;
    }

    private static string GeneratePublicId()
    {
        var date = DateTime.UtcNow.ToString("yyyyMMdd");
        var suffix = Random.Shared.Next(100000, 999999);
        return $"ORD-{date}-{suffix}";
    }
}