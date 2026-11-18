namespace DalilAcademy.Core.DTOs;

public record PaymentOrderResponseDto(
    int PaymentId,
    string OrderId,
    decimal Amount,
    string Currency,
    string ApprovalUrl);

public record PaymentResponseDto(
    int Id,
    int EnrollmentId,
    decimal Amount,
    string Currency,
    string PayPalOrderId,
    string Status,
    DateTime CreatedAt,
    DateTime? PaidAt
);