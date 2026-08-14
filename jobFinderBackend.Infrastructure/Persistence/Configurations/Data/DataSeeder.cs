using jobFinder.Domain.Entities;
using jobFinderBackend.Infrastructure.Data;
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
        // Seed Skills
        // ==============================

        if (!await context.Skills.AnyAsync())
        {
            var skills = new List<Skill>
            {
                // Programming Languages
                new Skill { Name = "C#", Category = "Programming Languages" },
                new Skill { Name = "Java", Category = "Programming Languages" },
                new Skill { Name = "JavaScript", Category = "Programming Languages" },
                new Skill { Name = "TypeScript", Category = "Programming Languages" },
                new Skill { Name = "Python", Category = "Programming Languages" },
                new Skill { Name = "PHP", Category = "Programming Languages" },
                new Skill { Name = "C++", Category = "Programming Languages" },
                new Skill { Name = "Go", Category = "Programming Languages" },
                new Skill { Name = "Rust", Category = "Programming Languages" },

                // Frontend
                new Skill { Name = "HTML", Category = "Frontend" },
                new Skill { Name = "CSS", Category = "Frontend" },
                new Skill { Name = "React", Category = "Frontend" },
                new Skill { Name = "Angular", Category = "Frontend" },
                new Skill { Name = "Vue.js", Category = "Frontend" },
                new Skill { Name = "Next.js", Category = "Frontend" },
                new Skill { Name = "Tailwind CSS", Category = "Frontend" },

                // Backend
                new Skill { Name = "ASP.NET Core", Category = "Backend" },
                new Skill { Name = "Node.js", Category = "Backend" },
                new Skill { Name = "Express.js", Category = "Backend" },
                new Skill { Name = "Spring Boot", Category = "Backend" },
                new Skill { Name = "Laravel", Category = "Backend" },
                new Skill { Name = "Django", Category = "Backend" },
                new Skill { Name = "REST API", Category = "Backend" },
                new Skill { Name = "Entity Framework Core", Category = "Backend" },

                // Database
                new Skill { Name = "PostgreSQL", Category = "Database" },
                new Skill { Name = "MySQL", Category = "Database" },
                new Skill { Name = "SQL Server", Category = "Database" },
                new Skill { Name = "MongoDB", Category = "Database" },
                new Skill { Name = "Redis", Category = "Database" },

                // DevOps & Cloud
                new Skill { Name = "Git", Category = "DevOps & Cloud" },
                new Skill { Name = "GitHub", Category = "DevOps & Cloud" },
                new Skill { Name = "Docker", Category = "DevOps & Cloud" },
                new Skill { Name = "Kubernetes", Category = "DevOps & Cloud" },
                new Skill { Name = "AWS", Category = "DevOps & Cloud" },
                new Skill { Name = "Azure", Category = "DevOps & Cloud" },
                new Skill { Name = "Google Cloud", Category = "DevOps & Cloud" },
                new Skill { Name = "CI/CD", Category = "DevOps & Cloud" },

                // Mobile
                new Skill { Name = "Flutter", Category = "Mobile" },
                new Skill { Name = "React Native", Category = "Mobile" },
                new Skill { Name = "Android", Category = "Mobile" },
                new Skill { Name = "iOS", Category = "Mobile" },

                // Testing
                new Skill { Name = "Selenium", Category = "Testing" },
                new Skill { Name = "Jest", Category = "Testing" },
                new Skill { Name = "xUnit", Category = "Testing" },
                new Skill { Name = "Cypress", Category = "Testing" },

                // AI & Data
                new Skill { Name = "Machine Learning", Category = "AI & Data" },
                new Skill { Name = "Data Analysis", Category = "AI & Data" },
                new Skill { Name = "TensorFlow", Category = "AI & Data" },
                new Skill { Name = "PyTorch", Category = "AI & Data" },
                new Skill { Name = "Pandas", Category = "AI & Data" },

                // UI/UX
                new Skill { Name = "Figma", Category = "UI/UX" },
                new Skill { Name = "UI Design", Category = "UI/UX" },
                new Skill { Name = "UX Design", Category = "UI/UX" },

                // Security
                new Skill { Name = "Cybersecurity", Category = "Security" },
                new Skill { Name = "OAuth 2.0", Category = "Security" },
                new Skill { Name = "JWT", Category = "Security" }
            };

            await context.Skills.AddRangeAsync(skills);

            await context.SaveChangesAsync();
        }
    }
}