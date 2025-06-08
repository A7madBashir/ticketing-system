using TicketingSystem.Data.Seeder;

namespace TicketingSystem.Extensions;

public static class SeedDataExtension
{
    public static async Task SeedData(this IServiceCollection services)
    {
        var seeder = services
            .BuildServiceProvider()
            .CreateScope()
            .ServiceProvider.GetService<DataSeeder>();

        if (seeder is null)
        {
            return;
        }

        // Call seed methods
        await seeder.SeedRoles();
        await seeder.SeedUsers();
    }
}
