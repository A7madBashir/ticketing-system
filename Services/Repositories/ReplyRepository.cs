using TicketingSystem.Data;
using TicketingSystem.Models.Entities.Agency;
using TicketingSystem.Services.Repositories;

namespace TicketingSystem.Services.Repositories;

public interface IReplyRepository : IUlidRepository<Reply> { }

public class ReplyRepository(ApplicationDbContext db)
    : UlidRepository<Reply>(db),
        IReplyRepository { }
