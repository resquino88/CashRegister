using CashRegisterAPI.Rule;
using CashRegisterAPI.Tests.TestData;

namespace CashRegisterAPI.Tests.Rules;

[TestFixture]
public class MinChangeRuleTests
{
    private MinChangeRule _rule = null!;

    [SetUp]
    public void SetUp() => _rule = new MinChangeRule();

    // IsApplicable

    [Test]
    public void IsApplicable_AlwaysReturnsTrue()
    {
        Assert.That(_rule.IsApplicable(RuleInfo.Create(0, 5, Denominations.Penny)), Is.True);
    }

    // Apply — name formatting

    [Test]
    public void Apply_UsesPluralName_WhenCountGreaterThanOne_AndPluralNameSet()
    {
        // change=3, penny(value=1) < 3 → count=3, pluralName="pennies"
        var result = _rule.Apply(RuleInfo.Create(0, 3, Denominations.Penny));
        Assert.That(result, Is.EqualTo("3 pennies"));
    }

    [Test]
    public void Apply_AppendsSuffix_WhenCountGreaterThanOne_AndNoPluralName()
    {
        // change=56: quarter(25) < 56 → count=2; penny(1) → count=6
        // quarter has no pluralName → appended 's' → "quarters"
        var result = _rule.Apply(RuleInfo.Create(0, 56, Denominations.Quarter, Denominations.Penny));
        Assert.That(result, Is.EqualTo("2 quarters, 6 pennies"));
    }

    [Test]
    public void Apply_UsesSingularName_WhenCountIsOne()
    {
        // change=11: dime(10) < 11 → count=1; penny(1) < 11 → count=1
        var result = _rule.Apply(RuleInfo.Create(0, 11, Denominations.Dime, Denominations.Penny));
        Assert.That(result, Is.EqualTo("1 dime, 1 penny"));
    }

    // Apply — greedy algorithm

    [Test]
    public void Apply_PrefersLargerDenominations()
    {
        // change=36: quarter(25)→count=1, dime(10)→count=1, penny(1)→count=1
        var result = _rule.Apply(RuleInfo.Create(0, 36, Denominations.Quarter, Denominations.Dime, Denominations.Penny));
        Assert.That(result, Is.EqualTo("1 quarter, 1 dime, 1 penny"));
    }

    [Test]
    public void Apply_IncludesZeroCountDenominations_WhenNotConsumed()
    {
        // change=88 (300-212): quarter(25)<88→count=3(75), dime(10)<88→count=1(10),
        // nickel(5)<88 but 3 remaining < 5 → count=0, penny(1)→count=3
        var result = _rule.Apply(RuleInfo.Create(212, 300, Denominations.Quarter, Denominations.Dime, Denominations.Nickel, Denominations.Penny));
        Assert.That(result, Is.EqualTo("3 quarters, 1 dime, 0 nickel, 3 pennies"));
    }

    // Apply — denomination filter

    [Test]
    public void Apply_ExcludesDenominationsEqualToChange()
    {
        // change=10, dime(value=10): 10 < 10 is false → excluded, only penny used
        var result = _rule.Apply(RuleInfo.Create(0, 10, Denominations.Dime, Denominations.Penny));
        Assert.That(result, Is.EqualTo("10 pennies"));
    }

    [Test]
    public void Apply_ReturnsEmptyString_WhenAllDenominationsExcluded()
    {
        // change=1, penny(value=1): 1 < 1 is false → no denominations included
        var result = _rule.Apply(RuleInfo.Create(0, 1, Denominations.Penny));
        Assert.That(result, Is.EqualTo(string.Empty));
    }
}
