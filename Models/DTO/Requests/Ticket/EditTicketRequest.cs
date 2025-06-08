using System.Text.Json.Serialization;

namespace TicketingSystem.Models.DTO.Requests.Ticket;

public class EditTicketRequest : CreateTicketRequest, IEditRequest<Ulid>
{
    [JsonIgnore]
    public Ulid Id { get; set; }
}
