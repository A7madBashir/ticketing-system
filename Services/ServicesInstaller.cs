using TicketingSystem.Data.Seeder;
using TicketingSystem.Settings;

namespace TicketingSystem.Services;

public static class ServicesInstaller
{
    /// <summary>
    /// Install services from external extension method
    /// </summary>
    /// <param name="services"></param>
    public static void CustomServicesInstaller(this IServiceCollection services)
    {
        services.AddSingleton<Mapper>();
        services.AddScoped<DataSeeder>();

        services
            .AddOptions<TokenSettings>()
            .BindConfiguration(nameof(TokenSettings))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();

        services.AddScoped<IIdentityService, IdentityService>();
    }
}
