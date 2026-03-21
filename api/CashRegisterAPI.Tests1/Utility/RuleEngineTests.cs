using CashRegisterAPI.DTO;
using CashRegisterAPI.Repository;
using Xunit;
using CashRegisterAPI.Rule;
using CashRegisterAPI.RuleOptions;
using CashRegisterAPI.Utility;
using Microsoft.Extensions.Options;
using Moq;
using DomainRule = CashRegisterAPI.Domain.Rule;

namespace CashRegisterAPI.Tests.Utility;

public class RuleEngineTests
{
    private static readonly BasicRuleInfoDTO AnyInfo = new(0, 9, 100, []);

    private static IOptions<RuleEngineInfo> DefaultOptions() =>
        Options.Create(new RuleEngineInfo
        {
            DefaultRuleName = "minChange",
            DivisibleBy = new DivisibleByRuleOptions { Divisor = 3 }
        });

    private static Mock<IRule<string, BasicRuleInfoDTO>> MockRule(string name, bool isApplicable, string applyResult)
    {
        var mock = new Mock<IRule<string, BasicRuleInfoDTO>>();
        mock.Setup(r => r.Name()).Returns(name);
        mock.Setup(r => r.IsApplicable(It.IsAny<BasicRuleInfoDTO>())).Returns(isApplicable);
        mock.Setup(r => r.Apply(It.IsAny<BasicRuleInfoDTO>())).Returns(applyResult);
        return mock;
    }

    private static Mock<IRuleRepository> MockRepo(params DomainRule[] activeRules)
    {
        var mock = new Mock<IRuleRepository>();
        mock.Setup(r => r.GetActiveRules()).ReturnsAsync([.. activeRules]);
        return mock;
    }

    [Fact]
    public async Task Start_AppliesNonDefaultRule_WhenApplicable()
    {
        var minChange = MockRule("minChange", isApplicable: true, applyResult: "1 dollar");
        var divisibleBy = MockRule("divisibleBy", isApplicable: true, applyResult: "3 pennies");

        var repo = MockRepo(
            new DomainRule(1, "minChange", 0, true),
            new DomainRule(2, "divisibleBy", 1, true));

        var engine = new RuleEngine(
            [minChange.Object, divisibleBy.Object],
            repo.Object,
            DefaultOptions());

        var result = await engine.Start(AnyInfo);

        Assert.Equal("3 pennies", result);
        divisibleBy.Verify(r => r.Apply(It.IsAny<BasicRuleInfoDTO>()), Times.Once);
        minChange.Verify(r => r.Apply(It.IsAny<BasicRuleInfoDTO>()), Times.Never);
    }

    [Fact]
    public async Task Start_FallsBackToDefaultRule_WhenNoNonDefaultRuleIsApplicable()
    {
        var minChange = MockRule("minChange", isApplicable: true, applyResult: "1 dollar");
        var divisibleBy = MockRule("divisibleBy", isApplicable: false, applyResult: "3 pennies");

        var repo = MockRepo(
            new DomainRule(1, "minChange", 0, true),
            new DomainRule(2, "divisibleBy", 1, true));

        var engine = new RuleEngine(
            [minChange.Object, divisibleBy.Object],
            repo.Object,
            DefaultOptions());

        var result = await engine.Start(AnyInfo);

        Assert.Equal("1 dollar", result);
        minChange.Verify(r => r.Apply(It.IsAny<BasicRuleInfoDTO>()), Times.Once);
        divisibleBy.Verify(r => r.Apply(It.IsAny<BasicRuleInfoDTO>()), Times.Never);
    }

    [Fact]
    public async Task Start_ThrowsInvalidOperationException_WhenDefaultRuleNotActive()
    {
        var minChange = MockRule("minChange", isApplicable: true, applyResult: "1 dollar");
        var divisibleBy = MockRule("divisibleBy", isApplicable: false, applyResult: "3 pennies");

        // Active rules do not include "minChange"
        var repo = MockRepo(new DomainRule(2, "divisibleBy", 1, true));

        var engine = new RuleEngine(
            [minChange.Object, divisibleBy.Object],
            repo.Object,
            DefaultOptions());

        await Assert.ThrowsAsync<InvalidOperationException>(() => engine.Start(AnyInfo));
    }

    [Fact]
    public async Task Start_IgnoresInactiveNonDefaultRules()
    {
        var minChange = MockRule("minChange", isApplicable: true, applyResult: "1 dollar");
        var divisibleBy = MockRule("divisibleBy", isApplicable: true, applyResult: "3 pennies");

        // divisibleBy is registered but NOT in active rules
        var repo = MockRepo(new DomainRule(1, "minChange", 0, true));

        var engine = new RuleEngine(
            [minChange.Object, divisibleBy.Object],
            repo.Object,
            DefaultOptions());

        var result = await engine.Start(AnyInfo);

        Assert.Equal("1 dollar", result);
        divisibleBy.Verify(r => r.Apply(It.IsAny<BasicRuleInfoDTO>()), Times.Never);
    }
}
