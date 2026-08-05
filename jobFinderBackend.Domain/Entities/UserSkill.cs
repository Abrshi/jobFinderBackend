namespace jobFinder.Domain.Entities;

public class UserSkill
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int SkillId { get; set; }


    public string? Level { get; set; }

    public int? YearsOfExperience { get; set; }


    public Users User { get; set; } = null!;

    public Skill Skill { get; set; } = null!;
}