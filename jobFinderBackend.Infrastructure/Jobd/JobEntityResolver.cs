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

        var localTracked = _db.JobCategories.Local
            .FirstOrDefault(c => c.Name.Equals(normalized, StringComparison.OrdinalIgnoreCase));

        if (localTracked != null)
        {
            return localTracked;
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

        var localTracked = _db.Companies.Local
            .FirstOrDefault(c => c.Name.Equals(normalized, StringComparison.OrdinalIgnoreCase));

        if (localTracked != null)
        {
            if (string.IsNullOrWhiteSpace(localTracked.Industry)
                && !string.IsNullOrWhiteSpace(industry))
            {
                localTracked.Industry = industry.Trim();
            }

            if (string.IsNullOrWhiteSpace(localTracked.Country)
                && !string.IsNullOrWhiteSpace(country))
            {
                localTracked.Country = country.Trim();
            }

            return localTracked;
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
        return SkillCategoryResolver.ResolveCategory(skillName);
    }
}
