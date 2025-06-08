using System.Text.Json.Serialization;

namespace TicketingSystem.Models.DTO.Requests;

public interface IEditRequest<T>
    where T : IEquatable<T>
{
    [JsonIgnore]
    T Id { get; set; }
}
