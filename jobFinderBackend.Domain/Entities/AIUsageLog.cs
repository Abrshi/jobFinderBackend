namespace jobFinder.Domain.Entities;

public class AIUsageLog
{
    public int Id { get; set; }


    public int UserId { get; set; }


    // CV
    // CoverLetter
    // Interview

    public string Feature { get; set; } = null!;



    public int TokensUsed { get; set; }



    public decimal EstimatedCost { get; set; }



    public DateTime CreatedAt { get; set; }
        = DateTime.UtcNow;



    // Navigation


    public Users User { get; set; } = null!;
}