using System;
using System.Text.Json.Serialization;

namespace TicketingSystem.Models.DTO.Requests.Subscriptions
{
    public class CreateSubscriptionRequest : ICreateRequest
    {
        public required string PlanName { get; set; }
        public decimal Price { get; set; }
        public string? Features { get; set; }
    }

    public class UpdateSubscriptionRequest : IEditRequest<Ulid>
    {
        [JsonIgnore]
        public Ulid Id { get; set; }
        public required string PlanName { get; set; }
        public decimal Price { get; set; }
        public string? Features { get; set; }
    }
}
