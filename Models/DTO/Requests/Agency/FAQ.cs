using System.Text.Json.Serialization;

namespace TicketingSystem.Models.DTO.Requests.FAQ
{
    public class CreateFAQRequest : ICreateRequest
    {
        public required string AgencyId { get; set; }
        public string? Question { get; set; }
        public string? Answer { get; set; }
    }

    public class EditFAQRequest : CreateFAQRequest, IEditRequest<Ulid>
    {
       [JsonIgnore]
       public Ulid Id { get; set; }
    }
}
