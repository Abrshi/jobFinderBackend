using UserEntity = jobFinder.Domain.Entities.Users;
using RoleEntity = jobFinder.Domain.Entities.Role;
using SubscriptionPlanEntity = jobFinder.Domain.Entities.SubscriptionPlans;

namespace jobFinderBackend.Application.Interfaces;

public interface IUserRepository
{
    Task<UserEntity?> GetByIdAsync(int id);

    Task<UserEntity?> GetByEmailAsync(string email);

    Task<RoleEntity?> GetRoleByUserIdAsync(int userId);

    Task<bool> ExistsByEmailAsync(string email);

    Task AddAsync(UserEntity user);

    Task<SubscriptionPlanEntity?> GetActiveSubscriptionPlanAsync(int userId);
}
