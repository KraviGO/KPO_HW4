namespace Payments.Presentation.Contracts.Accounts;

public record TopUpRequest(string AccountNumber, decimal Amount);
