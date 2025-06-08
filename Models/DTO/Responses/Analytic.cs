using System;

namespace TicketingSystem.Models.DTO.Responses.Agency
{
    public class AnalyticResponse
    {
        public Ulid Id { get; set; }
        public Ulid AgentId { get; set; }
        public Ulid AgencyId { get; set; }
        public int TicketsResolved { get; set; }
        public TimeSpan AverageResponseTime { get; set; }
        public float CustomerSatisfactionScore { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
