namespace jobFinder.Domain.Entities;

public class SavedJob
{
    public int Id { get; set; }


    public int UserId { get; set; }


    public int JobId { get; set; }


    public DateTime SavedAt { get; set; }
        = DateTime.UtcNow;



    // Navigation


    public Users User { get; set; } = null!;


    public Job Job { get; set; } = null!;
}