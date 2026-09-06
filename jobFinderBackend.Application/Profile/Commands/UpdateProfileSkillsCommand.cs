using jobFinderBackend.Application.Interfaces;
using jobFinderBackend.Application.Profile.DTOs;
using MediatR;

namespace jobFinderBackend.Application.Profile.Commands.UpdateProfileSkills;

public record UpdateProfileSkillsCommand(
    int UserId,
    List<UserSkillSelectionDto> Skills
) : IRequest;

public class UpdateProfileSkillsCommandHandler
    : IRequestHandler<UpdateProfileSkillsCommand>
{
    private readonly ISkillRepository _skillRepository;
    private readonly IUserSkillRepository _userSkillRepository;

    public UpdateProfileSkillsCommandHandler(
        ISkillRepository skillRepository,
        IUserSkillRepository userSkillRepository)
    {
        _skillRepository = skillRepository;
        _userSkillRepository = userSkillRepository;
    }

    public async Task Handle(
        UpdateProfileSkillsCommand request,
        CancellationToken cancellationToken)
    {
        var selectedSkillIds = request.Skills
            .Select(s => s.SkillId)
            .Distinct()
            .ToList();

        if (selectedSkillIds.Count == 0)
        {
            await _userSkillRepository.ReplaceForUserAsync(request.UserId, []);
            return;
        }

        var validSkillIds = (await _skillRepository.GetAllAsync())
            .Select(s => s.Id)
            .ToHashSet();

        var invalidSkillIds = selectedSkillIds
            .Except(validSkillIds)
            .ToList();

        if (invalidSkillIds.Count > 0)
        {
            throw new InvalidOperationException(
                "One or more selected skills are invalid."
            );
        }

        await _userSkillRepository.ReplaceForUserAsync(
            request.UserId,
            request.Skills
        );
    }
}
