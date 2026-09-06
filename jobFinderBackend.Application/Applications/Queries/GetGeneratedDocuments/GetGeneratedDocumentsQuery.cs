using jobFinderBackend.Application.Applications.DTOs;
using jobFinderBackend.Application.Interfaces;
using MediatR;

namespace jobFinderBackend.Application.Applications.Queries.GetGeneratedDocuments;

public record GetGeneratedDocumentsQuery(int JobId)
    : IRequest<GeneratedDocumentsResponse?>;

public class GetGeneratedDocumentsQueryHandler
    : IRequestHandler<GetGeneratedDocumentsQuery, GeneratedDocumentsResponse?>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IGeneratedDocumentRepository _repository;

    public GetGeneratedDocumentsQueryHandler(
        ICurrentUserService currentUser,
        IGeneratedDocumentRepository repository)
    {
        _currentUser = currentUser;
        _repository = repository;
    }

    public Task<GeneratedDocumentsResponse?> Handle(
        GetGeneratedDocumentsQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not int userId)
        {
            throw new UnauthorizedAccessException();
        }

        return _repository.GetLatestAsync(userId, request.JobId, cancellationToken);
    }
}