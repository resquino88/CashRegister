using CashRegisterAPI.Data;
using CashRegisterAPI.Domain;
using Microsoft.EntityFrameworkCore;

namespace CashRegisterAPI.Repository;

public class CountryRepository(CashRegisterDbContext context) : ICountryRepository
{
    public async Task<Country> GetByName(string countryName)
    {
        var country = await context.Country.Include(c => c.CountryCurrencies).ThenInclude(cc => cc.Currency).ThenInclude(curr => curr.Denominations).SingleOrDefaultAsync(c => c.Name == countryName);

        if (country == null)
        {
            throw new InvalidOperationException($"No countries exist with a name of: ${countryName}");
        }

        return country;
    }

    public async Task<Country> GetById(int id)
    {
        var country = await context.Country.Include(c => c.CountryCurrencies).ThenInclude(cc => cc.Currency).ThenInclude(curr => curr.Denominations).SingleOrDefaultAsync(c => c.Id == id);

        if (country == null)
        {
            throw new InvalidOperationException($"No countries exist with an id of: ${id}");
        }

        return country;
    }

    public async Task<List<Country>> GetAll()
    {
        return await context.Country.Include(c => c.CountryCurrencies).ThenInclude(cc => cc.Currency).ThenInclude(curr => curr.Denominations).ToListAsync();
    }
}
