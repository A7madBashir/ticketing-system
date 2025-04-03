using TicketingSystem.Mapper;

namespace TicketingSystem.Services;

public static class ServicesInstaller
{
    /// <summary>
    /// Install services from external extension method
    /// </summary>
    /// <param name="services"></param>
    public static void CustomServicesInstaller(this IServiceCollection services)
    {
        services.AddSingleton<AppMapper>();
    }
}
