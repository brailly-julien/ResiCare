using FluentValidation;

namespace ResiCare.Application.CareTasks.Commands.CreateCareTask;

public sealed class CreateCareTaskCommandValidator : AbstractValidator<CreateCareTaskCommand>
{
    public CreateCareTaskCommandValidator()
    {
        RuleFor(x => x.ResidentId).NotEmpty();
        RuleFor(x => x.Label).NotEmpty().MaximumLength(200);
    }
}
