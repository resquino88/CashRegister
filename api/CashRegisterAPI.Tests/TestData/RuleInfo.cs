using CashRegisterAPI.DTO;

namespace CashRegisterAPI.Tests.TestData;

/// <summary>
/// Factory for BasicRuleInfoDTO. Uses currencyMultiplier=100 (USD default).
/// </summary>
public static class RuleInfo
{
    public static BasicRuleInfoDTO Create(long amountOwed, long amountPaid, params DenominationDTO[] denominations)
        => new(amountOwed, amountPaid, 100, denominations);
}
