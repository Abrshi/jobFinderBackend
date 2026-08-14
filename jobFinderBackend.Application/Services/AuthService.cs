using FluentValidation;
using jobFinder.Application.DTOs.Auth;
using jobFinder.Application.Interfaces;
using jobFinder.Domain.Entities;
using jobFinderBackend.Application.Interfaces;

namespace jobFinder.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IValidator<RegisterRequest> _registerValidator;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IValidator<RegisterRequest> registerValidator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _registerValidator = registerValidator;
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        // 1. Validate registration data
        await _registerValidator.ValidateAndThrowAsync(request);

        // 2. Check if email already exists
        var emailExists = await _userRepository.ExistsByEmailAsync(request.Email);

        if (emailExists)
        {
            throw new InvalidOperationException("Email is already registered.");
        }

        // 3. Hash password
        var passwordHash = _passwordHasher.Hash(request.Password);

        // 4. Create user entity
        var user = new Users
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = passwordHash,
            SubscriptionPlanId = 1
        };

        // 5. Save user to database
        await _userRepository.AddAsync(user);

        // 6. Return response
        return new RegisterResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Message = "Registration successful."
        };
    }
}