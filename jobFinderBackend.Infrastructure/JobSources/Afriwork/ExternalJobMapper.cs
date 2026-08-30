using System.Net;
using System.Text.RegularExpressions;
using jobFinder.Application.Jobs.DTOs;

namespace jobFinder.Infrastructure.JobSources.Afriwork;

public static class AfriworkJobMapper
{
    public static ExternalJobDto ToExternalJobDto(AfriworkJob job)
    {
        return new ExternalJobDto
        {
            ExternalId = job.Id,

            Title = CleanText(job.Title),

            Description = CleanHtml(job.Description),

            EmploymentType = job.JobType,

            ExperienceLevel = job.ExperienceLevel,

            Country = job.City?.Country?.Name,

            City = job.City?.Name,

            RemoteType = job.JobSite,

            PostedDate = ToUtc(job.PublishedAt ?? job.CreatedAt),

            ExpiryDate = ToUtc(job.Deadline),

            CompanyName = job.Entity?.Name,

            Skills = job.SkillRequirements
                .Where(x => x.Skill?.Name != null)
                .Select(x => x.Skill!.Name!)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList(),

            SalaryMin = ConvertSalary(
                job.CompensationAmountCents),

            SalaryMax = ConvertSalary(
                job.CompensationAmountCents),

            SalaryCurrency = job.CompensationCurrency,

            OriginalUrl = null
        };
    }

    private static DateTime? ToUtc(DateTime? value)
    {
        if (!value.HasValue)
            return null;

        return value.Value.Kind switch
        {
            DateTimeKind.Utc => value.Value,
            DateTimeKind.Local => value.Value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)
        };
    }

    private static decimal? ConvertSalary(decimal? amountCents)
    {
        if (amountCents == null)
            return null;

        return amountCents.Value / 100m;
    }

    private static string CleanText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        return WebUtility.HtmlDecode(value).Trim();
    }

    private static string CleanHtml(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return string.Empty;

        var decoded = WebUtility.HtmlDecode(html);

        var text = Regex.Replace(
            decoded,
            "<[^>]*>",
            " ");

        return Regex.Replace(
                text,
                @"\s+",
                " ")
            .Trim();
    }
}