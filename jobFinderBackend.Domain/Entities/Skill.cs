namespace jobFinder.Domain.Entities;

public class Skill
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Category { get; set; }


    public ICollection<UserSkill> UserSkills { get; set; } 
        = new List<UserSkill>();

    public ICollection<JobSkill> JobSkills { get; set; }
        = new List<JobSkill>();
}