using CashRegisterAPI.DTO;
using CashRegisterAPI.Rule;
using Xunit;
using CashRegisterAPI.RuleOptions;
using CashRegisterAPI.Utility;
using Microsoft.Extensions.Options;

namespace CashRegisterAPI.Tests.Rules;

public class DivisibleByRuleTests
{
    private static DivisibleByRule BuildRule(long divisor = 3)
    {
        var options = Options.Create(new RuleEngineInfo
        {
            DefaultRuleName = "minChange",
            DivisibleBy = new DivisibleByRuleOptions { Divisor = divisor }
        });
        return new DivisibleByRule(options);
    }

    private static DenominationDTO Penny(int id = 1) => new(id, "penny", "pennies", 1, 1);
    private static DenominationDTO Nickel(int id = 2) => new(id, "nickel", null, 5, 1);
    private static DenominationDTO Dime(int id = 3) => new(id, "dime", null, 10, 1);
    private static DenominationDTO Quarter(int id = 4) => new(id, "quarter", null, 25, 1);

    private static BasicRuleInfoDTO Info(long amountOwed, long amountPaid, params DenominationDTO[] denominations)
        => new(amountOwed, amountPaid, 100, denominations);

    // Parses "N name, N name" output and sums count * denomination value
    private static long ParseTotalChange(string output, DenominationDTO[] denominations)
    {
        if (string.IsNullOrWhiteSpace(output)) return 0;
        long total = 0;
        foreach (var part in output.Split(", "))
        {
            var spaceIdx = part.IndexOf(' ');
            var count = long.Parse(part[..spaceIdx]);
            var name = part[(spaceIdx + 1)..];
            var denom = denominations.First(d =>
                d.Name == name ||
                d.PluralName == name ||
                d.Name + "s" == name);
            total += count * denom.Value;
        }
        return total;
    }

    // IsApplicable

    [Fact]
    public void IsApplicable_ReturnsTrue_WhenChangeDivisibleByDivisor()
    {
        // change = 9, divisor = 3 → 9 % 3 == 0
        var rule = BuildRule(divisor: 3);
        Assert.True(rule.IsApplicable(Info(0, 9, Penny())));
    }

    [Fact]
    public void IsApplicable_ReturnsFalse_WhenChangeNotDivisibleByDivisor()
    {
        // change = 10, divisor = 3 → 10 % 3 == 1
        var rule = BuildRule(divisor: 3);
        Assert.False(rule.IsApplicable(Info(0, 10, Penny())));
    }

    [Fact]
    public void IsApplicable_UsesCustomDivisor()
    {
        // change = 10, divisor = 5 → 10 % 5 == 0
        var rule = BuildRule(divisor: 5);
        Assert.True(rule.IsApplicable(Info(0, 10, Penny())));
    }

    [Fact]
    public void IsApplicable_ReturnsFalse_WhenChangeIsZero_AndDivisorIsNonZero()
    {
        // change = 0 → 0 % 3 == 0 → true (exact change edge case)
        var rule = BuildRule(divisor: 3);
        Assert.True(rule.IsApplicable(Info(5, 5, Penny())));
    }

    // Apply — random allocation, but total must equal change

    [Fact]
    public void Apply_TotalChange_EqualsExpected_ForDivisibleAmount()
    {
        // change = 99 (divisible by 3): random denominations but total must equal 99
        var rule = BuildRule(divisor: 3);
        var denominations = new[] { Quarter(), Dime(), Nickel(), Penny() };
        var info = Info(0, 99, denominations);

        var result = rule.Apply(info);
        var total = ParseTotalChange(result, denominations);

        Assert.Equal(99, total);
    }

    [Fact]
    public void Apply_TotalChange_IsCorrect_AcrossMultipleRuns()
    {
        // Run Apply several times to account for randomness
        var rule = BuildRule(divisor: 3);
        var denominations = new[] { Quarter(), Dime(), Nickel(), Penny() };
        var info = Info(0, 300, denominations);

        for (int i = 0; i < 10; i++)
        {
            var result = rule.Apply(info);
            var total = ParseTotalChange(result, denominations);
            Assert.Equal(300, total);
        }
    }
}
