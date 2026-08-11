using MediatR;
using jobFinderBackend.Application.Interfaces;
using UserEntity = jobFinder.Domain.Entities.Users;

namespace jobFinderBackend.Application.Users.Commands;

public class RegisterUserCommandHandler
    : IRequestHandler<RegisterUserCommand, UserEntity>
{
    private readonly IUserRepository _userRepository;

    public RegisterUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserEntity> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        var exists = await _userRepository
            .ExistsByEmailAsync(request.Email);

        if (exists)
        {
            throw new InvalidOperationException(
                "A user with this email already exists.");
        }

        var user = new UserEntity
        {
            Email = request.Email,
            PasswordHash = request.Password
        };

        await _userRepository.AddAsync(user);

        return user;
    }
}