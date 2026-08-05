namespace jobFinder.Domain.Entities;

public class InterviewSession
{
    public int Id { get; set; }


    public int UserId { get; set; }


    public int JobId { get; set; }



    public decimal? OverallScore { get; set; }



    // Started
    // Completed
    // Cancelled

    public string Status { get; set; }
        = "Started";



    public DateTime StartedAt { get; set; }
        = DateTime.UtcNow;


    public DateTime? CompletedAt { get; set; }



    // Navigation


    public Users User { get; set; } = null!;


    public Job Job { get; set; } = null!;


    public ICollection<InterviewQuestion> Questions { get; set; }
        = new List<InterviewQuestion>();
}