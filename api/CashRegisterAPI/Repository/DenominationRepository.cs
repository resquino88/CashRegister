using CashRegisterAPI.Data;
using CashRegisterAPI.Domain;
using Microsoft.EntityFrameworkCore;

namespace CashRegisterAPI.Repository;

public class DenominationRepository(CashRegisterDbContext context) : IDenominationRepository
{

    public async Task<Denomination> GetByName(string denominationName)
    {
        var denomination = await context.Denomination.SingleOrDefaultAsync(d => d.Name == denominationName) ?? throw new InvalidOperationException($"No denominations exist with a name of: ${denominationName}");
        return denomination;
    }

    public async Task<Denomination> GetById(int id)
    {
        var denomination = await context.Denomination.SingleOrDefaultAsync(d => d.Id == id) ?? throw new InvalidOperationException($"No denominations exist with an id of: ${id}");
        return denomination;
    }

    public async Task<List<Denomination>> GetAll()
    {
        return await context.Denomination.ToListAsync();
    }
}
