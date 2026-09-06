using jobFinderBackend.Application.Profile.DTOs;

namespace jobFinderBackend.Application.Interfaces;

public interface IUserProfileRepository
{
    Task<ProfileResponse> GetProfileAsync(
        int userId,
        CancellationToken cancellationToken = default);

    Task UpdateProfileAsync(
        int userId,
        UpdateProfileRequest request,
        CancellationToken cancellationToken = default);
}