using FluentValidation;
using TenantFlow.Application.DTOs;

namespace TenantFlow.Application.Validators;

public class CreateTaskDtoValidator : AbstractValidator<CreateTaskDto>
{
    public CreateTaskDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

        RuleFor(x => x.DueDate)
            .GreaterThan(_ => DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("The due date must be in the future.");

    }
}