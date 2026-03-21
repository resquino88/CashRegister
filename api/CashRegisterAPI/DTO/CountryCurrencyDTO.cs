using CashRegisterAPI.Domain;

namespace CashRegisterAPI.DTO;

public record CountryCurrencyDTO(int Id, int CountryId, int CurrencyId, bool IsPrimary)
{
    public static CountryCurrencyDTO FromEntity(CountryCurrency cc) => new(cc.Id, cc.CountryId, cc.CurrencyId, cc.IsPrimary);
}
