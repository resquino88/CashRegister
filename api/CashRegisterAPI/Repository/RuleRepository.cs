using CashRegisterAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace CashRegisterAPI.Repository;

public class RuleRepository(CashRegisterDbContext context) : IRuleRepository
{
    public async Task<Domain.Rule> GetByName(string ruleName)
    {
        var rule = await context.Rule.SingleOrDefaultAsync(r => r.Name == ruleName) ?? throw new InvalidOperationException($"No rules exist with a name of: ${ruleName}");
        return rule;
    }

    public async Task<Domain.Rule> GetById(int id)
    {
        var rule = await context.Rule.SingleOrDefaultAsync(c => c.Id == id) ?? throw new InvalidOperationException($"No rules exist with an id of: ${id}");
        return rule;
    }

    public async Task<List<Domain.Rule>> GetAll()
    {
        return await context.Rule.ToListAsync();
    }

    public async Task<List<Domain.Rule>> GetActiveRules()
    {
        return await context.Rule.Where(r => r.IsActive).ToListAsync();
    }
}
