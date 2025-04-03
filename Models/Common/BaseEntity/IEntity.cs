namespace TicketingSystem.Models.Common.BaseEntity;

public interface IEntity<T>
    where T : IEquatable<T>
{
    T Id { get; set; }
}
