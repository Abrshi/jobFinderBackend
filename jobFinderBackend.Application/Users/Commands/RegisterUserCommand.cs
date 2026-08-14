using MediatR;
using UserEntity = jobFinder.Domain.Entities.Users;

namespace jobFinderBackend.Application.Users.Commands;

public record RegisterUserCommand(
    string Email,
    string Password
) : IRequest<UserEntity>;