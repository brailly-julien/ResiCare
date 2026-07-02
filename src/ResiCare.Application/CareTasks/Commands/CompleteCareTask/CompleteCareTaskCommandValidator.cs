using FluentValidation;

namespace ResiCare.Application.CareTasks.Commands.CompleteCareTask;

public sealed class CompleteCareTaskCommandValidator : AbstractValidator<CompleteCareTaskCommand>
{
    public CompleteCareTaskCommandValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
    }
}
