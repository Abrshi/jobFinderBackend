namespace jobFinderBackend.Application.Profile.DTOs;

public class GetJobPlatformResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Website { get; set; }
    public string? Logo { get; set; }
    public string? SourceType { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsMy { get; set; }
}
