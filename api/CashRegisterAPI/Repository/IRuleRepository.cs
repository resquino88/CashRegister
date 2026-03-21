namespace CashRegisterAPI.Repository;

public interface IRuleRepository : IRepository<Domain.Rule>
{
    public Task<List<Domain.Rule>> GetActiveRules();
}
