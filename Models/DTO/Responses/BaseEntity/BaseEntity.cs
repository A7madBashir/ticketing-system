using TicketingSystem.Models.Common.BaseEntity;

namespace TicketingSystem.Models.DTO.Responses.BaseEntity;

public class BaseEntity<T> : IEntity<T>
    where T : IEquatable<T>
{
    public T Id { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime? ModifiedTime { get; set; }
}
