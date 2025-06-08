using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Data.Converter;
using TicketingSystem.Models.Entities.Agency;
using TicketingSystem.Models.FAQs;
using TicketingSystem.Models.Identity;
using TicketingSystem.Models.Integrations;
using TicketingSystem.Models.Tickets;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ticket>().HasMany(t => t.AssignedTo).WithMany(c => c.AssignedTickets);
        modelBuilder
            .Entity<Ticket>()
            .HasOne(t => t.CreatedBy)
            .WithMany(c => c.CreatedTickets)
            .HasForeignKey(t => t.CreatedById);

        modelBuilder.Entity<Subscription>().Property(s => s.Features).HasColumnType("jsonb"); // For PostgreSQL, this will generate a jsonb column

        // Ensure base.OnModelCreating(modelBuilder) is called if you inherit from a base DbContext
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Agency> Agencies { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Reply> Replies { get; set; }
    public DbSet<FAQ> FAQs { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Integration> Integrations { get; set; }
    public DbSet<Analytic> Analytics { get; set; }
}
