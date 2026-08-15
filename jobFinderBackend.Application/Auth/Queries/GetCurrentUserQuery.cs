using jobFinderBackend.Application.DTOs.Auth;
using jobFinderBackend.Application.Interfaces;
using MediatR;

namespace jobFinderBackend.Application.Users.Queries.GetCurrentUser;

public record GetCurrentUserQuery : IRequest<GetCurrentUserResponse>;
public class GetCurrentUserQueryHandler
    : IRequestHandler<GetCurrentUserQuery, GetCurrentUserResponse>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUserRepository _userRepository;

    public GetCurrentUserQueryHandler(
        ICurrentUserService currentUser,
        IUserRepository userRepository)
    {
        _currentUser = currentUser;
        _userRepository = userRepository;
    }

    public async Task<GetCurrentUserResponse> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new UnauthorizedAccessException();
        }

        var user = await _userRepository.GetByIdAsync(_currentUser.UserId.Value);

        if (user is null)
        {
            throw new UnauthorizedAccessException();
        }

        var role = await _userRepository.GetRoleByUserIdAsync(user.Id);

        var subscriptionPlan = await _userRepository.GetActiveSubscriptionPlanAsync(user.Id);

        return new GetCurrentUserResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = role?.Name ?? _currentUser.Role ?? string.Empty,
            SubscriptionPlanId = user.SubscriptionPlanId,
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