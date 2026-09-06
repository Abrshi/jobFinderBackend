using jobFinder.Domain.Entities;
using jobFinderBackend.Infrastructure.Data;
using jobFinderBackend.Infrastructure.Jobd;
using Microsoft.EntityFrameworkCore;

namespace jobFinderBackend.Infrastructure.Persistence.Configuration.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(
        JobFinderBackendDbContext context)
    {
        // ==============================
        // Seed Subscription Plans
        // ==============================

        if (!await context.SubscriptionPlans.AnyAsync())
        {
            var subscriptionPlans = new List<SubscriptionPlans>
            {
                new SubscriptionPlans
                {
                    Name = "Free",
                    Price = 0,
                    MaxJobSources = 5,
                    CanGenerateCV = false,
                    CanGenerateCoverLetter = false,
                    CanUseInterviewAI = false
                },

                new SubscriptionPlans
                {
                    Name = "Paid",
                    Price = 0,
                    MaxJobSources = 7,
                    CanGenerateCV = true,
                    CanGenerateCoverLetter = true,
                    CanUseInterviewAI = true
                },

                new SubscriptionPlans
                {
                    Name = "Pro",
                    Price = 0,
                    MaxJobSources = 10,
                    CanGenerateCV = true,
                    CanGenerateCoverLetter = true,
                    CanUseInterviewAI = true
                }
            };

            await context.SubscriptionPlans.AddRangeAsync(
                subscriptionPlans);

            await context.SaveChangesAsync();
        }


        // ==============================
        // Seed Roles
        // ==============================

        if (!await context.Roles.AnyAsync())
        {
            var roles = new List<Role>
            {
                new Role
                {
                    Name = "JOB_SEEKER"
                },

                new Role
                {
                    Name = "ADMIN"
                }
            };

            await context.Roles.AddRangeAsync(roles);

            await context.SaveChangesAsync();
        }


        // ==============================
        // Seed & Re-categorize Skills
        // ==============================

        var junkSkills = await context.Skills
            .Where(s => s.Name == "Select" || s.Name == "—" || s.Name.Length < 2 || s.Name.Length > 50 || s.Name.Contains("<") || s.Name.Contains(">") || s.Name.Contains("http://") || s.Name.Contains("https://"))
            .ToListAsync();

        if (junkSkills.Count > 0)
        {
            var junkSkillIds = junkSkills.Select(s => s.Id).ToList();
            var userSkillLinks = await context.UserSkills.Where(us => junkSkillIds.Contains(us.SkillId)).ToListAsync();
            if (userSkillLinks.Count > 0) context.UserSkills.RemoveRange(userSkillLinks);

            var jobSkillLinks = await context.JobSkills.Where(js => junkSkillIds.Contains(js.SkillId)).ToListAsync();
            if (jobSkillLinks.Count > 0) context.JobSkills.RemoveRange(jobSkillLinks);

            context.Skills.RemoveRange(junkSkills);
            await context.SaveChangesAsync();
        }

        // Re-categorize any existing skills to ensure no skills remain under "General" or "Other"
        var allSkills = await context.Skills.ToListAsync();
        bool updatedAny = false;

        foreach (var skill in allSkills)
        {
            var resolvedCategory = SkillCategoryResolver.ResolveCategory(skill.Name);
            if (skill.Category != resolvedCategory)
            {
                skill.Category = resolvedCategory;
                updatedAny = true;
            }
        }

        if (updatedAny)
        {
            await context.SaveChangesAsync();
        }
    

// ==============================
// Seed Job Platforms
// ==============================

if (!await context.JobPlatforms.AnyAsync())
{
    var jobPlatforms = new List<JobPlatform>
    {
        // ==============================
        // Major Job Platforms
        // ==============================

        new JobPlatform
        {
            Name = "LinkedIn",
            Website = "https://www.linkedin.com/jobs",
            Logo = "linkedin",
            SourceType = "Scraper",
            IsActive = true
        },

        new JobPlatform
        {
            Name = "Indeed",
            Website = "https://www.indeed.com",
            Logo = "indeed",
            SourceType = "Scraper",
            IsActive = true
        },

        new JobPlatform
        {
            Name = "Glassdoor",
            Website = "https://www.glassdoor.com",
            Logo = "glassdoor",
            SourceType = "Scraper",
            IsActive = true
        },

        // ==============================
        // Freelance Platforms
        // ==============================

        new JobPlatform
        {
            Name = "Upwork",
            Website = "https://www.upwork.com",
            Logo = "upwork",
            SourceType = "API",
            IsActive = true
        },

        new JobPlatform
        {
            Name = "Fiverr",
            Website = "https://www.fiverr.com",
            Logo = "fiverr",
            SourceType = "Scraper",
            IsActive = true
        },

        new JobPlatform
        {
            Name = "Freelancer",
            Website = "https://www.freelancer.com",
            Logo = "freelancer",
            SourceType = "Scraper",
            IsActive = true
        },

        // ==============================
        // Developer-Focused Platforms
        // ==============================

        new JobPlatform
        {
            Name = "GitHub Jobs",
            Website = "https://github.com",
            Logo = "github",
            SourceType = "Manual",
            IsActive = false
        },

        new JobPlatform
        {
            Name = "Wellfound",
            Website = "https://wellfound.com",
            Logo = "wellfound",
            SourceType = "Scraper",
            IsActive = true
        },

        new JobPlatform
        {
            Name = "Stack Overflow Jobs",
            Website = "https://stackoverflow.com/jobs",
            Logo = "stackoverflow",
            SourceType = "Manual",
            IsActive = false
        },

        new JobPlatform
        {
            Name = "Dice",
            Website = "https://www.dice.com",
            Logo = "dice",
            SourceType = "Scraper",
            IsActive = true
        },

        // ==============================
        // Remote Job Platforms
        // ==============================

        new JobPlatform
        {
            Name = "Remote OK",
            Website = "https://remoteok.com",
            Logo = "remoteok",
            SourceType = "API",
            IsActive = true
        },

        new JobPlatform
        {
            Name = "We Work Remotely",
            Website = "https://weworkremotely.com",
            Logo = "weworkremotely",
            SourceType = "RSS",
            IsActive = true
        },

        new JobPlatform
        {
            Name = "Remotive",
            Website = "https://remotive.com",
            Logo = "remotive",
            SourceType = "API",
            IsActive = true
        },

        new JobPlatform
        {
            Name = "Remote.co",
            Website = "https://remote.co",
            Logo = "remote-co",
            SourceType = "Scraper",
            IsActive = true
        },

        new JobPlatform
        {
            Name = "FlexJobs",
            Website = "https://www.flexjobs.com",
            Logo = "flexjobs",
            SourceType = "Scraper",
            IsActive = true
        },

        // ==============================
        // Tech Job Boards
        // ==============================

        new JobPlatform
        {
            Name = "Hacker News Jobs",
            Website = "https://news.ycombinator.com/jobs",
            Logo = "hacker-news",
            SourceType = "Scraper",
            IsActive = true
        },

        new JobPlatform
        {
            Name = "Arc",
            Website = "https://arc.dev",
            Logo = "arc",
            SourceType = "Scraper",
            IsActive = true
        },

        new JobPlatform
        {
            Name = "Landing.Jobs",
            Website = "https://landing.jobs",
            Logo = "landing-jobs",
            SourceType = "Scraper",
            IsActive = true
        },

        // ==============================
        // General Job Boards
        // ==============================

        new JobPlatform
        {
            Name = "Monster",
            Website = "https://www.monster.com",
            Logo = "monster",
            SourceType = "Scraper",
            IsActive = true
        },

        new JobPlatform
        {
            Name = "ZipRecruiter",
            Website = "https://www.ziprecruiter.com",
            Logo = "ziprecruiter",
            SourceType = "Scraper",
            IsActive = true
        },

        // ==============================
        // Manual / Custom Source
        // ==============================

        new JobPlatform
        {
            Name = "JobFinder Manual",
            Website = null,
            Logo = "jobfinder",
            SourceType = "Manual",
            IsActive = true
        }
    };

    await context.JobPlatforms.AddRangeAsync(jobPlatforms);

    await context.SaveChangesAsync();
}
}
}