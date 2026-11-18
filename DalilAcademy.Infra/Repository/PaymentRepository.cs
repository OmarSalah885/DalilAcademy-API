using DalilAcademy.Core.Entities;
using DalilAcademy.Core.Enums;
using DalilAcademy.Core.Repository;
using DalilAcademy.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace DalilAcademy.Infra.Repository;

public class PaymentRepository(AppDbContext db) : IPaymentRepository
{
    public async Task<Payment?> GetByIdAsync(int id)
    {
        return await db.Payments.FindAsync(id);
    }

    public async Task<List<Payment>> GetByEnrollmentIdAsync(int enrollmentId)
    {
        return await db.Payments
            .Where(p => p.EnrollmentId == enrollmentId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<Payment?> GetLatestCreatedByEnrollmentIdAsync(int enrollmentId)
    {
        return await db.Payments
            .Where(p => p.EnrollmentId == enrollmentId && p.Status == PaymentStatus.Created)
            .OrderByDescending(p => p.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(Payment payment)
    {
        db.Payments.Add(payment);
        await db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Payment payment)
    {
        db.Payments.Update(payment);
        await db.SaveChangesAsync();
    }
}