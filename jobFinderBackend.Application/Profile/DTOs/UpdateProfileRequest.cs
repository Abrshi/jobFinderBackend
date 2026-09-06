namespace jobFinderBackend.Application.Profile.DTOs;

public class UpdateProfileRequest
{
    public List<EducationDto> Education { get; set; } = [];

    public List<ExperienceDto> Experience { get; set; } = [];
}