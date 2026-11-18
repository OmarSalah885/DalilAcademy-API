using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Entities;
using DalilAcademy.Core.Enums;
using DalilAcademy.Core.Exceptions;
using DalilAcademy.Core.Mappings;
using DalilAcademy.Core.Repository;
using DalilAcademy.Core.Service;
using DalilAcademy.Infra.Payments;
using Microsoft.Extensions.Options;

namespace DalilAcademy.Infra.Service;

public class PaymentService(
    IEnrollmentRepository enrollmentRepository,
    IPaymentRepository paymentRepository,
    IPaymentGateway paymentGateway,
    IOptions<PayPalSettings> paypalOptions) : IPaymentService
{
    private readonly decimal _jodToUsdRate = paypalOptions.Value.JodToUsdRate;

    public async Task<PaymentOrderResponseDto> PayAsync(int enrollmentId)
    {
        var enrollment = await enrollmentRepository.GetByIdAsync(enrollmentId)
            ?? throw new NotFoundException($"Enrollment with id {enrollmentId} was not found.");

        if (enrollment.Status == EnrollmentStatus.Paid)
            throw new ConflictException("This enrollment is already paid.");

        await EnsureSeatAvailableAsync(enrollment.Course);

        var amountUsd = Math.Round(enrollment.Course.Price * _jodToUsdRate, 2);

        PayPalOrderResult order;
        try
        {
            order = await paymentGateway.CreateOrderAsync(amountUsd);
        }
        catch (HttpRequestException)
        {
            throw;
        }

        var payment = new Payment
        {
            EnrollmentId = enrollment.Id,
            Amount = amountUsd,
            Currency = "USD",
            PayPalOrderId = order.OrderId,
            Status = PaymentStatus.Created,
            CreatedAt = DateTime.UtcNow
        };

        await paymentRepository.AddAsync(payment);

        return new PaymentOrderResponseDto(
            payment.Id,
            order.OrderId,
            amountUsd,
            "USD",
            order.ApprovalUrl);
    }

    public async Task<EnrollmentResponseDto> CaptureAsync(int enrollmentId)
    {
        var enrollment = await enrollmentRepository.GetByIdAsync(enrollmentId)
            ?? throw new NotFoundException($"Enrollment with id {enrollmentId} was not found.");

        if (enrollment.Status == EnrollmentStatus.Paid)
            throw new ConflictException("This enrollment is already paid.");

        var payment = await paymentRepository.GetLatestCreatedByEnrollmentIdAsync(enrollmentId)
                        ?? throw new PaymentFailedException("Call /pay first.");

        await EnsureSeatAvailableAsync(enrollment.Course);

        bool completed;
        try
        {
            completed = await paymentGateway.CaptureOrderAsync(payment.PayPalOrderId);
        }
        catch (HttpRequestException)
        {
            throw;
        }

        if (!completed)
        {
            payment.Status = PaymentStatus.Failed;
            await paymentRepository.UpdateAsync(payment);
            throw new PaymentFailedException(
                "Payment was not completed. The order may be unapproved, expired, or already captured. Call /pay again.");
        }

        payment.Status = PaymentStatus.Completed;
        payment.PaidAt = DateTime.UtcNow;
        await paymentRepository.UpdateAsync(payment);

        enrollment.Status = EnrollmentStatus.Paid;

        await enrollmentRepository.UpdateAsync(enrollment);

        var updated = await enrollmentRepository.GetByIdAsync(enrollmentId)
            ?? enrollment;

        return updated.ToDto();
    }

    public async Task<List<PaymentResponseDto>> GetPaymentsByEnrollmentIdAsync(int enrollmentId)
    {
        var enrollment = await enrollmentRepository.GetByIdAsync(enrollmentId)
            ?? throw new NotFoundException($"Enrollment with id {enrollmentId} was not found.");

        var payments = await paymentRepository.GetByEnrollmentIdAsync(enrollmentId);

        return payments.Select(p => p.ToDto()).ToList();
    }

    private async Task EnsureSeatAvailableAsync(Course course)
    {
        if (!course.MaxStudents.HasValue) return;   

        var paid = await enrollmentRepository.CountPaidAsync(course.Id);
        if (paid >= course.MaxStudents.Value)
            throw new ConflictException($"Course '{course.Title}' is full ({course.MaxStudents} students).");
    }
}