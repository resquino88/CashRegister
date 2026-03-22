using CashRegisterAPI.Rule;
using CashRegisterAPI.Tests.TestData;

namespace CashRegisterAPI.Tests.Rules;

[TestFixture]
public class DivisibleByRuleTests
{
    private static DivisibleByRule BuildRule(long divisor = 3) =>
        new(TestOptions.WithDivisor(divisor));

    // IsApplicable

    [Test]
    public void IsApplicable_ReturnsTrue_WhenOwedDivisibleByDivisor()
    {
        // owed=9, 9 % 3 == 0
        Assert.That(BuildRule(3).IsApplicable(RuleInfo.Create(9, 18, Denominations.Penny)), Is.True);
    }

    [Test]
    public void IsApplicable_ReturnsFalse_WhenOwedNotDivisibleByDivisor()
    {
        // owed=10, 10 % 3 == 1
        Assert.That(BuildRule(3).IsApplicable(RuleInfo.Create(10, 20, Denominations.Penny)), Is.False);
    }

    [Test]
    public void IsApplicable_RespectsCustomDivisor()
    {
        // owed=10, divisor=5 → 10 % 5 == 0
        Assert.That(BuildRule(5).IsApplicable(RuleInfo.Create(10, 20, Denominations.Penny)), Is.True);
    }

    // Apply — zero change

    [Test]
    public void Apply_ReturnsNoChange_WhenChangeIsZero()
    {
        var result = BuildRule(3).Apply(RuleInfo.Create(5, 5, Denominations.Penny));
        Assert.That(result, Is.EqualTo("No change"));
    }

    // Apply — denomination filter

    [Test]
    public void Apply_IncludesDenominationsEqualToChange()
    {
        // change=10, dime(value=10): 10 <= 10 → included, must consume all change
        var result = BuildRule(5).Apply(RuleInfo.Create(0, 10, Denominations.Dime));
        Assert.That(OutputParser.ParseTotal(result, Denominations.AllCoins), Is.EqualTo(10));
    }

    // Apply — total must equal change regardless of random allocation

    [Test]
    public void Apply_OutputTotal_EqualsChange()
    {
        var result = BuildRule(3).Apply(RuleInfo.Create(0, 99, Denominations.AllCoins));
        Assert.That(OutputParser.ParseTotal(result, Denominations.AllCoins), Is.EqualTo(99));
    }

    [Test]
    public void Apply_OutputTotal_IsCorrect_AcrossMultipleRuns()
    {
        // Run 20 times to exercise randomness
        var rule = BuildRule(3);
        var info = RuleInfo.Create(0, 300, Denominations.AllCoins);

        for (int i = 0; i < 20; i++)
        {
            Assert.That(OutputParser.ParseTotal(rule.Apply(info), Denominations.AllCoins), Is.EqualTo(300));
        }
    }

    [Test]
    public void Apply_OutputTotal_IsCorrect_WithAmountOwedOffset()
    {
        // change = 300 - 99 = 201
        var result = BuildRule(3).Apply(RuleInfo.Create(99, 300, Denominations.AllCoins));
        Assert.That(OutputParser.ParseTotal(result, Denominations.AllCoins), Is.EqualTo(201));
    }
}
