using Accounts.Entities.Models;
using SharedKernel.ValueObjects;

namespace Accounts.UseCases.Commands.AccountStatus;

public record BlockAccountCommand(AccountNumber AccountNumber);