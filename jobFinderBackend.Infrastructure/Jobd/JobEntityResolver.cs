using jobFinder.Application.Jobs.Classification;
using jobFinder.Application.Jobs.DTOs;
using jobFinder.Domain.Entities;
using jobFinderBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace jobFinderBackend.Infrastructure.Jobd;

public sealed class JobEntityResolver
{
    private readonly JobFinderBackendDbContext _db;
    private readonly ILogger<JobEntityResolver> _logger;

    public JobEntityResolver(
        JobFinderBackendDbContext db,
        ILogger<JobEntityResolver> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task ApplyClassificationAsync(
        Job job,
        ExternalJobDto externalJob,
        JobClassificationResult classification,
        CancellationToken cancellationToken)
    {
        if ((string.IsNullOrWhiteSpace(job.Title) || string.Equals(job.Title, "Untitled Position", StringComparison.OrdinalIgnoreCase))
            && !string.IsNullOrWhiteSpace(classification.Title))
        {
            job.Title = classification.Title.Trim();
        }

        var category = await GetOrCreateCategoryAsync(
            classification.Category,
            cancellationToken);

        job.CategoryId = category.Id;

        var companyName = PickValue(externalJob.CompanyName, classification.CompanyName);
        var country = PickValue(externalJob.Country, classification.Country);
        var city = PickValue(externalJob.City, classification.City);

        var company = await GetOrCreateCompanyAsync(
            companyName,
            country,
            classification.CompanyIndustry,
            cancellationToken);

        job.CompanyId = company?.Id;
        job.Country = country;
        job.City = city;

        job.EmploymentType =
            PickValue(externalJob.EmploymentType, classification.EmploymentType);

        job.ExperienceLevel =
            PickValue(
                externalJob.ExperienceLevel,
                classification.ExperienceLevel)
            ?? MapExperienceYears(classification.MinimumExperienceYears);

        job.RemoteType =
            PickValue(externalJob.RemoteType, classification.RemoteType);

        await SyncJobSkillsAsync(
            job,
            classification.Skills,
            cancellationToken);

        _logger.LogInformation(
            "Classified job '{Title}': Category={Category}, Company={Company}, Location={City}, {Country}, Skills={SkillCount}",
            job.Title,
            category.Name,
            company?.Name ?? "none",
            job.City ?? "unknown",
            job.Country ?? "unknown",
            classification.Skills.Count);
    }

    public async Task<JobCategory> GetOrCreateCategoryAsync(
        string categoryName,
        CancellationToken cancellationToken)
    {
        var normalized = string.IsNullOrWhiteSpace(categoryName)
            ? "Other"
            : categoryName.Trim();

        var existing = await _db.JobCategories
            .FirstOrDefaultAsync(
                c => c.Name.ToLower() == normalized.ToLower(),
                cancellationToken);

        if (existing != null)
        {
            return existing;
        }

        var category = new JobCategory
        {
            Name = normalized,
            ParentCategoryId = null
        };

        _db.JobCategories.Add(category);

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Created job category: {Category} (Id: {Id})",
            category.Name,
            category.Id);

        return category;
    }

    public async Task<Company?> GetOrCreateCompanyAsync(
        string? companyName,
        string? country,
        string? industry,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(companyName))
        {
            return null;
        }

        var normalized = companyName.Trim();

        var existing = await _db.Companies
            .FirstOrDefaultAsync(
                c => c.Name.ToLower() == normalized.ToLower(),
                cancellationToken);

        if (existing != null)
        {
            if (string.IsNullOrWhiteSpace(existing.Industry)
                && !string.IsNullOrWhiteSpace(industry))
            {
                existing.Industry = industry.Trim();
            }

            if (string.IsNullOrWhiteSpace(existing.Country)
                && !string.IsNullOrWhiteSpace(country))
            {
                existing.Country = country.Trim();
            }

            return existing;
        }

        var company = new Company
        {
            Name = normalized,
            Country = country?.Trim(),
            Industry = industry?.Trim()
        };

