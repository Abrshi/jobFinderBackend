using jobFinderBackend.Application.Interfaces;
using MediatR;

namespace jobFinderBackend.Application.Profile.Queries.GetMySkills;

public record GetMySkillsQuery : IRequest<List<int>>;

public class GetMySkillsQueryHandler
    : IRequestHandler<GetMySkillsQuery, List<int>>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUserSkillRepository _userSkillRepository;

    public GetMySkillsQueryHandler(
        ICurrentUserService currentUser,
        IUserSkillRepository userSkillRepository)
    {
        _currentUser = currentUser;
        _userSkillRepository = userSkillRepository;
    }

    public async Task<List<int>> Handle(
        GetMySkillsQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new UnauthorizedAccessException();
        }

        var skillIds = await _userSkillRepository.GetUserSkillIdsAsync(_currentUser.UserId.Value);

        return skillIds;
    }
}