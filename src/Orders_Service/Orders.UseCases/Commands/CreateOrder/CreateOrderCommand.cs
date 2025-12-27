using SharedKernel.ValueObjects;

namespace Orders.UseCases.Commands.CreateOrder;

public record CreateOrderCommand(
    AccountNumber AccountNumber,
    decimal Amount,
    string Description
);