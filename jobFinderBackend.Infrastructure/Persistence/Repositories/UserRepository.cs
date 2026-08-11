using jobFinder.Application.Interfaces;
using jobFinder.Domain.Entities;
using jobFinderBackend.Application.Interfaces;
using jobFinderBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace jobFinder.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly JobFinderBackendDbContext _context;

    public UserRepository(JobFinderBackendDbContext context)
    {
        _context = context;
    }

    public async Task<Users?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Users
            .AnyAsync(u => u.Email == email);
    }

    public async Task AddAsync(Users user)
    {
        await _context.Users.AddAsync(user);

        await _context.SaveChangesAsync();
    }
}