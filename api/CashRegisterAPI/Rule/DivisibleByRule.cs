using CashRegisterAPI.DTO;
using CashRegisterAPI.Utility;
using Microsoft.Extensions.Options;
using System.Text;

namespace CashRegisterAPI.Rule;

public class DivisibleByRule(IOptions<RuleEngineInfo> options) : IRule<string, BasicRuleInfoDTO>
{
    private readonly long _divisor = options.Value.DivisibleBy.Divisor;

    public string Name() => "divisibleBy";

    public string Apply(BasicRuleInfoDTO info)
    {
        var change = info.AmountPaid - info.AmountOwed;

        var sortedDenominations = info.Denominations.Where(d => d.Value < change).OrderByDescending(d => d.Value).ToArray();

        StringBuilder sb = new("");
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

            sb.Append(denominationCount);
            sb.Append(' ');

            if (denominationCount > 1)
            {
                if (currentDenominationDto.PluralName != null)
                {
                    sb.Append(currentDenominationDto.PluralName);
                }
                else
                {
                    sb.Append(currentDenominationDto.Name);
                    sb.Append('s');
                }
            }
            else
            {
                sb.Append(currentDenominationDto.Name);
            }

            if (i + 1 != sortedDenominations.Length)
            {
                sb.Append(", ");
            }
        }

        return sb.ToString();
    }

    public bool IsApplicable(BasicRuleInfoDTO info)
    {
        return (info.AmountPaid - info.AmountOwed) % _divisor == 0;
    }
}
