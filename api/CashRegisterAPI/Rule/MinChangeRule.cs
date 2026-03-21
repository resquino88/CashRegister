using CashRegisterAPI.DTO;
using System.Text;

namespace CashRegisterAPI.Rule;

public class MinChangeRule : IRule<string, BasicRuleInfoDTO>
{
    public string Name() => "minChange";

    public string Apply(BasicRuleInfoDTO info)
    {
        var change = info.AmountPaid - info.AmountOwed;

        var sortedDenominations = info.Denominations.Where(d => d.Value < change).OrderByDescending(d => d.Value).ToArray();

        StringBuilder sb = new("");
        var currChange = change;
        int currDenominationValue = 0;
        int denominationCount = 0;
        DenominationDTO currDenomination;

        for (int i = 0; i < sortedDenominations.Length; i++)
        {
            currDenomination = sortedDenominations[i];
            currDenominationValue = currDenomination.Value;
            denominationCount = 0;

            while (currChange >= currDenominationValue)
            {
                currChange -= currDenominationValue;
                denominationCount += 1;
            }

            sb.Append(denominationCount);
            sb.Append(' ');

            if (denominationCount > 1)
            {
                if (currDenomination.PluralName != null)
                {
                    sb.Append(currDenomination.PluralName);
                }
                else
                {
                    sb.Append(currDenomination.Name);
                    sb.Append('s');
                }
            }
            else
            {
                sb.Append(currDenomination.Name);
            }

            if (i + 1 != sortedDenominations.Length)
            {
                sb.Append(", ");
            }
        }

        return sb.ToString();
    }

    public bool IsApplicable(BasicRuleInfoDTO info) => true;
}

