namespace jobFinder.Domain.Entities;

public class Notification
{
    public int Id { get; set; }


    public int UserId { get; set; }



    public string Title { get; set; } = null!;


    public string Message { get; set; } = null!;



    // JobAlert
    // System
    // Payment
    // Interview

    public string Type { get; set; } = null!;



    public bool IsRead { get; set; }


    public DateTime CreatedAt { get; set; }
        = DateTime.UtcNow;



    // Navigation


    public Users User { get; set; } = null!;
}