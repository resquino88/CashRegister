using CashRegisterAPI.DTO;

namespace CashRegisterAPI.Utility;

public interface IRuleEngine
{
    public Task<string> Start(BasicRuleInfoDTO value);
}
