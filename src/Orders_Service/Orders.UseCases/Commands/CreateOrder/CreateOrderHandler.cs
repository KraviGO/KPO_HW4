using System.Text.Json;
using Orders.Entities.Models;
using Orders.UseCases.Abstractions;

namespace Orders.UseCases.Commands.CreateOrder;

public class CreateOrderHandler
{
    private readonly IOrderRepository _orders;
    private readonly IOutboxMessageRepository _outbox;
    private readonly IUnitOfWork _uow;

    public CreateOrderHandler(
        IOrderRepository orders,
        IOutboxMessageRepository outbox,
        IUnitOfWork uow)
    {
        _orders = orders;
        _outbox = outbox;
        _uow = uow;
    }

    public async Task<string> Handle(CreateOrderCommand cmd, CancellationToken ct)
    {
        var order = new Order(
            cmd.AccountNumber,
            cmd.Amount,
            cmd.Description);

        await _orders.AddAsync(order, ct);

        var message = new
        {
            MessageId = Guid.NewGuid(),
            OrderId = order.Id,
            AccountNumber = order.AccountNumber.Value,
            Amount = order.Amount
        };

        await _outbox.AddAsync(
            new OutboxMessage(
                "OrderPaymentRequested",
                JsonSerializer.Serialize(message)),
            ct);

        await _uow.SaveChangesAsync(ct);

        return order.PublicId;
    }
}