using TicketingSystem.Data;
using TicketingSystem.Models.Integrations;
using TicketingSystem.Services.Repositories;

namespace TicketingSystem.Services.Repositories;

public interface IIntegrationsRepository : IUlidRepository<Integration> { }

public class IntegrationsRepository(ApplicationDbContext db)
    : UlidRepository<Integration>(db),
        IIntegrationsRepository { }
