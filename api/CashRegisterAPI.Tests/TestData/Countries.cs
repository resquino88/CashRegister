using CashRegisterAPI.Domain;

namespace CashRegisterAPI.Tests.TestData;

/// <summary>
/// Factories for Country domain entities. Mirrors the seeded data in DataSeeder.
/// Uses the list-reference trick to satisfy the circular Currency ↔ Denomination dependency.
/// </summary>
public static class Countries
{
    /// <summary>
    /// USA with USD currency (decimal separator '.', multiplier 100).
    /// Denominations: one dollar (100), penny (1).
    /// </summary>
    public static Country BuildUsa(int countryId = 1, int currencyId = 1)
    {
        var denominations = new List<Denomination>();
        var currency = new Currency(currencyId, "USD", '.', denominations, []);
        denominations.Add(new Denomination(1, "one dollar", null, 100, currency));
        denominations.Add(new Denomination(2, "penny", "pennies", 1, currency));

        var countryCurrencies = new List<CountryCurrency>();
        var country = new Country(countryId, "United States of America", "USA", 100, countryCurrencies);
        countryCurrencies.Add(new CountryCurrency(1, country, currency, true));
        return country;
    }

    /// <summary>
    /// France with EURO currency (decimal separator ',', multiplier 100).
    /// Denominations: one euro (100), one cent (1).
    /// </summary>
    public static Country BuildFrance(int countryId = 2, int currencyId = 2)
    {
        var denominations = new List<Denomination>();
        var currency = new Currency(currencyId, "EURO", ',', denominations, []);
        denominations.Add(new Denomination(3, "one euro", null, 100, currency));
        denominations.Add(new Denomination(4, "one cent", null, 1, currency));

        var countryCurrencies = new List<CountryCurrency>();
        var country = new Country(countryId, "France", "FRA", 100, countryCurrencies);
        countryCurrencies.Add(new CountryCurrency(2, country, currency, true));
        return country;
    }
}
