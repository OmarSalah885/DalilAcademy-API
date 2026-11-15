using DalilAcademy.Core.DTOs;
using FluentValidation;

namespace DalilAcademy.Core.Validators;

public class CreateInstructorDtoValidator : AbstractValidator<CreateInstructorDto>
{
    public CreateInstructorDtoValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(200);
    }
}