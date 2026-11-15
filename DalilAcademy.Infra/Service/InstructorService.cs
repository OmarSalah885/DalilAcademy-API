using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Exceptions;
using DalilAcademy.Core.Mappings;
using DalilAcademy.Core.Repository;
using DalilAcademy.Core.Service;
using FluentValidation;

namespace DalilAcademy.Infra.Service;

public class InstructorService(
    IInstructorRepository instructorRepository,
    IValidator<CreateInstructorDto> createValidator) : IInstructorService
{
    public async Task<List<InstructorResponseDto>> GetAllAsync()
    {
        var instructors = await instructorRepository.GetAllAsync();
        return instructors.Select(i => i.ToDto()).ToList();
    }

    public async Task<InstructorResponseDto> GetByIdAsync(int id)
    {
        var instructor = await instructorRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Instructor with id {id} was not found.");

        return instructor.ToDto();
    }

    public async Task<InstructorResponseDto> CreateAsync(CreateInstructorDto dto)
    {
        await createValidator.ValidateAndThrowAsync(dto);

        var instructor = dto.ToEntity();
        var created = await instructorRepository.CreateAsync(instructor);

        return created.ToDto();
    }

    public async Task UpdateAsync(int id, UpdateInstructorDto dto)
    {
        var existing = await instructorRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Instructor with id {id} was not found.");

        existing.FullName = dto.FullName;
        existing.Email = dto.Email;

        await instructorRepository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(int id)
    {
        var deleted = await instructorRepository.DeleteAsync(id);
        if (!deleted)
            throw new NotFoundException($"Instructor with id {id} was not found.");
    }
}