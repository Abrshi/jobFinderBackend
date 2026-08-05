namespace jobFinder.Domain.Entities;

public class Company
{
    public int Id { get; set; }


    public string Name { get; set; } = null!;


    public string? Website { get; set; }


    public string? Logo { get; set; }


    public string? Industry { get; set; }


    public string? Country { get; set; }



    // Navigation

    public ICollection<Job> Jobs { get; set; }
        = new List<Job>();
}