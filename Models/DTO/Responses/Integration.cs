using System;

namespace TicketingSystem.Models.DTO.Responses.Integrations
{
    public class IntegrationResponse
    {
        public Ulid Id { get; set; }
        public string Name { get; set; }
        public string? ApiKey { get; set; }
        public Ulid AgencyId { get; set; }
        public bool Enabled { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
