using Microsoft.EntityFrameworkCore;
using Payments.Entities.Models;
using Payments.Infrastructure.Persistence;
using Payments.UseCases.Abstractions;
using SharedKernel.ValueObjects;

namespace Payments.Infrastructure.Repositories;

public class PaymentAccountRepository : IPaymentAccountRepository
{
    private readonly PaymentsDbContext _db;
    public PaymentAccountRepository(PaymentsDbContext db) => _db = db;

    public Task<PaymentAccount?> GetByAccountNumberAsync(AccountNumber number, CancellationToken ct) =>
        _db.PaymentAccounts.FirstOrDefaultAsync(x => x.AccountNumber.Value == number.Value, ct);

    public Task AddAsync(PaymentAccount account, CancellationToken ct) =>
        _db.PaymentAccounts.AddAsync(account, ct).AsTask();
}
