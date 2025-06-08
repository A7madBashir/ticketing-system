using TicketingSystem.Data;
using TicketingSystem.Models.Entities.Agency;

namespace TicketingSystem.Services.Repositories;

public interface ICategoryRepository : IUlidRepository<Category> { }

public class CategoryRepository(ApplicationDbContext db)
    : UlidRepository<Category>(db),
        ICategoryRepository { }
