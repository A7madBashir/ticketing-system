using System;

namespace TicketingSystem.Models.DTO.Requests.Agency
{
    public class CreateAnalyticRequest : ICreateRequest
    {
        public string AgentId { get; set; }
        public string AgencyId { get; set; }
        public int TicketsResolved { get; set; }
        public TimeSpan AverageResponseTime { get; set; }
        public float CustomerSatisfactionScore { get; set; }
    }

    public class UpdateAnalyticRequest : IEditRequest<Ulid>
    {
        public Ulid Id { get; set; }
        public string AgentId { get; set; }
        public string AgencyId { get; set; }
        public int TicketsResolved { get; set; }
        public TimeSpan AverageResponseTime { get; set; }
        public float CustomerSatisfactionScore { get; set; }
    }
}
