namespace TicketingSystem.Models.Common.BaseEntity;

public class BaseEntity<T> : IEntity<T>
    where T : IEquatable<T>
{
    public T Id { get; set; } = default!;
}

public class BaseEntity : BaseEntity<Ulid>
{
    public DateTime CreateTime { get; set; }
    public DateTime? ModifiedTime { get; set; }
    public DateTime? DeleteTime { get; set; }

    public BaseEntity()
    {
        Id = Ulid.NewUlid();
    }
}
