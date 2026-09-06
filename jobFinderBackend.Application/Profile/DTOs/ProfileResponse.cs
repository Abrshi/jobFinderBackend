namespace jobFinderBackend.Application.Profile.DTOs;

public class ProfileResponse
{
    public List<EducationDto> Education { get; set; } = [];

    public List<ExperienceDto> Experience { get; set; } = [];
}