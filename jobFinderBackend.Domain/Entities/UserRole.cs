namespace jobFinder.Domain.Entities;

public class UserRole
{
    public int Id { get; set; }


    public int UserId { get; set; }


    public int RoleId { get; set; }



    // Navigation


    public Users User { get; set; } = null!;


    public Role Role { get; set; } = null!;
}