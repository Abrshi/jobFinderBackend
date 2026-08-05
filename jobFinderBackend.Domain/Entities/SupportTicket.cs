namespace jobFinder.Domain.Entities;

public class SupportTicket
{
    public int Id { get; set; }


    public int UserId { get; set; }



    public string Subject { get; set; } = null!;


    public string Message { get; set; } = null!;



    // Open
    // InProgress
    // Closed

    public string Status { get; set; }
        = "Open";



    // Low
    // Medium
    // High

    public string Priority { get; set; }
        = "Medium";



    public DateTime CreatedAt { get; set; }
        = DateTime.UtcNow;



    public DateTime? ClosedAt { get; set; }



    // Navigation


    public Users User { get; set; } = null!;
}