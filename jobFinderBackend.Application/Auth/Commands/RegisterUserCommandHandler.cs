using MediatR;
using jobFinderBackend.Application.Interfaces;
using UserEntity = jobFinder.Domain.Entities.Users;

namespace jobFinderBackend.Application.Users.Commands;
public record RegisterUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password
) : IRequest<UserEntity>;
public class RegisterUserCommandHandler
    : IRequestHandler<RegisterUserCommand, UserEntity>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
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

        var passwordHash = _passwordHasher.Hash(
            request.Password
        );

        var user = new UserEntity
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = passwordHash,

            // Default subscription plan
            SubscriptionPlanId = 1
        };

        await _userRepository.AddAsync(user);

        return user;
    }
}