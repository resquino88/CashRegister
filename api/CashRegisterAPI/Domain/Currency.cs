namespace CashRegisterAPI.Domain;

public class Currency
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public char CurrencySeparator { get; private set; }
    public List<Denomination> Denominations { get; private set; } = [];
    public List<CountryCurrency> CountryCurrencies { get; private set; } = [];

    public Currency() { }

    public Currency(int id, string name, char currencySeparator, List<Denomination> denominations, List<CountryCurrency> countryCurrencies)
    {
        Id = id;
        Name = name;
        CurrencySeparator = currencySeparator;
        Denominations = denominations;
        CountryCurrencies = countryCurrencies;
    }
}
