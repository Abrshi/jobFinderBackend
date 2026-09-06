using UserSubscriptionEntity = jobFinder.Domain.Entities.UserSubscription;

namespace jobFinderBackend.Application.Interfaces;

public interface IUserSubscriptionRepository
{
    Task<UserSubscriptionEntity?> GetActiveSubscriptionByUserIdAsync(int userId);
}
