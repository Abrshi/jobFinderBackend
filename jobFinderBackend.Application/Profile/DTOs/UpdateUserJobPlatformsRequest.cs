namespace jobFinderBackend.Application.Profile.DTOs;

public class UpdateUserJobPlatformsRequest
{
    public List<UserJobPlatformSelectionDto> JobPlatforms { get; set; } = new();
}

public class UserJobPlatformSelectionDto
{
    public int JobPlatformId { get; set; }

    public string? AccountUrl { get; set; }

    public bool IsSynced { get; set; }
}
