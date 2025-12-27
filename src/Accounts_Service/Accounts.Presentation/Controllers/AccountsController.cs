using Accounts.Presentation.Contracts;
using Accounts.UseCases.Commands.AccountStatus;
using Accounts.UseCases.Commands.CreateAccount;
using Accounts.UseCases.Commands.UpdateProfile;
using Accounts.UseCases.Queries.GetAccount;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.ValueObjects;

namespace Accounts.Presentation.Controllers;

[ApiController]
[Route("accounts")]
public class AccountsController : ControllerBase
{
    private readonly CreateAccountHandler _createAccount;
    private readonly GetAccountHandler _getAccount;
    private readonly UpdateProfileHandler _updateProfile;
    private readonly BlockAccountHandler _blockAccount;
    private readonly ActivateAccountHandler _activateAccount;

    public AccountsController(
        CreateAccountHandler createAccount,
        GetAccountHandler getAccount,
        UpdateProfileHandler updateProfile,
        BlockAccountHandler blockAccount,
        ActivateAccountHandler activateAccount)
    {
        _createAccount = createAccount;
        _getAccount = getAccount;
        _updateProfile = updateProfile;
        _blockAccount = blockAccount;
        _activateAccount = activateAccount;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateAccountResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(
        [FromBody] CreateAccountRequest request,
        CancellationToken ct)
    {
        var number = await _createAccount.Handle(
            new CreateAccountCommand(
                request.FirstName,
                request.LastName,
                request.Description),
            ct);

        return Created(
            $"/accounts/{number}",
            new CreateAccountResponse(number.Value));
    }

    [HttpGet("{accountNumber}")]
    [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(
        [FromRoute] string accountNumber,
        CancellationToken ct)
    {
        var acc = await _getAccount.Handle(
            new GetAccountQuery(new AccountNumber(accountNumber)),
            ct);

        if (acc is null)
            return NotFound();

        return Ok(new AccountResponse(
            acc.Number.Value,
            acc.Profile.FirstName,
            acc.Profile.LastName,
            acc.Profile.Description,
            acc.Status.ToString()));
    }

    [HttpPut("{accountNumber}/profile")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateProfile(
        string accountNumber,
        [FromBody] UpdateProfileRequest request,
        CancellationToken ct)
    {
        await _updateProfile.Handle(
            new UpdateProfileCommand(
                new AccountNumber(accountNumber),
                request.Description),
            ct);

        return NoContent();
    }

    [HttpPost("{accountNumber}/block")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Block(
        string accountNumber,
        CancellationToken ct)
    {
        await _blockAccount.Handle(
            new BlockAccountCommand(new AccountNumber(accountNumber)),
            ct);

        return NoContent();
    }

    [HttpPost("{accountNumber}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Activate(
        string accountNumber,
        CancellationToken ct)
    {
        await _activateAccount.Handle(
            new ActivateAccountCommand(new AccountNumber(accountNumber)),
            ct);

        return NoContent();
    }
}