        _db.Companies.Add(company);

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Created company: {Company} (Id: {Id})",
            company.Name,
            company.Id);

        return company;
    }

    public async Task SyncJobSkillsAsync(
        Job job,
        IReadOnlyList<string> skillNames,
        CancellationToken cancellationToken)
    {
        if (skillNames.Count == 0)
        {
            return;
        }

        var normalizedNames = skillNames
            .Select(SanitizeSkillName)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (normalizedNames.Count == 0)
        {
            return;
        }

        var existingSkills = await _db.Skills
            .ToListAsync(cancellationToken);

        var skillsByName = existingSkills
            .ToDictionary(
                s => s.Name,
                StringComparer.OrdinalIgnoreCase);

        foreach (var skillName in normalizedNames)
        {
            if (!skillsByName.TryGetValue(skillName, out var skill))
            {
                skill = new Skill
                {
                    Name = skillName,
                    Category = InferSkillCategory(skillName)
                };

                _db.Skills.Add(skill);
                skillsByName[skillName] = skill;
            }

            var alreadyLinked = job.JobSkills.Any(js =>
                string.Equals(
                    js.Skill?.Name,
                    skillName,
                    StringComparison.OrdinalIgnoreCase)
                || (js.SkillId != 0
                    && skill.Id != 0
                    && js.SkillId == skill.Id));

            if (alreadyLinked)
            {
                continue;
            }

            job.JobSkills.Add(new JobSkill
            {
                Skill = skill,
                Importance = "Required"
            });
        }
    }

    private static string? SanitizeSkillName(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;
        var trimmed = raw.Trim();
        if (trimmed.Length < 2 || trimmed.Length > 50) return null;
        if (trimmed.Equals("Select", StringComparison.OrdinalIgnoreCase) || trimmed.Equals("—", StringComparison.OrdinalIgnoreCase)) return null;
        if (trimmed.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || trimmed.StartsWith("https://", StringComparison.OrdinalIgnoreCase)) return null;
        if (trimmed.Contains("<") || trimmed.Contains(">") || trimmed.Contains("\n") || trimmed.Contains("\r")) return null;

        // Clean up common prefixes/suffixes
        if (trimmed.StartsWith("- ") || trimmed.StartsWith("• "))
        {
            trimmed = trimmed[2..].Trim();
        }

        return trimmed;
    }

    private static string? PickValue(
        string? sourceValue,
        string? classifiedValue)
    {
        if (!string.IsNullOrWhiteSpace(classifiedValue))
        {
            return classifiedValue.Trim();
        }

        return string.IsNullOrWhiteSpace(sourceValue)
            ? null
            : sourceValue.Trim();
    }

    private static string? MapExperienceYears(int? years)
    {
        if (!years.HasValue)
        {
            return null;
        }

        return years.Value switch
        {
            <= 1 => "Entry Level",
            <= 3 => "Junior",
            <= 5 => "Mid-Level",
            <= 8 => "Senior",
            _ => "Executive"
        };
    }

    private static string InferSkillCategory(string skillName)
    {
        var lower = skillName.ToLower();

        if (lower.Contains("c#") || lower.Contains("java") || lower.Contains("python") || lower.Contains("typescript") || lower.Contains("javascript") || lower.Contains("c++") || lower.Contains("golang") || lower.Contains("rust") || lower.Contains("php"))
        {
            return "Programming Languages";
        }

        if (lower.Contains("react") || lower.Contains("angular") || lower.Contains("vue") || lower.Contains("html") || lower.Contains("css") || lower.Contains("tailwind") || lower.Contains("next"))
        {
            return "Frontend";
        }

        if (lower.Contains("node") || lower.Contains(".net") || lower.Contains("asp.net") || lower.Contains("express") || lower.Contains("spring") || lower.Contains("laravel") || lower.Contains("django") || lower.Contains("api"))
        {
            return "Backend";
        }

        if (lower.Contains("sql") || lower.Contains("postgres") || lower.Contains("mongo") || lower.Contains("redis") || lower.Contains("database"))
        {
            return "Database";
        }

        if (lower.Contains("docker") || lower.Contains("kubernetes") || lower.Contains("aws") || lower.Contains("azure") || lower.Contains("cloud") || lower.Contains("git") || lower.Contains("ci/cd") || lower.Contains("devops"))
        {
            return "DevOps & Cloud";
        }

        if (lower.Contains("flutter") || lower.Contains("react native") || lower.Contains("android") || lower.Contains("ios") || lower.Contains("mobile"))
        {
            return "Mobile";
        }

        if (lower.Contains("test") || lower.Contains("jest") || lower.Contains("cypress") || lower.Contains("qa") || lower.Contains("selenium") || lower.Contains("xunit"))
        {
            return "Testing";
        }

        if (lower.Contains("machine learning") || lower.Contains("ai") || lower.Contains("data") || lower.Contains("tensorflow") || lower.Contains("pytorch"))
        {
            return "AI & Data";
        }

        if (lower.Contains("design") || lower.Contains("figma") || lower.Contains("ux") || lower.Contains("ui"))
        {
            return "UI/UX";
        }

        if (lower.Contains("leader") || lower.Contains("manage") || lower.Contains("project") || lower.Contains("agile") || lower.Contains("scrum"))
        {
            return "Management & Leadership";
        }

        return "General";
    }
}
