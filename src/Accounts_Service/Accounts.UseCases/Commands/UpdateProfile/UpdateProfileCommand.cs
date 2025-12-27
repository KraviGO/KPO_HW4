using Accounts.Entities.Models;
using SharedKernel.ValueObjects;

namespace Accounts.UseCases.Commands.UpdateProfile;

public record UpdateProfileCommand(
    AccountNumber AccountNumber,
    string? Description
);