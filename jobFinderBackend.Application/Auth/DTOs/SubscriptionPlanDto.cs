namespace jobFinderBackend.Application.DTOs.Auth;

public class SubscriptionPlanDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public int MaxJobSources { get; set; }

    public bool CanGenerateCV { get; set; }

    public bool CanGenerateCoverLetter { get; set; }

    public bool CanUseInterviewAI { get; set; }
}