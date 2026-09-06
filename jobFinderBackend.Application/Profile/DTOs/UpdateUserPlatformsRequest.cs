namespace jobFinderBackend.Application.Profile.DTOs;

public class UpdateUserPlatformsRequest
{
    public List<int> PlatformIds { get; set; } = new();
}
