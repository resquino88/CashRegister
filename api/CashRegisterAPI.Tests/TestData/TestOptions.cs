using CashRegisterAPI.RuleOptions;
using CashRegisterAPI.Utility;
using Microsoft.Extensions.Options;

namespace CashRegisterAPI.Tests.TestData;

/// <summary>
/// Shared IOptions&lt;RuleEngineInfo&gt; instances for tests.
/// </summary>
public static class TestOptions
{
    /// <summary>Default options: defaultRule="minChange", divisor=3.</summary>
    public static readonly IOptions<RuleEngineInfo> Default = Options.Create(new RuleEngineInfo
    {
        DefaultRuleName = "minChange",
        DivisibleBy = new DivisibleByRuleOptions { Divisor = 3 }
    });

    /// <summary>Creates options with a custom divisor, keeping all other defaults.</summary>
    public static IOptions<RuleEngineInfo> WithDivisor(long divisor) =>
        Options.Create(new RuleEngineInfo
        {
            DefaultRuleName = "minChange",
            DivisibleBy = new DivisibleByRuleOptions { Divisor = divisor }
        });
}
