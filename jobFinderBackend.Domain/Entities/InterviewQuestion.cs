namespace jobFinder.Domain.Entities;

public class InterviewQuestion
{
    public int Id { get; set; }


    public int InterviewSessionId { get; set; }



    public string Question { get; set; } = null!;



    // Technical
    // Behavioral
    // HR
    // Coding

    public string QuestionType { get; set; } = null!;



    // Easy
    // Medium
    // Hard

    public string? Difficulty { get; set; }



    public string? UserAnswer { get; set; }



    public decimal? Score { get; set; }



    public string? Feedback { get; set; }



    // Navigation


    public InterviewSession InterviewSession { get; set; } = null!;
}