using DalilAcademy.Core.Entities;
using DalilAcademy.Core.Exceptions;
using DalilAcademy.Core.Repository;
using DalilAcademy.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DalilAcademy.Infra.Repository;

public class InstructorRepository(AppDbContext db) : IInstructorRepository
{
    public async Task<List<Instructor>> GetAllAsync()
        => await db.Instructors.ToListAsync();

    public async Task<Instructor?> GetByIdAsync(int id)
        => await db.Instructors.FindAsync(id);

    public async Task<Instructor> CreateAsync(Instructor instructor)
    {
        try
        {
            db.Instructors.Add(instructor);
            await db.SaveChangesAsync();
            return instructor;
        }
        catch (DbUpdateException ex) when (
            ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            throw new ConflictException("An instructor with this email already exists.");
        }
    }

    public async Task UpdateAsync(Instructor instructor)
    {
        try
        {
            db.Instructors.Update(instructor);
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (
            ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            throw new ConflictException("An instructor with this email already exists.");
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var instructor = await db.Instructors.FindAsync(id);
        if (instructor is null) return false;

        db.Instructors.Remove(instructor);
        await db.SaveChangesAsync();
        return true;
    }
}