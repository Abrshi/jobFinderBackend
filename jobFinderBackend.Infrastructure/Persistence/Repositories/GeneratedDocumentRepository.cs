using System.Text.Json;
using jobFinder.Domain.Entities;
using jobFinderBackend.Application.Applications.DTOs;
using jobFinderBackend.Application.Interfaces;
using jobFinderBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace jobFinderBackend.Infrastructure.Persistence.Repositories;

public class GeneratedDocumentRepository : IGeneratedDocumentRepository
{
    private readonly JobFinderBackendDbContext _context;

    public GeneratedDocumentRepository(JobFinderBackendDbContext context)
    {
        _context = context;
    }

    public async Task<DocumentGenerationData?> GetGenerationDataAsync(
        int userId,
        int jobId,
        CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Include(item => item.UserProfile)
            .Include(item => item.Educations)
            .Include(item => item.Experiences)
            .Include(item => item.UserSkills)
                .ThenInclude(item => item.Skill)
            .FirstOrDefaultAsync(item => item.Id == userId, cancellationToken);

        var job = await _context.Jobs
            .AsNoTracking()
            .Include(item => item.Company)
            .Include(item => item.Category)
            .Include(item => item.JobSkills)
                .ThenInclude(item => item.Skill)
            .FirstOrDefaultAsync(item => item.Id == jobId, cancellationToken);

        if (user is null || job is null)
        {
            return null;
        }

        return new DocumentGenerationData
        {
            Candidate = new CandidateData
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Profile = user.UserProfile is null ? null : new CandidateProfileData
                {
                    Headline = user.UserProfile.Headline,
                    Summary = user.UserProfile.Summary,
                    CurrentJob = user.UserProfile.CurrentJob,
                    YearsOfExperience = user.UserProfile.YearsOfExperience
                },
                Education = user.Educations.Select(item => new CandidateEducationData
                {
                    Institution = item.Institution,
                    Degree = item.Degree,
                    FieldOfStudy = item.FieldOfStudy,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate
                }).ToList(),
                Experience = user.Experiences.Select(item => new CandidateExperienceData
                {
                    Company = item.Company,
                    Position = item.Position,
                    Description = item.Description,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    IsCurrent = item.IsCurrent
                }).ToList(),
                Skills = user.UserSkills
                    .Where(item => item.Skill != null)
                    .Select(item => new CandidateSkillData
                    {
                        Name = item.Skill.Name,
                        Level = item.Level,
                        YearsOfExperience = item.YearsOfExperience
                    }).ToList()
            },
            Job = new JobData
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                EmploymentType = job.EmploymentType,
                ExperienceLevel = job.ExperienceLevel,
                SalaryMin = job.SalaryMin,
                SalaryMax = job.SalaryMax,
                Country = job.Country,
                City = job.City,
                RemoteType = job.RemoteType,
                Company = job.Company?.Name,
                CompanyIndustry = job.Company?.Industry,
                Category = job.Category?.Name,
                Skills = job.JobSkills
                    .Where(item => item.Skill != null)
                    .Select(item => new JobSkillData
                    {
                        Name = item.Skill.Name,
                        Importance = item.Importance
                    }).ToList()
            }
        };
    }

    public async Task<GeneratedDocumentsResponse> SaveAsync(
        int userId,
        int jobId,
        CvContent cv,
        string coverLetter,
        CancellationToken cancellationToken = default)
    {
        var generatedAt = DateTime.UtcNow;
        var versionName = $"Generated {generatedAt:yyyy-MM-dd HH:mm:ss} UTC";
        var cvEntity = new GeneratedCV
        {
            UserId = userId,
            JobId = jobId,
            VersionName = versionName,
            Content = JsonSerializer.Serialize(cv),
            GeneratedAt = generatedAt
        };
        var coverLetterEntity = new GeneratedCoverLetter
        {
            UserId = userId,
            JobId = jobId,
            Content = coverLetter,
            GeneratedAt = generatedAt
        };

        await using var transaction = await _context.Database
            .BeginTransactionAsync(cancellationToken);

        await _context.GeneratedCVs.AddAsync(cvEntity, cancellationToken);
        await _context.GeneratedCoverLetters.AddAsync(coverLetterEntity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return ToResponse(cvEntity, coverLetterEntity, cv);
    }

    public async Task<GeneratedDocumentsResponse?> GetLatestAsync(
        int userId,
        int jobId,
        CancellationToken cancellationToken = default)
    {
        var cv = await _context.GeneratedCVs
            .AsNoTracking()
            .Where(item => item.UserId == userId && item.JobId == jobId)
            .OrderByDescending(item => item.GeneratedAt)
            .ThenByDescending(item => item.Id)
            .FirstOrDefaultAsync(cancellationToken);
        var coverLetter = await _context.GeneratedCoverLetters
            .AsNoTracking()
            .Where(item => item.UserId == userId && item.JobId == jobId)
            .OrderByDescending(item => item.GeneratedAt)
            .ThenByDescending(item => item.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (cv is null || coverLetter is null)
        {
            return null;
        }

        var content = JsonSerializer.Deserialize<CvContent>(cv.Content)
            ?? throw new InvalidOperationException("Stored CV content is invalid.");

        return ToResponse(cv, coverLetter, content);
    }

    private static GeneratedDocumentsResponse ToResponse(
        GeneratedCV cv,
        GeneratedCoverLetter coverLetter,
        CvContent content)
    {
        return new GeneratedDocumentsResponse
        {
            JobId = cv.JobId!.Value,
            Cv = new GeneratedCvResponse
            {
                Id = cv.Id,
                VersionName = cv.VersionName,
                Content = content,
                GeneratedAt = cv.GeneratedAt
            },
            CoverLetter = new GeneratedCoverLetterResponse
            {
                Id = coverLetter.Id,
                Content = coverLetter.Content,
                GeneratedAt = coverLetter.GeneratedAt
            }
        };
    }
}