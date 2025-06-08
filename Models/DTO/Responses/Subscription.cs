using System;

namespace TicketingSystem.Models.DTO.Responses.Subscriptions
{
    public class SubscriptionResponse
    {
        public Ulid Id { get; set; }
        public string PlanName { get; set; }
        public decimal Price { get; set; }
        public string? Features { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
