namespace jobFinderBackend.Application.Profile.DTOs;

public class UpdateUserSkillsRequest
{
    public List<UserSkillSelectionDto> Skills { get; set; } = new();
}

public class UserSkillSelectionDto
{
    public int SkillId { get; set; }

    public string? Level { get; set; }

    public int? YearsOfExperience { get; set; }
}
