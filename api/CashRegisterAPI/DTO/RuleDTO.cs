using CashRegisterAPI.Domain;

namespace CashRegisterAPI.DTO;

public record RuleDTO(int Id, string Name, int Priority, bool IsActive)
{
    public static RuleDTO FromEntity(Domain.Rule rule) => new(rule.Id, rule.Name, rule.Priority, rule.IsActive);
    public Domain.Rule ToEntity() => new(Id, Name, Priority, IsActive);
}
