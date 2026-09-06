namespace jobFinderBackend.Application.Profile.DTOs;

public class GetMyJobPlatformResponse
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public int? JobPlatformId { get; set; }
    public bool? IsActive { get; set; }
    public string? SelectedAt { get; set; }
    public DateTime? LastSyncDate { get; set; }
}
