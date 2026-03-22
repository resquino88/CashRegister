namespace CashRegisterAPI.Domain;

public class Country
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    // In ISO 3166-1 alpha-3 format
    public string Abbrevation { get; private set; } = string.Empty;
    public int CurrencyMultipler { get; private set; }
    public List<CountryCurrency> CountryCurrencies { get; private set; } = [];

    public Country() { }

    public Country(int id, string name, string abbrevation, int currencyMultipler, List<CountryCurrency> countryCurrencies)
    {
        Id = id;
        Name = name;
        Abbrevation = abbrevation;
        CountryCurrencies = countryCurrencies;
        CurrencyMultipler = currencyMultipler;
    }

}
