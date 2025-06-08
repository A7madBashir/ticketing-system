using TicketingSystem.Data;
using TicketingSystem.Models.Entities.Agency;

namespace TicketingSystem.Services.Repositories;

public interface ISubscriptionRepository : IUlidRepository<Subscription> { }

public class SubscriptionRepository(ApplicationDbContext db)
    : UlidRepository<Subscription>(db),
        ISubscriptionRepository { }
