namespace jobFinder.Domain.Entities;

public class SubscriptionPlans
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public int MaxJobSources { get; set; }

    public bool CanGenerateCV { get; set; }

    public bool CanGenerateCoverLetter { get; set; }

    public bool CanUseInterviewAI { get; set; }


    public ICollection<Users> Users { get; set; } = new List<Users>();
}