using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Entities;
using DalilAcademy.Core.Exceptions;
using DalilAcademy.Core.Mappings;
using DalilAcademy.Core.Repository;
using DalilAcademy.Core.Service;
using FluentValidation;

namespace DalilAcademy.Infra.Service;

public class CourseService(
    ICourseRepository courseRepository,
    IValidator<CreateCourseDto> createValidator,
    IValidator<UpdateCourseDto> updateValidator,
    IImageService images) : ICourseService
{
    public async Task<List<CourseResponseDto>> GetAllAsync() => await courseRepository.GetAllAsync();

    public async Task<PagedResult<CourseResponseDto>> GetPagedAsync(int page, int pageSize, decimal? minPrice = null, decimal? maxPrice = null)
        => await courseRepository.GetPagedAsync(page, pageSize,minPrice,maxPrice);

    public async Task<int> GetCountAsync(decimal? minPrice = null, decimal? maxPrice = null)
    {
        return await courseRepository.GetCountAsync(minPrice,maxPrice);
    }
    public async Task<CourseResponseDto> GetByIdAsync(int id)
    {
        var course = await courseRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Course with id {id} was not found.");

        return course.ToDto();
    }


    public async Task<CourseResponseDto> CreateAsync(CreateCourseDto dto)
    {
        await createValidator.ValidateAndThrowAsync(dto);

        var course = dto.ToEntity();
        var created = await courseRepository.CreateAsync(course);

        return created.ToDto();
    }

    public async Task UpdateAsync(int id, UpdateCourseDto dto)
    {
        await updateValidator.ValidateAndThrowAsync(dto);          

        var existing = await courseRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Course with id {id} was not found.");

        var oldImage = existing.ImageUrl;                          

        existing.Title = dto.Title;
        existing.Description = dto.Description;
        existing.Price = dto.Price;
        existing.Level = dto.Level;
        existing.ImageUrl = dto.ImageUrl;

        await courseRepository.UpdateAsync(existing);              

        if (oldImage != dto.ImageUrl)                              
            images.DeleteCourseImage(oldImage);                    
    }

    public async Task DeleteAsync(int id)
    {
        var course = await courseRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Course {id} was not found.");
        await courseRepository.DeleteAsync(id);          // 1. database first
        images.DeleteCourseImage(course.ImageUrl);       // 2. then the file
    }

    public async Task<List<CourseWithEnrollmentsDto>> GetAllWithEnrollmentsAsync()
    {
        return await courseRepository.GetAllWithEnrollmentsAsync();
    }
}