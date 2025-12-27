namespace Accounts.Presentation.Contracts;

public record AccountResponse(
    string AccountNumber,
    string FirstName,
    string LastName,
    string? Description,
    string Status
);