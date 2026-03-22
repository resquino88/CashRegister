namespace CashRegisterAPI.Domain;

public class CountryCurrency
{
    public int Id { get; private set; }
    public int CountryId { get; private set; }
    public Country Country { get; private set; } = null!;
    public int CurrencyId { get; private set; }
    public Currency Currency { get; private set; } = null!;
    public bool IsPrimary { get; private set; } = false;

    public CountryCurrency() { }

    public CountryCurrency(int id, Country country, Currency currency, bool isPrimary = false)
    {
        Id = id;
        Country = country;
        Currency = currency;
        CountryId = country.Id;
        CurrencyId = currency.Id;
        IsPrimary = isPrimary;
    }
}
