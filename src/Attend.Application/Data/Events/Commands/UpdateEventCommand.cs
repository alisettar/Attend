using Attend.Application.Repositories;
using Attend.Domain.Entities;
using FluentValidation;
using MediatR;

namespace Attend.Application.Data.Events.Commands;

public sealed record UpdateEventCommand(EventRequest Request) : IRequest<bool>;

public sealed class UpdateEventCommandHandler(IEventRepository repository)
    : IRequestHandler<UpdateEventCommand, bool>
{
    public async Task<bool> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
    {
        if (!request.Request.Id.HasValue)
            throw new ArgumentException("Event ID is required");

        var @event = await repository.GetByIdAsync(request.Request.Id.Value, cancellationToken);
        if (@event == null)
            return false;

        Event.Update(@event,
            title: request.Request.Title,
            description: request.Request.Description,
            date: request.Request.Date);

        await repository.UpdateAsync(@event, cancellationToken);
        return true;
    }
}

public sealed class UpdateEventCommandValidator : AbstractValidator<UpdateEventCommand>
{
    public UpdateEventCommandValidator()
    {
        RuleFor(x => x.Request.Id)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.Request.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(200)
            .WithMessage("Title must not exceed 200 characters.");
    }
}
