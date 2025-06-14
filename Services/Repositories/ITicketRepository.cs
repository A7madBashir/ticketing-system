using TicketingSystem.Data;
using TicketingSystem.Models.Entities.Tickets;

namespace TicketingSystem.Services.Repositories;

public interface ITicketRepository : IUlidRepository<Ticket> { }

public class TicketRepository(ApplicationDbContext db)
    : UlidRepository<Ticket>(db),
        ITicketRepository { }
