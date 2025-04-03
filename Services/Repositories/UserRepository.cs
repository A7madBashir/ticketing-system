using TicketingSystem.Data;
using TicketingSystem.Models.Identity;

namespace TicketingSystem.Services.Repositories;

public interface IUserRepository : IRepository<User, Ulid> { }

public class UserRepository(ApplicationDbContext db)
    : Repository<User, Ulid>(db),
        IUserRepository { }
