using UserEntity = jobFinder.Domain.Entities.Users;
using RoleEntity = jobFinder.Domain.Entities.Role;

namespace jobFinderBackend.Application.Interfaces;

public interface IUserRepository
{
    Task<UserEntity?> GetByIdAsync(int id);

    Task<UserEntity?> GetByEmailAsync(string email);

    Task<RoleEntity?> GetRoleByUserIdAsync(int userId);

    Task<bool> ExistsByEmailAsync(string email);

    Task AddAsync(UserEntity user);
}
