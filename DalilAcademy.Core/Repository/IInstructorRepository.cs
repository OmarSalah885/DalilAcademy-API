using DalilAcademy.Core.Entities;

namespace DalilAcademy.Core.Repository;

public interface IInstructorRepository
{
    Task<List<Instructor>> GetAllAsync();
    Task<Instructor?> GetByIdAsync(int id);
    Task<Instructor> CreateAsync(Instructor instructor);
    Task UpdateAsync(Instructor instructor);
    Task<bool> DeleteAsync(int id);
}