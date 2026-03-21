using CashRegisterAPI.DTO;
using CashRegisterAPI.Rule;
using Xunit;

namespace CashRegisterAPI.Tests.Rules;

public class MinChangeRuleTests
{
    private readonly MinChangeRule _rule = new();

    // Helpers
    private static DenominationDTO Penny(int id = 1) => new(id, "penny", "pennies", 1, 1);
    private static DenominationDTO Nickel(int id = 2) => new(id, "nickel", null, 5, 1);
    private static DenominationDTO Dime(int id = 3) => new(id, "dime", null, 10, 1);
    private static DenominationDTO Quarter(int id = 4) => new(id, "quarter", null, 25, 1);

    private static BasicRuleInfoDTO Info(long amountOwed, long amountPaid, params DenominationDTO[] denominations)
        => new(amountOwed, amountPaid, 100, denominations);

    // IsApplicable

    [Fact]
    public void IsApplicable_AlwaysReturnsTrue()
    {
        var info = Info(0, 5, Penny());
        Assert.True(_rule.IsApplicable(info));
    }

    // Apply — greedy minimum denominations

    [Fact]
    public void Apply_ReturnsPluralName_WhenCountGreaterThanOne()
    {
        // change = 3, penny(1) < 3 → count=3, uses PluralName "pennies"
        var result = _rule.Apply(Info(0, 3, Penny()));
        Assert.Equal("3 pennies", result);
    }

    [Fact]
    public void Apply_ReturnsSingularName_WhenCountIsOne()
    {
        // change = 11: dime(10) < 11 → count=1; penny(1) < 11 → count=1
        var result = _rule.Apply(Info(0, 11, Dime(), Penny()));
        Assert.Equal("1 dime, 1 penny", result);
    }

    [Fact]
    public void Apply_AppendsSuffix_WhenCountGreaterThanOne_AndNoPluralName()
    {
        // change = 56: quarter(25) < 56 → count=2 (50), penny(1) < 56 → count=6
        // quarter has no pluralName → "quarters" (suffix 's')
        var result = _rule.Apply(Info(0, 56, Quarter(), Penny()));
        Assert.Equal("2 quarters, 6 pennies", result);
    }

    [Fact]
    public void Apply_UsesGreedyAlgorithm_PrefersLargerDenominations()
    {
        // change = 36: quarter(25) < 36 → count=1 (25); dime(10) < 36 → count=1 (10); penny(1) < 36 → count=1
        var result = _rule.Apply(Info(0, 36, Quarter(), Dime(), Penny()));
        Assert.Equal("1 quarter, 1 dime, 1 penny", result);
    }

    [Fact]
    public void Apply_ExcludesDenominationsEqualToOrGreaterThanChange()
    {
        // change = 10: dime(10) is NOT < 10, so only penny(1) < 10 → 10 pennies
        var result = _rule.Apply(Info(0, 10, Dime(), Penny()));
        Assert.Equal("10 pennies", result);
    }

    [Fact]
    public void Apply_ReturnsEmptyString_WhenNoDenominationsBelowChange()
    {
        // change = 1: penny(1) is NOT < 1 → no denominations in range → empty string
        var result = _rule.Apply(Info(0, 1, Penny()));
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Apply_HandlesAmountOwedAndPaid_WithOffset()
    {
        // 3.00 - 2.12 = 88 cents (as integer units with multiplier 100)
        // Denominations where value < 88: quarter(25), dime(10), nickel(5), penny(1)
        // 3 quarters (75), 1 dime (10), then nickel: 3 < 5 → count=0, then penny: count=3
        var result = _rule.Apply(Info(212, 300, Quarter(), Dime(), Nickel(), Penny()));
        Assert.Equal("3 quarters, 1 dime, 0 nickel, 3 pennies", result);
    }
}
