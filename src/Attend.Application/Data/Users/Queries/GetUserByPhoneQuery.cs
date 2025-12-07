using Attend.Application.Repositories;
using MediatR;

namespace Attend.Application.Data.Users.Queries;

public sealed record GetUserByPhoneQuery(string Phone) : IRequest<UserResponse?>;

public sealed class GetUserByPhoneQueryHandler(IUserRepository repository)
    : IRequestHandler<GetUserByPhoneQuery, UserResponse?>
{
    public async Task<UserResponse?> Handle(GetUserByPhoneQuery request, CancellationToken cancellationToken)
    {
        var user = await repository.GetByPhoneAsync(request.Phone, cancellationToken);
        return user != null ? UserResponse.FromDomain(user) : null;
    }
}
