namespace CashRegisterAPI.DTO;

public record BasicRuleInfoDTO(long AmountOwed, long AmountPaid, int CurrencyMultipler, DenominationDTO[] Denominations)
{
}
