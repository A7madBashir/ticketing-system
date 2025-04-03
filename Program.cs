using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TicketingSystem.Data;
using TicketingSystem.Extensions;
using TicketingSystem.Models.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(
    (context, configuration) => configuration.ReadFrom.Configuration(context.Configuration)
);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder
    .Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation().AddControllersAsServices();
builder.Services.AddAuthentication();
builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorPages();

if (args.Length != 0 && args[0] == "seed")
{
    await builder.Services.SeedData();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
