using System.Text.Json.Serialization;

namespace TicketingSystem.Models.DTO.Requests.Agency;

public class CreateAgency : ICreateRequest
{
    public required string Name { get; set; }
    public required string Domain { get; set; }
    public required string SubscriptionId { get; set; }
}

public class EditAgency : CreateAgency, IEditRequest<Ulid>
{
    [JsonIgnore]
    public Ulid Id { get; set; }
}
