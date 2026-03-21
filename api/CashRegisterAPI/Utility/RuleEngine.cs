using CashRegisterAPI.DTO;
using CashRegisterAPI.Repository;
using CashRegisterAPI.Rule;
using Microsoft.Extensions.Options;

namespace CashRegisterAPI.Utility;

public class RuleEngine(IEnumerable<IRule<string, BasicRuleInfoDTO>> rules, IRuleRepository ruleRepository, IOptions<RuleEngineInfo> options) : IRuleEngine
{
    private readonly string _defaultRuleName = options.Value.DefaultRuleName;

    public async Task<string> Start(BasicRuleInfoDTO info)
    {
        var foundActiveRules = await ruleRepository.GetActiveRules();

        var defaultRule = rules.SingleOrDefault(r => r.Name() == _defaultRuleName && foundActiveRules.Any(far => far.Name == r.Name()));
        if (defaultRule == null)
        {
            throw new InvalidOperationException($"No active default rule found with name '{_defaultRuleName}'. Ensure the rule is seeded and marked active in the database.");
        }

        var filteredRules = rules.Where(r => r.Name() != _defaultRuleName && foundActiveRules.Any(far => far.Name == r.Name())).ToArray();

        foreach (var rule in filteredRules)
        {
            if (rule.IsApplicable(info))
                return rule.Apply(info);
        }

        return defaultRule.Apply(info);
    }
}
