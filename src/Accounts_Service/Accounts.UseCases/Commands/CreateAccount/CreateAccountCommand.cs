namespace Accounts.UseCases.Commands.CreateAccount;

public record CreateAccountCommand(
    string FirstName,
    string LastName,
    string? Description
);