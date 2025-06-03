using System;

namespace TicketingSystem.Models.DTO.Requests.Subscriptions
{
    public class CreateSubscriptionRequest
    {
        public required string PlanName { get; set; }
        public decimal Price { get; set; }
        public string? Features { get; set; }
    }

    public class UpdateSubscriptionRequest
    {
        public required Ulid Id { get; set; }
        public required string PlanName { get; set; }
        public decimal Price { get; set; }
        public string? Features { get; set; }
    }
}
