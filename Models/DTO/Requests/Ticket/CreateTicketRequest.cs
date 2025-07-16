using System.Text.Json.Serialization;

namespace TicketingSystem.Models.DTO.Requests.Ticket;

public class CreateTicketRequest : ICreateRequest
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public string? Priority { get; set; }

    public required string CategoryId { get; set; }
    public required string AgencyId { get; set; }

    [JsonIgnore]
    public string? CreatedById { get; set; }
}
