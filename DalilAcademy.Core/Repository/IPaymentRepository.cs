using DalilAcademy.Core.Entities;

namespace DalilAcademy.Core.Repository;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(int id);
    Task<List<Payment>> GetByEnrollmentIdAsync(int enrollmentId);
    Task<Payment?> GetLatestCreatedByEnrollmentIdAsync(int enrollmentId);
    Task AddAsync(Payment payment);
    Task UpdateAsync(Payment payment);


}