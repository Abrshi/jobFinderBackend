using jobFinder.Domain.Entities;
using jobFinderBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace jobFinderBackend.Infrastructure.Persistence.Configuration.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(JobFinderBackendDbContext context)
    {
        // Check if subscription plans already exist
        if (await context.SubscriptionPlans.AnyAsync())
        {
            return;
        }

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

        await context.SubscriptionPlans.AddRangeAsync(subscriptionPlans);

        await context.SaveChangesAsync();
    }
}