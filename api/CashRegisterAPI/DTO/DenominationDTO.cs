using CashRegisterAPI.Domain;

namespace CashRegisterAPI.DTO;

public record DenominationDTO(int Id, string Name, string? PluralName, int Value, int CurrencyId)
{
    public static DenominationDTO FromEntity(Denomination denomination) => new (denomination.Id, denomination.Name, denomination.PluralName, denomination.Value, denomination.CurrencyId);
}
