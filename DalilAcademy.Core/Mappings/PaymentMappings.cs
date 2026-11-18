using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Entities;

namespace DalilAcademy.Core.Mappings;

public static class PaymentMappings
{
    public static PaymentResponseDto ToDto(this Payment payment)
    {
        return new PaymentResponseDto(
            payment.Id,
            payment.EnrollmentId,
            payment.Amount,
            payment.Currency,
            payment.PayPalOrderId,
            payment.Status.ToString(),
            payment.CreatedAt,
            payment.PaidAt
        );
    }
}