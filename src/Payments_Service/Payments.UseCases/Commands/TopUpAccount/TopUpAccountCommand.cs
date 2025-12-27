using SharedKernel.ValueObjects;

namespace Payments.UseCases.Commands.TopUpAccount;

public record TopUpAccountCommand(AccountNumber AccountNumber, decimal Amount);
