namespace jobFinderBackend.Application.Applications.DTOs;

public class GeneratedDocumentsResponse
{
    public int JobId { get; set; }

    public GeneratedCvResponse Cv { get; set; } = null!;

    public GeneratedCoverLetterResponse CoverLetter { get; set; } = null!;
}

public class GeneratedCvResponse
{
    public int Id { get; set; }

    public string VersionName { get; set; } = string.Empty;

    public CvContent Content { get; set; } = null!;

    public DateTime GeneratedAt { get; set; }
}

public class GeneratedCoverLetterResponse
{
    public int Id { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTime GeneratedAt { get; set; }
}