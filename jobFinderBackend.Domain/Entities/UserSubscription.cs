namespace jobFinder.Domain.Entities;

public class UserSubscription
{
    public int Id { get; set; }


    public int UserId { get; set; }


    public int SubscriptionPlanId { get; set; }



    public DateTime StartDate { get; set; }


    public DateTime? EndDate { get; set; }



    public bool IsActive { get; set; }



    // Navigation


    public Users User { get; set; } = null!;


    public SubscriptionPlans SubscriptionPlan { get; set; } = null!;
}