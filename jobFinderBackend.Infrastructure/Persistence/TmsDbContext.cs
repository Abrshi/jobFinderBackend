using Microsoft.EntityFrameworkCore;
using jobFinder.Domain.Entities;
using ApplicationEntity = jobFinder.Domain.Entities.Application;

namespace jobFinderBackend.Infrastructure.Data;

public class JobFinderBackendDbContext(DbContextOptions<JobFinderBackendDbContext> options) : DbContext(options)
{
    public DbSet<AIUsageLog> AIUsageLogs => Set<AIUsageLog>();
    public DbSet<ApplicationEntity> Applications => Set<ApplicationEntity>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Education> Educations => Set<Education>();
    public DbSet<Experience> Experiences => Set<Experience>();
    public DbSet<GeneratedCoverLetter> GeneratedCoverLetters => Set<GeneratedCoverLetter>();
    public DbSet<GeneratedCV> GeneratedCVs => Set<GeneratedCV>();
    public DbSet<InterviewQuestion> InterviewQuestions => Set<InterviewQuestion>();
    public DbSet<InterviewSession> InterviewSessions => Set<InterviewSession>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<JobCategory> JobCategories => Set<JobCategory>();
    public DbSet<JobPlatform> JobPlatforms => Set<JobPlatform>();
    public DbSet<JobRecommendation> JobRecommendations => Set<JobRecommendation>();
    public DbSet<JobSkill> JobSkills => Set<JobSkill>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<SavedJob> SavedJobs => Set<SavedJob>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<SubscriptionPlans> SubscriptionPlans => Set<SubscriptionPlans>();
    public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<UserJobSource> UserJobSources => Set<UserJobSource>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Users> Users => Set<Users>();
    public DbSet<UserSkill> UserSkills => Set<UserSkill>();
    public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();
}