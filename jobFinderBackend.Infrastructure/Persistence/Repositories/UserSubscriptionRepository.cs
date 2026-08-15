using jobFinder.Domain.Entities;
using jobFinderBackend.Application.Interfaces;
using jobFinderBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace jobFinderBackend.Infrastructure.Persistence.Repositories;

public class UserSubscriptionRepository : IUserSubscriptionRepository
{
    private readonly JobFinderBackendDbContext _context;

    public UserSubscriptionRepository(JobFinderBackendDbContext context)
    {
        _context = context;
    }

    public async Task<UserSubscription?> GetActiveSubscriptionByUserIdAsync(int userId)
    {
        return await _context.UserSubscriptions
            .Include(us => us.SubscriptionPlan)
            .FirstOrDefaultAsync(us =>
                us.UserId == userId
                && us.IsActive
                && (us.EndDate == null || us.EndDate > DateTime.UtcNow)
            );
    }
}
