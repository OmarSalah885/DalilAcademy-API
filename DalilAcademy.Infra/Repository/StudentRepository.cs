using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Entities;
using DalilAcademy.Core.Enums;
using DalilAcademy.Core.Exceptions;
using DalilAcademy.Core.Repository;
using DalilAcademy.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DalilAcademy.Infra.Repository;

public class StudentRepository(AppDbContext db) : IStudentRepository
{
    public async Task<List<Student>> GetAllAsync()
    {
        return await db.Students.ToListAsync();
    }
    

    public async Task<Student?> GetByIdAsync(int id)
    {
        return await db.Students.FindAsync(id);
    }
    // Search by Student ID
    public async Task<List<CourseResponseDto>> GetCoursesByStudentIdAsync(int studentId)
    {
        return await db.Enrollments
            .AsNoTracking()
            .Where(e => e.StudentId == studentId)
            .Select(e => new CourseResponseDto(
                e.Course.Id,
                e.Course.Title,
                e.Course.Description,
                e.Course.Price,
                e.Course.Level,
                e.Course.Instructor!.FullName,
                e.Course.ImageUrl))
            .ToListAsync();
    }

    // Search by User ID
    public async Task<List<CourseResponseDto>> GetCoursesByUserIdAsync(int userId)
    {
        return await db.Enrollments
            .AsNoTracking()
            .Where(e => e.Student.UserId == userId
                     && e.Status == EnrollmentStatus.Paid)
            .Select(e => new CourseResponseDto(
                e.Course.Id,
                e.Course.Title,
                e.Course.Description,
                e.Course.Price,
                e.Course.Level,
                e.Course.Instructor!.FullName,
                e.Course.ImageUrl))
            .ToListAsync();
    }
    public async Task<PagedResult<StudentResponseDto>> GetPagedAsync(int page, int pageSize)
    {
        var query = db.Students.AsNoTracking();

        var total = await query.CountAsync();

        var items = await query
            .OrderBy(s => s.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new StudentResponseDto(
                s.Id,
                s.FullName,
                s.Email
            ))
            .ToListAsync();

        return new PagedResult<StudentResponseDto>(items, page, pageSize, total);
    }
    public async Task<Student?> GetByUserIdAsync(int userId)
    {
        return await db.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == userId);
    }

    public async Task<Student> CreateAsync(Student student)
    {
        try
        {
            db.Students.Add(student);
            await db.SaveChangesAsync();
            return student;
        }
        catch (DbUpdateException ex) when (
            ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            throw new ConflictException("A student with this email already exists.");
        }
    }

    public async Task UpdateAsync(Student student)
    {
        try
        {
            db.Students.Update(student);
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (
            ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            throw new ConflictException("A student with this email already exists.");
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var student = await db.Students.FindAsync(id);
        if (student is null) return false;

        db.Students.Remove(student);
        await db.SaveChangesAsync();
        return true;
    }
}