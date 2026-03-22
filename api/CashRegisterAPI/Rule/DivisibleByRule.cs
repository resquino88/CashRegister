using CashRegisterAPI.DTO;
using CashRegisterAPI.Utility;
using Microsoft.Extensions.Options;

namespace CashRegisterAPI.Rule;

public class DivisibleByRule(IOptions<RuleEngineInfo> options) : IRule<string, BasicRuleInfoDTO>
{
    private readonly long _divisor = options.Value.DivisibleBy.Divisor;

    public string Name() => "divisibleBy";

    public string Apply(BasicRuleInfoDTO info)
    {
        var change = info.AmountPaid - info.AmountOwed;

        if (change == 0)
        {
            return "No change";
        }

        var sortedDenominations = info.Denominations.Where(d => d.Value <= change).OrderByDescending(d => d.Value).ToArray();

        var parts = new List<string>();
        var currChange = change;
        int currDenominationValue = 0;
        long denominationCount = 0;
        Random rnd = new();
        DenominationDTO currentDenominationDto;

        for (int i = 0; i < sortedDenominations.Length; i++)
        {
            currentDenominationDto = sortedDenominations[i];
            currDenominationValue = currentDenominationDto.Value;
            denominationCount = 0;

            if (i + 1 == sortedDenominations.Length)
            {
                denominationCount = currChange / currDenominationValue;
                currChange = 0;
            }
            else if (currChange >= currDenominationValue)
            {
                denominationCount = rnd.NextInt64(0, (currChange / currDenominationValue) + 1);
                currChange -= denominationCount * currDenominationValue;
            }

            if (denominationCount == 0)
            {
                continue;
            }

            string name = denominationCount > 1
                ? (currentDenominationDto.PluralName ?? currentDenominationDto.Name + "s")
                : currentDenominationDto.Name;

            parts.Add($"{denominationCount} {name}");
        }

        return string.Join(", ", parts);
    }

    public bool IsApplicable(BasicRuleInfoDTO info)
    {
        return info.AmountOwed % _divisor == 0;
    }
}
