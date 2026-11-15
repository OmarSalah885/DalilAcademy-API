using DalilAcademy.Core.Entities;

namespace DalilAcademy.Core.Repository;

public interface IEnrollmentRepository
{
    Task<List<Enrollment>> GetAllAsync();
    Task<Enrollment?> GetByIdAsync(int id);
    Task<Enrollment> CreateAsync(Enrollment enrollment);
    Task<bool> DeleteAsync(int id);
}