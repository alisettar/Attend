using MediatR;
using Attend.Application.Repositories;

namespace Attend.Application.Data.Users.Queries;

public sealed record GetUsersQuery(PaginationRequest? PaginationRequest = null) : IRequest<PaginationResponse<UserResponse>>;

public sealed class GetUsersQueryHandler(IUserRepository repository) 
    : IRequestHandler<GetUsersQuery, PaginationResponse<UserResponse>>
{
    public async Task<PaginationResponse<UserResponse>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var paginationRequest = request.PaginationRequest ?? new PaginationRequest();
        var (users, totalCount) = await repository.GetPaginatedAsync(paginationRequest, cancellationToken);
        
        var responseList = UserResponse.FromDomainList(users);

        return new PaginationResponse<UserResponse>(responseList, totalCount);
    }
}
