namespace jobFinder.Domain.Entities;

public class JobCategory
{
    public int Id { get; set; }


    public string Name { get; set; } = null!;


    // For parent-child categories

    public int? ParentCategoryId { get; set; }



    // Navigation

    public JobCategory? ParentCategory { get; set; }


    public ICollection<JobCategory> SubCategories { get; set; }
        = new List<JobCategory>();


    public ICollection<Job> Jobs { get; set; }
        = new List<Job>();
}