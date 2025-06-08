using System.Text.Json.Serialization;

namespace TicketingSystem.Models.DTO.Requests.Category;

public class CreateCategoryRequest : ICreateRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string AgencyId { get; set; }
}

public class EditCategoryRequest : CreateCategoryRequest, IEditRequest<Ulid>
{
    [JsonIgnore]
    public Ulid Id { get; set; }
}
