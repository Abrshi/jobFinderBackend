namespace jobFinderBackend.Application.DTOs.Auth;

public class LoginResponse
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string Role { get; set; } = null!;

    public SubscriptionPlanDto? Subscription { get; set; }
}