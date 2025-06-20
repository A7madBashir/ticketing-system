using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TicketingSystem.Models.Common;
using TicketingSystem.Settings;

namespace TicketingSystem.Extensions;

public static class AuthorizationsExtension
{
    public static IServiceCollection AddAuthorizations(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services
            .AddAuthentication(AuthenticationSchema.Identity)
            .AddJwtBearer(
                JwtBearerDefaults.AuthenticationScheme,
                opt =>
                {
                    string secretKey = configuration.GetSection(nameof(TokenSettings))[
                        nameof(TokenSettings.Secret)
                    ]!;
                    string audience = configuration.GetSection(nameof(TokenSettings))[
                        nameof(TokenSettings.Audience)
                    ]!;
                    string issuer = configuration.GetSection(nameof(TokenSettings))[
                        nameof(TokenSettings.Issuer)
                    ]!;
                    opt.RequireHttpsMetadata = false;
                    opt.SaveToken = true;
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateLifetime = true,
                        LifetimeValidator = (notBefore, expires, token, parameters) =>
                        {
                            var currentUtc = DateTime.UtcNow;
                            if (notBefore != null && currentUtc < notBefore)
                            {
                                return false;
                            }

                            if (expires != null && expires <= currentUtc)
                            {
                                return false;
                            }

                            return true;
                        },
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(secretKey)
                        ),
                        ClockSkew = TimeSpan.FromSeconds(0),
                    };
                }
            )
            .AddCookie(
                "Cookies",
                options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.SlidingExpiration = true;
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                }
            );

        return services;
    }
}
