using DalilAcademy.Core.DTOs;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

public class CurrencyService(HttpClient http) : ICurrencyService
{
    private record PairResponse(
        [property: JsonPropertyName("conversion_rate")] decimal ConversionRate);

    public async Task<ConvertPriceResponseDto> ConvertAsync(ConvertPriceRequestDto r)
    {
        
        var data = await http.GetFromJsonAsync<PairResponse>(
            $"pair/{r.FromCurrency}/{r.ToCurrency}")
            ?? throw new HttpRequestException("Empty response from ExchangeRate-API.");

        var converted = Math.Round(r.Amount * data.ConversionRate, 2);

        return new ConvertPriceResponseDto(
            r.Amount,
            r.FromCurrency,
            converted,
            r.ToCurrency,
            data.ConversionRate);
    }
}