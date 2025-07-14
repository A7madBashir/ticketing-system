using TicketingSystem.Data;
using TicketingSystem.Models.FAQs;
using TicketingSystem.Services.Repositories;

namespace TicketingSystem.Services.Repositories;

public interface IFAQRepository : IUlidRepository<FAQ> { }

public class FAQRepository(ApplicationDbContext db)
    : UlidRepository<FAQ>(db),
        IFAQRepository { }
