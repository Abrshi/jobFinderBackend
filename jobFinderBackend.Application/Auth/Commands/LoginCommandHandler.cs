using jobFinderBackend.Application.DTOs.Auth;
using jobFinderBackend.Application.Interfaces;
using MediatR;

namespace jobFinderBackend.Application.Users.Commands;

public record LoginUserCommand(
    string Email,
    string Password
) : IRequest<LoginResult>;


public class LoginUserCommandHandler
    : IRequestHandler<LoginUserCommand, LoginResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResult> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Find user
        var user = await _userRepository
            .GetByEmailAsync(request.Email);

        if (user == null)
        {
            throw new InvalidOperationException(
                "Invalid email or password."
            );
        }


        // 2. Verify password
        var passwordValid = _passwordHasher.Verify(
            request.Password,
            user.PasswordHash
        );

        if (!passwordValid)
        {
            throw new InvalidOperationException(
                "Invalid email or password."
            );
        }


        // 3. Get user's role
        var role = await _userRepository
            .GetRoleByUserIdAsync(user.Id);

        if (role == null)
        {
            throw new InvalidOperationException(
                "User role not found."
            );
        }

        // 4. Get user's active subscription
        var subscriptionPlan = await _userRepository.GetActiveSubscriptionPlanAsync(user.Id);


        // 5. Generate JWT
        var token = _jwtTokenGenerator.GenerateToken(
            user.Id,
            user.Email,
            role.Name
        );


        // 6. Return login result
        return new LoginResult
        {
            Id = user.Id,
            Email = user.Email,
            Role = role.Name,
            Token = token,
            Subscription = subscriptionPlan != null ? MapToDto(subscriptionPlan) : null
        };
    }

    private static SubscriptionPlanDto MapToDto(jobFinder.Domain.Entities.SubscriptionPlans plan)
    {
        return new SubscriptionPlanDto
        {
            Id = plan.Id,
            Name = plan.Name,
            Price = plan.Price,
            MaxJobSources = plan.MaxJobSources,
            CanGenerateCV = plan.CanGenerateCV,
            CanGenerateCoverLetter = plan.CanGenerateCoverLetter,
            CanUseInterviewAI = plan.CanUseInterviewAI
        };
    }
}