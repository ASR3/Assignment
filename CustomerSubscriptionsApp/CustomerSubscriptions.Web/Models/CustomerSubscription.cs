using System.ComponentModel.DataAnnotations;

namespace CustomerSubscriptions.Web.Models;

public class CustomerSubscription
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string CustomerId { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string SubscriptionName { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int SubscriptionCount { get; set; }

    [DataType(DataType.Date)]
    public DateOnly? StartDate { get; set; }

    [DataType(DataType.Date)]
    public DateOnly? EndDate { get; set; }

    public bool IsActive { get; set; } = true;
}

