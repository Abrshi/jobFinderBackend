using jobFinderBackend.Application.Interfaces;
using jobFinderBackend.Application.Profile.DTOs;
using MediatR;

namespace jobFinderBackend.Application.Profile.Queries.GetSkills;

public record GetSkillsQuery : IRequest<List<GetSkillsResponse>>;

public class GetSkillsQueryHandler
    : IRequestHandler<GetSkillsQuery, List<GetSkillsResponse>>
{
    private readonly ISkillRepository _skillRepository;

    public GetSkillsQueryHandler(ISkillRepository skillRepository)
    {
        _skillRepository = skillRepository;
    }

    public async Task<List<GetSkillsResponse>> Handle(
        GetSkillsQuery request,
        CancellationToken cancellationToken)
    {
        var skills = await _skillRepository.GetAllAsync();

        return skills
            .Select(s => new GetSkillsResponse
            {
                Id = s.Id,
                Name = s.Name,
                Category = s.Category
            })
            .ToList();
    }
}
