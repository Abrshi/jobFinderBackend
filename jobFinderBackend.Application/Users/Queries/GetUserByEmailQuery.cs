using MediatR;
using UserEntity = jobFinder.Domain.Entities.Users;

namespace jobFinderBackend.Application.Users.Queries;

public record GetUserByEmailQuery(
    string Email
) : IRequest<UserEntity?>;