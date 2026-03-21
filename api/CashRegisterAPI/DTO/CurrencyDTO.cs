using CashRegisterAPI.Domain;

namespace CashRegisterAPI.DTO;

public record CurrencyDTO(int Id, string Name, char CurrencySeparator, List<DenominationDTO> Denominations)
{
    public static CurrencyDTO FromEntity(Currency currency) => new(currency.Id, currency.Name, currency.CurrencySeparator, [.. currency.Denominations.Select(DenominationDTO.FromEntity)]);
}
