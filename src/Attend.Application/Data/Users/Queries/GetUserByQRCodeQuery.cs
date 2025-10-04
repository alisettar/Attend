using MediatR;
using Attend.Application.Repositories;

namespace Attend.Application.Data.Users.Queries;

public sealed record GetUserByQRCodeQuery(string QRCode) : IRequest<UserResponse?>;

public sealed class GetUserByQRCodeQueryHandler(IUserRepository repository) 
    : IRequestHandler<GetUserByQRCodeQuery, UserResponse?>
{
    public async Task<UserResponse?> Handle(GetUserByQRCodeQuery request, CancellationToken cancellationToken)
    {
        var user = await repository.GetByQRCodeAsync(request.QRCode, cancellationToken);
        return user != null ? UserResponse.FromDomain(user) : null;
    }
}
