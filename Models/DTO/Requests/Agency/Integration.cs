using System;

namespace TicketingSystem.Models.DTO.Requests.Integrations
{
    public class CreateIntegrationRequest : ICreateRequest
    {
        public required string Name { get; set; }
        public string? ApiKey { get; set; }
        public string AgencyId { get; set; }
        public bool Enabled { get; set; }
    }

    public class UpdateIntegrationRequest : IEditRequest<Ulid>
    {
        public required Ulid Id { get; set; }
        public required string Name { get; set; }
        public string? ApiKey { get; set; }
        public string AgencyId { get; set; }
        public bool Enabled { get; set; }
    }
}
