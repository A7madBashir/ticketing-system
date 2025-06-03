using System;

namespace TicketingSystem.Models.DTO.Requests.Agency
{
    public class CreateAnalyticRequest
    {
        public Ulid AgentId { get; set; }
        public Ulid AgencyId { get; set; }
        public int TicketsResolved { get; set; }
        public TimeSpan AverageResponseTime { get; set; }
        public float CustomerSatisfactionScore { get; set; }
    }

    public class UpdateAnalyticRequest
    {
        public Ulid Id { get; set; }
        public Ulid AgentId { get; set; }
        public Ulid AgencyId { get; set; }
        public int TicketsResolved { get; set; }
        public TimeSpan AverageResponseTime { get; set; }
        public float CustomerSatisfactionScore { get; set; }
    }
}
