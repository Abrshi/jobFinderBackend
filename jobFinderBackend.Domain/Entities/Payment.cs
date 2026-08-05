namespace jobFinder.Domain.Entities;

public class Payment
{
    public int Id { get; set; }


    public int UserId { get; set; }


    public int SubscriptionPlanId { get; set; }



    public decimal Amount { get; set; }


    public string Currency { get; set; }
        = "USD";



    // Pending
    // Completed
    // Failed
    // Refunded

    public string Status { get; set; }
        = "Pending";



    public string? TransactionReference { get; set; }



    public DateTime CreatedAt { get; set; }
        = DateTime.UtcNow;


    public DateTime? PaidAt { get; set; }



    // Navigation


    public Users User { get; set; } = null!;


    public SubscriptionPlans SubscriptionPlan { get; set; } = null!;
}