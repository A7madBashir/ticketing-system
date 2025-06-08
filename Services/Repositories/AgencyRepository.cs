using TicketingSystem.Data;
using TicketingSystem.Models.Entities.Agency;

namespace TicketingSystem.Services.Repositories;

public interface IAgencyRepository : IUlidRepository<Agency> { }

public class AgencyRepository(ApplicationDbContext db)
    : UlidRepository<Agency>(db),
        IAgencyRepository { }
