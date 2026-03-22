using CashRegisterAPI.DTO;

namespace CashRegisterAPI.Rule;

public class MinChangeRule : IRule<string, BasicRuleInfoDTO>
{
    public string Name() => "minChange";

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

            if (denominationCount == 0)
            {
                continue;
            }

            string name = denominationCount > 1
                ? (currDenomination.PluralName ?? currDenomination.Name + "s")
                : currDenomination.Name;

            parts.Add($"{denominationCount} {name}");
        }

        return string.Join(", ", parts);
    }

    public bool IsApplicable(BasicRuleInfoDTO info) => true;
}

