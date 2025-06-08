using TicketingSystem.Data;
using TicketingSystem.Models.Identity;

namespace TicketingSystem.Services.Repositories;

public interface IRefreshTokenRepository : IUlidRepository<RefreshToken> { }

public class RefreshTokenRepository(ApplicationDbContext db)
    : UlidRepository<RefreshToken>(db),
        IRefreshTokenRepository { }
