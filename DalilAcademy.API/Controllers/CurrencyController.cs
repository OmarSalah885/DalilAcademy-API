
using DalilAcademy.Core.DTOs;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class CurrencyController(ICurrencyService currencyService) : ControllerBase
{
    [HttpPost("convert")]
    public async Task<ActionResult<ConvertPriceResponseDto>> Convert(
        ConvertPriceRequestDto request)
    {
        return Ok(await currencyService.ConvertAsync(request));
    }
}