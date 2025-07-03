using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;
using Serilog;
using TicketingSystem.Data;
using TicketingSystem.Endpoints.Account;
using TicketingSystem.Extensions;
using TicketingSystem.Models.Identity;
using TicketingSystem.Services;
using TicketingSystem.Services.Repositories;
using TicketingSystem.Settings;
using TicketingSystem.Data.Seeder;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(
    (context, configuration) => configuration.ReadFrom.Configuration(context.Configuration)
);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder
    .Services.AddIdentityApiEndpoints<User>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = false;
    })
    .AddRoles<Role>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

builder.Services.AddOpenApiDocument(document =>
{
    document.AddSecurity(
        JwtBearerDefaults.AuthenticationScheme,
        new NSwag.OpenApiSecurityScheme
        {
            Type = OpenApiSecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            BearerFormat = "JWT",
            Description = "Type into the textbox: {your JWT token}.",
        }
    );

    document.OperationProcessors.Add(
        new AspNetCoreOperationSecurityScopeProcessor(JwtBearerDefaults.AuthenticationScheme)
    );
});
builder.Services.AddAuthorizations(builder.Configuration);
builder.Services.CustomServicesInstaller();
builder.Services.RepositoriesInstall();
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation().AddControllersAsServices();

builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorPages();
builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<DataSeeder>();


if (args.Length != 0 && args[0] == "seed")
{
    await builder.Services.SeedData();
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseOpenApi();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapDefaultControllerRoute();

app.MapUserAccountEndpoints();

app.Run();
