using TicketingSystem.Models.Common.BaseEntity;
using TicketingSystem.Models.DTO.Responses.Agency;
using TicketingSystem.Models.DTO.Responses.User;

namespace TicketingSystem.Models.DTO.Responses.Agency
{
    public class AnalyticResponse : BaseResponse
    {
        public Ulid Id { get; set; }
        public Ulid AgentId { get; set; }
        public Ulid AgencyId { get; set; }
        public int TicketsResolved { get; set; }
        public TimeSpan AverageResponseTime { get; set; }
        public float CustomerSatisfactionScore { get; set; }
        public DateTime CreateTime { get; set; }
        public virtual AgencyResponse? Agency { get; set; }
        public virtual UserResponse? Agent { get; set; }
    }
}
