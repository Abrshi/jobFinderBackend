namespace jobFinder.Domain.Entities;

public class Application
{
    public int Id { get; set; }


    public int UserId { get; set; }


    public int JobId { get; set; }



    public int? GeneratedCVId { get; set; }


    public int? GeneratedCoverLetterId { get; set; }



    // Draft
    // Applied
    // Interview
    // Offer
    // Rejected

    public string Status { get; set; }
        = "Draft";



    public DateTime CreatedAt { get; set; }
        = DateTime.UtcNow;


    public DateTime? AppliedDate { get; set; }



    // Navigation


    public Users User { get; set; } = null!;


    public Job Job { get; set; } = null!;


    public GeneratedCV? GeneratedCV { get; set; }


    public GeneratedCoverLetter? GeneratedCoverLetter { get; set; }
}