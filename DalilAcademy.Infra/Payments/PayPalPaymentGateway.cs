using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using DalilAcademy.Core.Service;
using Microsoft.Extensions.Options;

namespace DalilAcademy.Infra.Payments;

public class PayPalPaymentGateway(HttpClient http, IOptions<PayPalSettings> options) : IPaymentGateway
{
    private readonly PayPalSettings _settings = options.Value;

    public async Task<PayPalOrderResult> CreateOrderAsync(decimal amountUsd)
    {
        var token = await GetAccessTokenAsync();

        // PayPal wants the money as a string with a dot and exactly 2 decimals: "14.10"
        var value = amountUsd.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);

        var body = new
        {
            intent = "CAPTURE",
            purchase_units = new[]
            {
                new
                {
                    amount = new
                    {
                        currency_code = "USD",
                        value
                    }
                }
            },
            application_context = new
            {
                return_url = _settings.ReturnUrl,
                cancel_url = _settings.CancelUrl,
                user_action = "PAY_NOW"
            }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "v2/checkout/orders")
        {
            Content = JsonContent.Create(body)
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await http.SendAsync(request);
        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"PayPal create order failed: {json}");

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var orderId = root.GetProperty("id").GetString()
            ?? throw new HttpRequestException("PayPal did not return an order id.");

        // links is an array of { href, rel, method }. The buyer URL is rel == "approve".
        var approvalUrl = root.GetProperty("links")
            .EnumerateArray()
            .First(link => link.GetProperty("rel").GetString() == "approve")
            .GetProperty("href")
            .GetString()
            ?? throw new HttpRequestException("PayPal did not return an approval link.");

        return new PayPalOrderResult(orderId, approvalUrl);
    }

    public async Task<bool> CaptureOrderAsync(string orderId)
    {
        var token = await GetAccessTokenAsync();

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"v2/checkout/orders/{orderId}/capture");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = new StringContent("{}", Encoding.UTF8, "application/json");

        var response = await http.SendAsync(request);
        var json = await response.Content.ReadAsStringAsync();

        // A PayPal business refusal (not approved, already captured, expired) is just false.
        if (!response.IsSuccessStatusCode)
            return false;

        using var doc = JsonDocument.Parse(json);
        var status = doc.RootElement.GetProperty("status").GetString();
        return status == "COMPLETED";
    }

    private async Task<string> GetAccessTokenAsync()
    {
        // Basic auth is base64("clientId:clientSecret"). This is the client-credentials grant.
        var raw = $"{_settings.ClientId}:{_settings.ClientSecret}";
        var basic = Convert.ToBase64String(Encoding.UTF8.GetBytes(raw));

        var request = new HttpRequestMessage(HttpMethod.Post, "v1/oauth2/token");
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", basic);
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials"
        });

        var response = await http.SendAsync(request);
        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"PayPal token request failed: {json}");

        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("access_token").GetString()
            ?? throw new HttpRequestException("PayPal did not return an access token.");
    }
}