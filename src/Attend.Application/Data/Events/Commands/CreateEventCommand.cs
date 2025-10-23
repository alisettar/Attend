using Attend.Application.Repositories;
using Attend.Domain.Entities;
using FluentValidation;
using MediatR;

namespace Attend.Application.Data.Events.Commands;

public sealed record CreateEventCommand(EventRequest Request) : IRequest<Guid>;

public sealed class CreateEventCommandHandler(IEventRepository repository)
    : IRequestHandler<CreateEventCommand, Guid>
{
    public async Task<Guid> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        var @event = Event.Create(
            title: request.Request.Title,
            description: request.Request.Description,
            date: request.Request.Date);

        await repository.AddAsync(@event, cancellationToken);
        return @event.Id;
    }
}

public sealed class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventCommandValidator()
    {
        RuleFor(x => x.Request.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(200)
            .WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Request.Date)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Event date must be in the future.");
    }
}
