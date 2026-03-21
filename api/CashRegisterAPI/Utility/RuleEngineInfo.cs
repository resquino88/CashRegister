using CashRegisterAPI.Rule;
using CashRegisterAPI.RuleOptions;

namespace CashRegisterAPI.Utility;

public class RuleEngineInfo
{
    public string DefaultRuleName { get; set; } = string.Empty;
    public DivisibleByRuleOptions DivisibleBy { get; set; } = new();
}
