using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Data.Converter;
using TicketingSystem.Models.Entities.Agency;
using TicketingSystem.Models.Identity;

namespace TicketingSystem.Data;

public class ApplicationDbContext : IdentityDbContext<User, Role, Ulid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<Ulid>().HaveConversion<UlidToStringConverter>();
        configurationBuilder.Properties<DateTime>().HaveConversion<DateTimeConverter>();
    }

    public DbSet<Agency> Agencies { get; set; }
}
