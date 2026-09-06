using jobFinder.Domain.Entities;
using jobFinderBackend.Application.Interfaces;
using jobFinderBackend.Application.Profile.DTOs;
using jobFinderBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace jobFinderBackend.Infrastructure.Persistence.Repositories;

public class UserProfileRepository : IUserProfileRepository
{
    private readonly JobFinderBackendDbContext _context;

    public UserProfileRepository(JobFinderBackendDbContext context)
    {
        _context = context;
    }

    public async Task<ProfileResponse> GetProfileAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        return new ProfileResponse
        {
            Education = await _context.Educations
                .AsNoTracking()
                .Where(education => education.UserId == userId)
                .OrderBy(education => education.Id)
                .Select(education => new EducationDto
                {
                    Id = education.Id,
                    Institution = education.Institution,
                    Degree = education.Degree,
                    FieldOfStudy = education.FieldOfStudy,
                    StartDate = education.StartDate,
                    EndDate = education.EndDate
                })
                .ToListAsync(cancellationToken),
            Experience = await _context.Experiences
                .AsNoTracking()
                .Where(experience => experience.UserId == userId)
                .OrderBy(experience => experience.Id)
                .Select(experience => new ExperienceDto
                {
                    Id = experience.Id,
                    Company = experience.Company,
                    Position = experience.Position,
                    Description = experience.Description,
                    StartDate = experience.StartDate,
                    EndDate = experience.EndDate,
                    IsCurrent = experience.IsCurrent
                })
                .ToListAsync(cancellationToken)
        };
    }

    public async Task UpdateProfileAsync(
        int userId,
        UpdateProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        var education = await _context.Educations
            .Where(item => item.UserId == userId)
            .ToListAsync(cancellationToken);
        var experience = await _context.Experiences
            .Where(item => item.UserId == userId)
            .ToListAsync(cancellationToken);

        var educationIds = request.Education
            .Where(item => item.Id.HasValue)
            .Select(item => item.Id!.Value)
            .ToList();
        var experienceIds = request.Experience
            .Where(item => item.Id.HasValue)
            .Select(item => item.Id!.Value)
            .ToList();

        if (educationIds.Count != educationIds.Distinct().Count() ||
            experienceIds.Count != experienceIds.Distinct().Count())
        {
            throw new InvalidOperationException("Profile record IDs must be unique.");
        }

        if (educationIds.Except(education.Select(item => item.Id)).Any() ||
            experienceIds.Except(experience.Select(item => item.Id)).Any())
        {
            throw new InvalidOperationException(
                "One or more profile records do not belong to the current user.");
        }

        await using var transaction = await _context.Database
            .BeginTransactionAsync(cancellationToken);

        _context.Educations.RemoveRange(
            education.Where(item => !educationIds.Contains(item.Id)));
        _context.Experiences.RemoveRange(
            experience.Where(item => !experienceIds.Contains(item.Id)));

        foreach (var item in request.Education)
        {
            var entity = item.Id is int id
                ? education.Single(existing => existing.Id == id)
                : new Education { UserId = userId };

            entity.Institution = item.Institution.Trim();
            entity.Degree = item.Degree.Trim();
            entity.FieldOfStudy = item.FieldOfStudy?.Trim();
            entity.StartDate = ToUtc(item.StartDate);
            entity.EndDate = ToUtc(item.EndDate);

            if (item.Id is null)
            {
                await _context.Educations.AddAsync(entity, cancellationToken);
            }
        }

        foreach (var item in request.Experience)
        {
            var entity = item.Id is int id
                ? experience.Single(existing => existing.Id == id)
                : new Experience { UserId = userId };

            entity.Company = item.Company?.Trim();
            entity.Position = item.Position.Trim();
            entity.Description = item.Description?.Trim();
            entity.StartDate = ToUtc(item.StartDate);
            entity.EndDate = ToUtc(item.EndDate);
            entity.IsCurrent = item.IsCurrent;

            if (item.Id is null)
            {
                await _context.Experiences.AddAsync(entity, cancellationToken);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private static DateTime? ToUtc(DateTime? value)
    {
        return value.HasValue ? ToUtc(value.Value) : null;
    }

    private static DateTime ToUtc(DateTime value)
    {
        return value.Kind == DateTimeKind.Utc
            ? value
            : value.Kind == DateTimeKind.Local
                ? value.ToUniversalTime()
                : DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }
}