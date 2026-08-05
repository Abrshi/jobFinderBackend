namespace jobFinder.Domain.Entities;

public class SystemSetting
{
    public int Id { get; set; }


    public string Key { get; set; } = null!;


    public string Value { get; set; } = null!;


    public string? Description { get; set; }



    public DateTime UpdatedAt { get; set; }
        = DateTime.UtcNow;
}