using SharedKernel.ValueObjects;

namespace Payments.UseCases.Commands.CreatePaymentAccount;

public record CreatePaymentAccountCommand(AccountNumber AccountNumber);
