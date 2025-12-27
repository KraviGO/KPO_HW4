namespace SharedKernel.ValueObjects;

public sealed record AccountNumber
{
    public string Value { get; }

    public AccountNumber(string value)
    {
        Value = value;
    }

    public static AccountNumber Generate()
    {
        var raw = Guid.NewGuid().ToString("N")[..10].ToUpperInvariant();
        return new AccountNumber($"ACC-{raw}");
    }
    
    public override string ToString() => Value;
}