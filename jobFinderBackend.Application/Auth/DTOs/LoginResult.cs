namespace jobFinderBackend.Application.DTOs.Auth;

public class LoginResult
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string Token { get; set; } = null!;

    public SubscriptionPlanDto? Subscription { get; set; }
}