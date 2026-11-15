using DalilAcademy.Core.DTOs;

public interface ICurrencyService
{
    Task<ConvertPriceResponseDto> ConvertAsync(ConvertPriceRequestDto request);
}