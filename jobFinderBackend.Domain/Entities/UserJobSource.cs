namespace jobFinder.Domain.Entities;

public class UserJobSource
{
    public int Id { get; set; }


    public int UserId { get; set; }


    public int JobPlatformId { get; set; }


    public bool IsActive { get; set; } = true;


    public DateTime SelectedAt { get; set; }
        = DateTime.UtcNow;


    public DateTime? LastSyncDate { get; set; }



    // Navigation

    public Users User { get; set; } = null!;


    public JobPlatform JobPlatform { get; set; } = null!;
}