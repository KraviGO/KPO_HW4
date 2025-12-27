using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payments.Presentation.Contracts.Accounts;
using Payments.UseCases.Abstractions;
using Payments.UseCases.Commands.CreatePaymentAccount;
using Payments.UseCases.Commands.TopUpAccount;
using Payments.UseCases.Queries.GetBalance;
using SharedKernel.ValueObjects;

namespace Payments.Presentation.Controllers;

[ApiController]
[Route("payment-accounts")]
public class PaymentAccountsController : ControllerBase
{
    private readonly IPaymentAccountRepository _accounts;
    private readonly CreatePaymentAccountHandler _create;
    private readonly TopUpAccountHandler _topUp;
    private readonly GetBalanceHandler _getBalance;

    public PaymentAccountsController(
        IPaymentAccountRepository accounts,
        CreatePaymentAccountHandler create,
        TopUpAccountHandler topUp,
        GetBalanceHandler getBalance)
    {
        _accounts = accounts;
        _create = create;
        _topUp = topUp;
        _getBalance = getBalance;
    }

    [HttpPost]
    [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreatePaymentAccountRequest request, CancellationToken ct)
    {
        var number = (request.AccountNumber ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(number))
            return BadRequest("AccountNumber is required");

        var accountNumber = new AccountNumber(number);
        await _create.Handle(new CreatePaymentAccountCommand(accountNumber), ct);

        var acc = await _accounts.GetByAccountNumberAsync(accountNumber, ct);
        return Created($"/payment-accounts/{number}", new AccountResponse(acc!.AccountNumber.Value, acc.Balance));
    }

    [HttpPost("topup")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TopUp([FromBody] TopUpRequest request, CancellationToken ct)
    {
        var number = (request.AccountNumber ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(number))
            return BadRequest("AccountNumber is required");
        if (request.Amount <= 0)
            return BadRequest("Amount must be positive");

        await _topUp.Handle(new TopUpAccountCommand(new AccountNumber(number), request.Amount), ct);
        return NoContent();
    }

    [HttpGet("{accountNumber}/balance")]
    [ProducesResponseType(typeof(BalanceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBalance([FromRoute] string accountNumber, CancellationToken ct)
    {
        var number = (accountNumber ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(number))
            return NotFound();

        var acc = await _accounts.GetByAccountNumberAsync(new AccountNumber(number), ct);
        if (acc == null)
            return NotFound();

        var balance = await _getBalance.Handle(new GetBalanceQuery(acc.AccountNumber), ct);
        return Ok(new BalanceResponse(acc.AccountNumber.Value, balance));
    }
}
