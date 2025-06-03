using System;

namespace TicketingSystem.Models.DTO.Requests.Integrations
{
    public class CreateIntegrationRequest
    {
        public required string Name { get; set; }
        public string? ApiKey { get; set; }
        public Ulid AgencyId { get; set; }
        public bool Enabled { get; set; }
    }

    public class UpdateIntegrationRequest
    {
        public required Ulid Id { get; set; }
        public required string Name { get; set; }
        public string? ApiKey { get; set; }
        public Ulid AgencyId { get; set; }
        public bool Enabled { get; set; }
    }
}
