using TicketingSystem.Data;
using TicketingSystem.Models.Common.BaseEntity;

namespace TicketingSystem.Services.Repositories;

public interface IUlidRepository<TEntity> : IRepository<TEntity, Ulid>
    where TEntity : class, IEntity<Ulid> { }

public class UlidRepository<TEntity>(ApplicationDbContext context)
    : Repository<TEntity, Ulid>(context),
        IUlidRepository<TEntity>
    where TEntity : BaseEntity { }
