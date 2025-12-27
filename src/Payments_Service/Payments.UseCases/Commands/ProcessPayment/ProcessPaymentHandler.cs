using System.Text.Json;
using Payments.Entities.Models;
using Payments.UseCases.Abstractions;
using SharedKernel.ValueObjects;

namespace Payments.UseCases.Commands.ProcessPayment;

public class ProcessPaymentHandler
{
    private readonly IPaymentAccountRepository _accounts;
    private readonly IPaymentOperationRepository _payments;
    private readonly IInboxMessageRepository _inbox;
    private readonly IOutboxMessageRepository _outbox;
    private readonly IUnitOfWork _uow;

    public ProcessPaymentHandler(
        IPaymentAccountRepository accounts,
        IPaymentOperationRepository payments,
        IInboxMessageRepository inbox,
        IOutboxMessageRepository outbox,
        IUnitOfWork uow)
    {
        _accounts = accounts;
        _payments = payments;
        _inbox = inbox;
        _outbox = outbox;
        _uow = uow;
    }

    public async Task Handle(ProcessPaymentCommand cmd, CancellationToken ct)
    {
        if (await _inbox.ExistsAsync(cmd.MessageId, ct))
            return;

        await _inbox.AddAsync(
            new InboxMessage(cmd.MessageId, nameof(ProcessPaymentCommand), string.Empty),
            ct);

        await _uow.SaveChangesAsync(ct);

        if (await _payments.GetByOrderIdAsync(cmd.OrderId, ct) != null)
            return;

        var account = await _accounts.GetByAccountNumberAsync(cmd.AccountNumber, ct);
        if (account == null)
        {
            var resultMessage = new
            {
                cmd.OrderId,
                Status = "Failed",
                Reason = "AccountNotFound"
            };

            await _outbox.AddAsync(
                new OutboxMessage("PaymentFailed", JsonSerializer.Serialize(resultMessage)),
                ct);

            await _uow.SaveChangesAsync(ct);
            return;
        }

        var payment = PaymentOperation.ForOrder(
            cmd.OrderId,
            cmd.AccountNumber,
            cmd.Amount,
            PaymentStatus.Pending);

        var sufficientFunds = account.TryWithdraw(cmd.Amount);
        if (sufficientFunds)
            payment.MarkSucceeded();
        else
            payment.MarkFailed("InsufficientFunds");

        await _payments.AddAsync(payment, ct);

        var okResult = new
        {
            cmd.OrderId,
            Status = payment.Status == PaymentStatus.Succeeded ? "Succeeded" : "Failed",
            Reason = payment.Status == PaymentStatus.Succeeded ? null : "InsufficientFunds"
        };

        await _outbox.AddAsync(
            new OutboxMessage(
                payment.Status == PaymentStatus.Succeeded ? "PaymentSucceeded" : "PaymentFailed",
                JsonSerializer.Serialize(okResult)),
            ct);

        await _uow.SaveChangesAsync(ct);
    }
}
