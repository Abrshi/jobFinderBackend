using jobFinderBackend.Application.Applications.DTOs;

namespace jobFinderBackend.Application.Interfaces;

public interface IGenerativeDocumentService
{
    Task<GeneratedDocumentContent> GenerateAsync(
        DocumentGenerationData data,
        CancellationToken cancellationToken = default);
}

public class GeneratedDocumentContent
{
    public CvContent Cv { get; set; } = null!;

    public string CoverLetter { get; set; } = string.Empty;
}