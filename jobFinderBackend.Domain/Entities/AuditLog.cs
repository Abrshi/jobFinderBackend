namespace jobFinder.Domain.Entities;

public class AuditLog
{
    public int Id { get; set; }



    public int? UserId { get; set; }



    public string Action { get; set; } = null!;



    public string EntityName { get; set; } = null!;



    public int? EntityId { get; set; }



    public string? OldValue { get; set; }


    public string? NewValue { get; set; }



    public DateTime CreatedAt { get; set; }
        = DateTime.UtcNow;



    // Navigation


    public Users? User { get; set; }
}