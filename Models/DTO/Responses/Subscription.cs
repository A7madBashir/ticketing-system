using System;

namespace TicketingSystem.Models.DTO.Responses.Subscriptions;

public class SubscriptionResponse : BaseResponse
{
    public string PlanName { get; set; }
    public decimal Price { get; set; }
    public string? Features { get; set; }
}
