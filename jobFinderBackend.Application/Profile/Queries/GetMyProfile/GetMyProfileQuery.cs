using jobFinderBackend.Application.Interfaces;
using jobFinderBackend.Application.Profile.DTOs;
using MediatR;

namespace jobFinderBackend.Application.Profile.Queries.GetMyProfile;

public record GetMyProfileQuery : IRequest<ProfileResponse>;

public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, ProfileResponse>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUserProfileRepository _profileRepository;

    public GetMyProfileQueryHandler(
        ICurrentUserService currentUser,
        IUserProfileRepository profileRepository)
    {
        _currentUser = currentUser;
        _profileRepository = profileRepository;
    }

    public Task<ProfileResponse> Handle(
        GetMyProfileQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not int userId)
        {
            throw new UnauthorizedAccessException();
        }

        return _profileRepository.GetProfileAsync(userId, cancellationToken);
    }
}