namespace Accounts.Presentation.Contracts;

public record CreateAccountRequest(
    string FirstName,
    string LastName,
    string? Description
);