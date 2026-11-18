using DalilAcademy.Core.Enums;

namespace DalilAcademy.Core.Entities;

public class Payment
{
    public int Id { get; set; }

    public int EnrollmentId { get; set; }
    public Enrollment Enrollment { get; set; } = null!;

    public decimal Amount { get; set; }          // in USD, precision (10,2)

    public string Currency { get; set; } = "USD"; // always USD

    public string PayPalOrderId { get; set; } = null!;  // unique

    public PaymentStatus Status { get; set; } = PaymentStatus.Created;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? PaidAt { get; set; }         // set only when capture succeeds
}