namespace jobFinder.Domain.Entities;

public class Education
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Institution { get; set; } = null!;

    public string Degree { get; set; } = null!;

    public string? FieldOfStudy { get; set; }


    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }


    public Users User { get; set; } = null!;
}