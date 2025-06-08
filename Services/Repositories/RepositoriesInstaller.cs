namespace TicketingSystem.Services.Repositories;

public static class RepositoriesInstaller
{
    public static void RepositoriesInstall(this IServiceCollection services)
    {
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddTransient<ISubscriptionRepository, SubscriptionRepository>();
        services.AddTransient<IAgencyRepository, AgencyRepository>();
    }
}
