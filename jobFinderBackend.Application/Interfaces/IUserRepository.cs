using UserEntity = jobFinder.Domain.Entities.Users;

namespace jobFinderBackend.Application.Interfaces;

public interface IUserRepository
{
    Task<UserEntity?> GetByEmailAsync(string email);

    Task<bool> ExistsByEmailAsync(string email);

    Task AddAsync(UserEntity user);
}