using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Entities;
using DalilAcademy.Core.Exceptions;
using DalilAcademy.Core.Repository;
using DalilAcademy.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DalilAcademy.Infra.Repository;

public class EfCourseRepository(AppDbContext db) : ICourseRepository
{
    public async Task<List<CourseResponseDto>> GetAllAsync()
    {
        return await db.Courses
            .AsNoTracking()
            .Select(c => new CourseResponseDto(
                c.Id,
                c.Title,
                c.Description,
                c.Price,
                c.Level,
                c.Instructor!.FullName,
                c.ImageUrl))
            .ToListAsync();
    }

    public async Task<PagedResult<CourseResponseDto>> GetPagedAsync(
    int page, int pageSize,
    decimal? minPrice = null,
    decimal? maxPrice = null)
    {
        var query = db.Courses.AsNoTracking();

        if (minPrice.HasValue)
            query = query.Where(c => c.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(c => c.Price <= maxPrice.Value);

        var total = await query.CountAsync();

        var items = await query
            .OrderBy(c => c.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CourseResponseDto(
                c.Id,
                c.Title,
                c.Description,
                c.Price,
                c.Level,
                c.Instructor!.FullName,
                c.ImageUrl))
            .ToListAsync();

        return new PagedResult<CourseResponseDto>(items, page, pageSize, total);
    }

    public Task<int> GetCountAsync(decimal? minPrice = null, decimal? maxPrice = null)
    {
        var query = db.Courses.AsNoTracking();

        if (minPrice.HasValue)
            query = query.Where(c => c.Price >= minPrice.Value);
        if (maxPrice.HasValue)
            query = query.Where(c => c.Price <= maxPrice.Value);

        return query.CountAsync();
    }
    public async Task<Course?> GetByIdAsync(int id)
    {
        return await db.Courses
            .Include(c => c.Instructor)
            .FirstOrDefaultAsync(c => c.Id == id);
    }


    public async Task<Course> CreateAsync(Course course)
    {
        try
        {
            db.Courses.Add(course);
            await db.SaveChangesAsync();
            return course;
        }
        catch (DbUpdateException ex) when (
            ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            throw new ConflictException("A course with this title already exists.");
        }
    }

    public async Task UpdateAsync(Course course)
    {
        try
        {
            db.Courses.Update(course);
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (
            ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            throw new ConflictException("A course with this title already exists.");
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var course = await db.Courses.FindAsync(id);
        if (course is null) return false;

        db.Courses.Remove(course);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<List<CourseWithEnrollmentsDto>> GetAllWithEnrollmentsAsync()
    {
        return await db.Courses
            .AsNoTracking()
            .Select(c => new CourseWithEnrollmentsDto(
                c.Id,
                c.Title,
                c.Instructor.FullName,
                c.Enrollments.Select(e => new EnrollmentSummaryDto(
                    e.Student.Id,
                    e.Student.FullName,
                    e.Student.Email,
                    e.EnrolledAt
                )).ToList()
            ))
            .ToListAsync();
    }
}