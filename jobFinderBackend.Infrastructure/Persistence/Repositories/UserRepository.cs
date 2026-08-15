using jobFinder.Domain.Entities;
using jobFinderBackend.Application.Interfaces;
using jobFinderBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace jobFinderBackend.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly JobFinderBackendDbContext _context;

    public UserRepository(JobFinderBackendDbContext context)
    {
        _context = context;
    }

    public async Task<Users?> GetByIdAsync(int id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Users?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<Role?> GetRoleByUserIdAsync(int userId)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Users
            .AnyAsync(u => u.Email == email);
    }

    public async Task AddAsync(Users user)
    {
        // Add the user
        await _context.Users.AddAsync(user);

        // Save first so PostgreSQL generates the User Id
        await _context.SaveChangesAsync();

        // Find the default JOB_SEEKER role
        var jobSeekerRole = await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == "JOB_SEEKER");

        if (jobSeekerRole is null)
        {
            throw new InvalidOperationException(
                "The JOB_SEEKER role does not exist."
            );
        }

        // Connect the new user with JOB_SEEKER
        var userRole = new UserRole
        {
            UserId = user.Id,
            RoleId = jobSeekerRole.Id
        };

        await _context.UserRoles.AddAsync(userRole);

        // Create initial user subscription with Free plan (Id = 1)
        var userSubscription = new UserSubscription
        {
            UserId = user.Id,
            SubscriptionPlanId = 1,
            StartDate = DateTime.UtcNow,
            EndDate = null,
            IsActive = true
        };

        await _context.UserSubscriptions.AddAsync(userSubscription);

        await _context.SaveChangesAsync();
    }

    public async Task<SubscriptionPlans?> GetActiveSubscriptionPlanAsync(int userId)
    {
        var activeSubscription = await _context.UserSubscriptions
            .Where(us => us.UserId == userId && us.IsActive)
            .Where(us => us.EndDate == null || us.EndDate > DateTime.UtcNow)
            .OrderByDescending(us => us.StartDate)
            .Select(us => us.SubscriptionPlan)
            .FirstOrDefaultAsync();

        return activeSubscription;
    }
}
