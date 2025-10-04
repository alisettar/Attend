using Carter;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Attend.Application.Data;
using Attend.Application.Data.Users.Commands;
using Attend.Application.Data.Users.Queries;

namespace Attend.Api.Modules;

public class UsersModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/{id}", GetUserById);
        app.MapGet("/users/qrcode/{qrcode}", GetUserByQRCode);
        app.MapGet("/users", GetUsers);
        app.MapPost("/users", CreateUser);
        app.MapPut("/users/{id}", UpdateUser);
        app.MapDelete("/users/{id}", DeleteUser);
    }

    private static async Task<Results<Ok<UserResponse>, NotFound>> GetUserById(
        Guid id,
        [FromServices] ISender sender,
        HttpContext context)
    {
        var user = await sender.Send(new GetUserByIdQuery(id), context.RequestAborted);
        return user is not null ? TypedResults.Ok(user) : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<UserResponse>, NotFound>> GetUserByQRCode(
        string qrcode,
        [FromServices] ISender sender,
        HttpContext context)
    {
        var user = await sender.Send(new GetUserByQRCodeQuery(qrcode), context.RequestAborted);
        return user is not null ? TypedResults.Ok(user) : TypedResults.NotFound();
    }

    private static async Task<Ok<PaginationResponse<UserResponse>>> GetUsers(
        [FromQuery] PaginationRequest? paginationRequest,
        [FromServices] ISender sender,
        HttpContext context)
    {
        var result = await sender.Send(
            new GetUsersQuery(paginationRequest ?? new()), 
            context.RequestAborted);
        return TypedResults.Ok(result);
    }

    private static async Task<Created> CreateUser(
        UserRequest request,
        [FromServices] ISender sender,
        HttpContext context)
    {
        var userId = await sender.Send(new CreateUserCommand(request), context.RequestAborted);
        return TypedResults.Created($"/users/{userId}");
    }

    private static async Task<Results<Ok, NotFound>> UpdateUser(
        Guid id,
        UserRequest request,
        [FromServices] ISender sender,
        HttpContext context)
    {
        var requestWithId = request with { Id = id };
        var result = await sender.Send(new UpdateUserCommand(requestWithId), context.RequestAborted);
        return result ? TypedResults.Ok() : TypedResults.NotFound();
    }

    private static async Task<Results<NoContent, NotFound>> DeleteUser(
        Guid id,
        [FromServices] ISender sender,
        HttpContext context)
    {
        var result = await sender.Send(new DeleteUserCommand(id), context.RequestAborted);
        return result ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
