using CashRegisterAPI.Data;
using CashRegisterAPI.Domain;
using Microsoft.EntityFrameworkCore;

namespace CashRegisterAPI.Repository;

public class CurrencyRepository(CashRegisterDbContext context) : ICurrencyRepository
{
    public async Task<Currency> GetByName(string currencyName)
    {
        var currency = await context.Currency.Include(c => c.Denominations).SingleOrDefaultAsync(c => c.Name == currencyName) ?? throw new InvalidOperationException($"No currencies exist with a name of: ${currencyName}");
        return currency;
    }

    public async Task<Currency> GetById(int id)
    {
        var currency = await context.Currency.Include(c => c.Denominations).SingleOrDefaultAsync(c => c.Id == id) ?? throw new InvalidOperationException($"No currencies exist with an id of: ${id}");
        return currency;
    }

    public async Task<List<Currency>> GetAll()
    {
        return await context.Currency.Include(c => c.Denominations).ToListAsync();
    }
}
