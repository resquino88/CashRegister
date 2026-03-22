using CashRegisterAPI.Domain;

namespace CashRegisterAPI.Data;

public class DataSeeder
{
    public static async Task SeedAsync(CashRegisterDbContext context)
    {
        // Ensure the database is created
        await context.Database.EnsureCreatedAsync();

        // Check if data already exists to prevent duplicates
        if (context.Denomination.Any() || context.Currency.Any() || context.Country.Any() || context.Rule.Any())
        {
            // Data already seeded
            return;
        }

        // Add USA, France, and Finland to Country table
        var usaCountry = new Country(0, "United States of America", "USA", 100, []);
        context.Country.Add(usaCountry);

        var franceCountry = new Country(0, "France", "FRA", 100, []);
        context.Country.Add(franceCountry);

        await context.SaveChangesAsync();

        // Add USD and EURO currencies to Currency table
        var usdCurrency = new Currency(0, "USD", '.', [], []);
        context.Currency.Add(usdCurrency);

        var euroCurrency = new Currency(0, "EURO", ',', [], []);
        context.Currency.Add(euroCurrency);

        await context.SaveChangesAsync();


        // Add each country's currency and whether or not its their default currency
        var usaCountryCurrency = new CountryCurrency(0, usaCountry, usdCurrency, true);
        context.CountryCurrency.Add(usaCountryCurrency);

        var franceCountryCurrency = new CountryCurrency(0, franceCountry, euroCurrency, true);
        context.CountryCurrency.Add(franceCountryCurrency);

        await context.SaveChangesAsync();


        // Add each currency's denomination info
        var denominations = new List<Denomination>
        {
            new(0, "one hundred dollar", null, 10000, usdCurrency),
            new(0, "fifty dollar", null, 5000, usdCurrency),
            new(0, "twenty dollar", null, 2000, usdCurrency),
            new(0, "ten dollar", null, 1000, usdCurrency),
            new(0, "five dollar", null, 500, usdCurrency),
            new(0, "one dollar", null, 100, usdCurrency),
            new(0, "quarter", null, 25, usdCurrency),
            new(0, "dime", null, 10, usdCurrency),
            new(0, "nickel", null, 5, usdCurrency),
            new(0, "penny", "pennies", 1, usdCurrency),
            new(0, "five hundred euro", null, 50000, euroCurrency),
            new(0, "two hundred euro", null, 20000, euroCurrency),
            new(0, "one hundred euro", null, 10000, euroCurrency),
            new(0, "fifty euro", null, 5000, euroCurrency),
            new(0, "twenty euro", null, 2000, euroCurrency),
            new(0, "ten euro", null, 1000, euroCurrency),
            new(0, "five euro", null, 500, euroCurrency),
            new(0, "two euro", null, 200, euroCurrency),
            new(0, "one euro", null, 100, euroCurrency),
            new(0, "fifty cent", null, 50, euroCurrency),
            new(0, "twenty cent", null, 20, euroCurrency),
            new(0, "ten cent", null, 10, euroCurrency),
            new(0, "five cent", null, 5, euroCurrency),
            new(0, "two cent", null, 2, euroCurrency),
            new(0, "one cent", null, 1, euroCurrency),
        };

        context.AddRange(denominations);

        await context.SaveChangesAsync();

        // Add role info into the db
        var rule = new List<Domain.Rule> {
            new (0, "minChange", 0, true),
            new (0, "divisibleBy", 1, true)
        };

        context.AddRange(rule);

        await context.SaveChangesAsync();
    }
}
