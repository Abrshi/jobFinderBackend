using jobFinder.Domain.Entities;

namespace jobFinder.Application.Interfaces;

public interface IUserRepository
{
    Task<Users?> GetByEmailAsync(string email);

    Task AddAsync(Users user);

    Task<bool> ExistsByEmailAsync(string email);
}