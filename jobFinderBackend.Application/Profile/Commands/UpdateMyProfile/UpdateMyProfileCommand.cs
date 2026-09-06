using FluentValidation;
using jobFinderBackend.Application.Interfaces;
using jobFinderBackend.Application.Profile.DTOs;
using MediatR;

namespace jobFinderBackend.Application.Profile.Commands.UpdateMyProfile;

public record UpdateMyProfileCommand(UpdateProfileRequest Profile) : IRequest<ProfileResponse>;

public class UpdateMyProfileCommandHandler : IRequestHandler<UpdateMyProfileCommand, ProfileResponse>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUserProfileRepository _profileRepository;
    private readonly IValidator<UpdateProfileRequest> _validator;

    public UpdateMyProfileCommandHandler(
        ICurrentUserService currentUser,
        IUserProfileRepository profileRepository,
        IValidator<UpdateProfileRequest> validator)
    {
        _currentUser = currentUser;
        _profileRepository = profileRepository;
        _validator = validator;
    }

    public async Task<ProfileResponse> Handle(
        UpdateMyProfileCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not int userId)
        {
            throw new UnauthorizedAccessException();
        }

        await _validator.ValidateAndThrowAsync(request.Profile, cancellationToken);
        await _profileRepository.UpdateProfileAsync(userId, request.Profile, cancellationToken);

        return await _profileRepository.GetProfileAsync(userId, cancellationToken);
    }
}