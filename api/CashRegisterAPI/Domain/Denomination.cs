namespace CashRegisterAPI.Domain;

public class Denomination
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? PluralName { get; set; } = null;
    public int Value { get; set; } = 0;
    public int CurrencyId { get; set; }
    public Currency Currency { get; set; } = null!;

    public Denomination() { }

    public Denomination(int id, string name, string? pluralName, int value, Currency currency)
    {
        Id = id;
        Name = name;
        PluralName = pluralName;
        Value = value;
        Currency = currency;
        CurrencyId = currency.Id;
    }
}
