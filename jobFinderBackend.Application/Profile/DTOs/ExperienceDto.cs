namespace jobFinderBackend.Application.Profile.DTOs;

public class ExperienceDto
{
    public int? Id { get; set; }

    public string? Company { get; set; }

    public string Position { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool IsCurrent { get; set; }
}