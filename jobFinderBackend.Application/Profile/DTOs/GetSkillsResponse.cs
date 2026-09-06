namespace jobFinderBackend.Application.Profile.DTOs;

public class GetSkillsResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Category { get; set; }
}
