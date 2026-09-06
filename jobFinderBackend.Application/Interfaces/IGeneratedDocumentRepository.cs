using jobFinderBackend.Application.Applications.DTOs;

namespace jobFinderBackend.Application.Interfaces;

public interface IGeneratedDocumentRepository
{
    Task<DocumentGenerationData?> GetGenerationDataAsync(
        int userId,
        int jobId,
        CancellationToken cancellationToken = default);

    Task<GeneratedDocumentsResponse> SaveAsync(
        int userId,
        int jobId,
        CvContent cv,
        string coverLetter,
        CancellationToken cancellationToken = default);

    Task<GeneratedDocumentsResponse?> GetLatestAsync(
        int userId,
        int jobId,
        CancellationToken cancellationToken = default);
}