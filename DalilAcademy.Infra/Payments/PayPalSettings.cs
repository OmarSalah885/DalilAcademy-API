namespace DalilAcademy.Infra.Payments;

public class PayPalSettings
{
    public string BaseUrl { get; set; } = "";
    public string ClientId { get; set; } = "";
    public string ClientSecret { get; set; } = "";
    public string ReturnUrl { get; set; } = "";
    public string CancelUrl { get; set; } = "";
    public decimal JodToUsdRate { get; set; }
}