using Attend.Application.Repositories;
using MediatR;

namespace Attend.Application.Data.Users.Queries;

public sealed record GetUserByIdQuery(Guid Id) : IRequest<UserResponse?>;

public sealed class GetUserByIdQueryHandler(IUserRepository repository)
    : IRequestHandler<GetUserByIdQuery, UserResponse?>
{
    public async Task<UserResponse?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await repository.GetByIdAsync(request.Id, cancellationToken);
        return user != null ? UserResponse.FromDomain(user) : null;
    }
}
