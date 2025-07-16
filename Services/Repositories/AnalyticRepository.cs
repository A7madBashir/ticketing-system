using TicketingSystem.Data;
using TicketingSystem.Models.Entities.Agency;
using TicketingSystem.Services.Repositories;

namespace TicketingSystem.Services.Repositories;

public interface IAnalyticRepository : IUlidRepository<Analytic> { }

public class AnalyticRepository(ApplicationDbContext db)
    : UlidRepository<Analytic>(db),
        IAnalyticRepository { }
