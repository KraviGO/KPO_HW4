using SharedKernel.ValueObjects;

namespace Payments.UseCases.Commands.ProcessPayment;

public record ProcessPaymentCommand(
    Guid MessageId,
    Guid OrderId,
    AccountNumber AccountNumber,
    decimal Amount
);
