namespace jobFinder.Domain.Entities;

public class Experience
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? Company { get; set; }

    public string Position { get; set; } = null!;

    public string? Description { get; set; }


    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }


    public bool IsCurrent { get; set; }


    public Users User { get; set; } = null!;
}