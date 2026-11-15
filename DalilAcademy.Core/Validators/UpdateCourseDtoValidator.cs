using DalilAcademy.Core.DTOs;
using FluentValidation;

namespace DalilAcademy.Core.Validators;

public class UpdateCourseDtoValidator : AbstractValidator<UpdateCourseDto>
{
    public UpdateCourseDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(1000);

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price must be zero or more.");

        RuleFor(x => x.Level)
            .NotEmpty()
            .MaximumLength(50);


        RuleFor(x => x.ImageUrl)
        .Must(url => url is null || (url.StartsWith("/images/courses/") && !url.Contains("..")))
        .WithMessage("ImageUrl must come from the upload endpoint.");
    }
}