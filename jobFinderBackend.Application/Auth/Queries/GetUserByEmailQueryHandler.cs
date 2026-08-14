using MediatR;
using jobFinderBackend.Application.Interfaces;
using UserEntity = jobFinder.Domain.Entities.Users;

namespace jobFinderBackend.Application.Users.Queries;
public record GetUserByEmailQuery(
    string Email
) : IRequest<UserEntity?>;
public class GetUserByEmailQueryHandler
    : IRequestHandler<GetUserByEmailQuery, UserEntity?>
{
    private readonly IUserRepository _userRepository;

    public GetUserByEmailQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserEntity?> Handle(
        GetUserByEmailQuery request,
        CancellationToken cancellationToken)
    {
        return await _userRepository
            .GetByEmailAsync(request.Email);
    }
}