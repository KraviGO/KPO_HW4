namespace Orders.Presentation.Contracts;

public record OrderDetailsResponse(
    string PublicId,
    string AccountNumber,
    decimal Amount,
    string Description,
    string Status
);