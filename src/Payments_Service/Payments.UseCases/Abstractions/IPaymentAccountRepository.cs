using Payments.Entities.Models;
using SharedKernel.ValueObjects;

namespace Payments.UseCases.Abstractions;

public interface IPaymentAccountRepository
{
    Task<PaymentAccount?> GetByAccountNumberAsync(AccountNumber number, CancellationToken ct);
    Task AddAsync(PaymentAccount account, CancellationToken ct);
}
