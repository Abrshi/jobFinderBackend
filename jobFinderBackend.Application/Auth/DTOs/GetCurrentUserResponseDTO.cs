using jobFinderBackend.Application.DTOs.Auth;

namespace jobFinderBackend.Application.Users.Queries.GetCurrentUser;

public class GetCurrentUserResponse
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public int SubscriptionPlanId { get; set; }

    public SubscriptionPlanDto? Subscription { get; set; }
}