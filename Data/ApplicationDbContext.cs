using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Data.Converter;
using TicketingSystem.Models.Entities.Agency;
using TicketingSystem.Models.Identity;
using TicketingSystem.Models.Categorys;
using TicketingSystem.Models.Tickets;
using TicketingSystem.Models.Replays;
using TicketingSystem.Models.FAQs;
using TicketingSystem.Models.Subscription;
using TicketingSystem.Models.Integrations;

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
        modelBuilder.Entity<Agency>()
         .HasMany(a => a.Users)
         .WithOne(u => u.Agency)
         .HasForeignKey(u => u.AgencyId);

        modelBuilder.Entity<Agency>()
        .HasMany(a => a.Tickets)
        .WithOne(t => t.Agency)
        .HasForeignKey(t => t.AgencyId);

        modelBuilder.Entity<Agency>()
        .HasMany(a => a.Categories)
        .WithOne(c => c.Agency)
        .HasForeignKey(c => c.AgencyId);

        modelBuilder.Entity<Agency>()
        .HasMany(a => a.FAQs)
        .WithOne(f => f.Agency)
        .HasForeignKey(f => f.AgencyId);

        modelBuilder.Entity<Agency>()
        .HasMany(a => a.Integrations)
        .WithOne(i => i.Agency)
        .HasForeignKey(i => i.AgencyId);

        modelBuilder.Entity<Agency>()
        .HasOne(a => a.Subscription)
        .WithMany() // assuming one-to-many, if one-to-one: use .WithOne()
        .HasForeignKey(a => a.SubscriptionId);

        modelBuilder.Entity<User>()
        .HasMany(u => u.TicketsCreated)
        .WithOne(t => t.CreatedByUser)
        .HasForeignKey(t => t.CreatedBy);

        modelBuilder.Entity<User>()
        .HasMany(u => u.TicketsAssigned)
        .WithOne(t => t.AssignedToUser)
        .HasForeignKey(t => t.AssignedTo);

        modelBuilder.Entity<User>()
        .HasMany(u => u.Replies)
        .WithOne(r => r.User)
        .HasForeignKey(r => r.UserId);

        modelBuilder.Entity<Ticket>()
        .HasMany(t => t.Replies)
        .WithOne(r => r.Ticket)
        .HasForeignKey(r => r.TicketId);

        modelBuilder.Entity<Ticket>()
        .HasOne(t => t.Category)
        .WithMany(c => c.Tickets)
        .HasForeignKey(t => t.CategoryId);


        modelBuilder.Entity<Category>()
        .HasMany(c => c.Tickets)
        .WithOne(t => t.Category)
        .HasForeignKey(t => t.CategoryId);

        modelBuilder.Entity<Replay>()
        .HasOne(r => r.Ticket)
        .WithMany(t => t.Replies)
        .HasForeignKey(r => r.TicketId);

        modelBuilder.Entity<Replay>()
        .HasOne(r => r.User)
        .WithMany(u => u.Replies)
        .HasForeignKey(r => r.UserId);

        modelBuilder.Entity<FAQ>()
        .HasOne(f => f.Agency)
        .WithMany(a => a.FAQs)
        .HasForeignKey(f => f.AgencyId);

        modelBuilder.Entity<Integration>()
        .HasOne(i => i.Agency)
        .WithMany(a => a.Integrations)
        .HasForeignKey(i => i.AgencyId);

        modelBuilder.Entity<Agency>()
        .HasOne(a => a.Subscription)
        .WithMany() // or .WithOne(s => s.Agency) if it's one-to-one
        .HasForeignKey(a => a.SubscriptionId);


    }
        public DbSet<Agency> Agencies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Replay> Replies { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Integration> Integrations { get; set; }

}
