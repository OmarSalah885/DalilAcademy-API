using DalilAcademy.Core.Entities;

namespace DalilAcademy.Core.Repository;

public interface IEnrollmentRepository
{
    Task<List<Enrollment>> GetAllAsync();
    Task<Enrollment?> GetByIdAsync(int id);
    Task<Enrollment> CreateAsync(Enrollment enrollment);
    Task UpdateAsync(Enrollment enrollment);
    Task<bool> DeleteAsync(int id);
    Task<List<Enrollment>> GetPaidByStudentIdAsync(int studentId);
    Task<List<Student>> GetPaidStudentsByCourseIdAsync(int courseId);
    Task<int> CountPaidAsync(int courseId);
}