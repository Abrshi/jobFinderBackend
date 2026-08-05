namespace jobFinder.Domain.Entities;

public class JobSkill
{
    public int Id { get; set; }


    public int JobId { get; set; }


    public int SkillId { get; set; }



    // Required
    // Preferred
    // NiceToHave

    public string Importance { get; set; } = null!;



    // Navigation


    public Job Job { get; set; } = null!;


    public Skill Skill { get; set; } = null!;
}