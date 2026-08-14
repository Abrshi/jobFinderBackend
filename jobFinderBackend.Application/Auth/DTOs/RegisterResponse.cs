namespace jobFinderBackend.Application.DTOs.Auth;

public class RegisterResponse
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Message { get; set; } = null!;
}