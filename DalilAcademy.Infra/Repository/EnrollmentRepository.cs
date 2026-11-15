using DalilAcademy.Core.Entities;
using DalilAcademy.Core.Exceptions;
using DalilAcademy.Core.Repository;
using DalilAcademy.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DalilAcademy.Infra.Repository;

public class EnrollmentRepository(AppDbContext db) : IEnrollmentRepository
{
    public async Task<List<Enrollment>> GetAllAsync()
    {
        return await db.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .ToListAsync();
    }

    public async Task<Enrollment?> GetByIdAsync(int id)
    {
        return await db.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Enrollment> CreateAsync(Enrollment enrollment)
    {
        try
        {
            db.Enrollments.Add(enrollment);
            await db.SaveChangesAsync();


            return await GetByIdAsync(enrollment.Id)
                   ?? enrollment;
        }
        catch (DbUpdateException ex) when (
            ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            throw new ConflictException("This student is already enrolled in this course.");
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var enrollment = await db.Enrollments.FindAsync(id);
        if (enrollment is null) return false;

        db.Enrollments.Remove(enrollment);
        await db.SaveChangesAsync();
        return true;
    }
}