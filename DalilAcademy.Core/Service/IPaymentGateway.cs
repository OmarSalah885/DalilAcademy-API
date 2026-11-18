namespace DalilAcademy.Core.Service;

public interface IPaymentGateway
{
    Task<PayPalOrderResult> CreateOrderAsync(decimal amountUsd);
    Task<bool> CaptureOrderAsync(string orderId);
}

public record PayPalOrderResult(string OrderId, string ApprovalUrl);