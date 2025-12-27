namespace Orders.Presentation.Contracts;

public record CreateOrderRequest(
    string AccountNumber,
    decimal Amount,
    string Description
);