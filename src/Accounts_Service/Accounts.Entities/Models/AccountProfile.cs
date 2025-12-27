namespace Accounts.Entities.Models;

public class AccountProfile
{
    public string FirstName { get; private set; } = null!;
    public string LastName  { get; private set; } = null!;
    public string? Description { get; private set; }

    private AccountProfile() {}

    public AccountProfile(string firstName, string lastName, string? description)
    {
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Description = description?.Trim();
    }

    public void UpdateDescription(string? description)
    {
        Description = description?.Trim();
    }
}