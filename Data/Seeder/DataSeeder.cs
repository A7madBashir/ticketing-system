using System.Reflection;
using Microsoft.AspNetCore.Identity;
using TicketingSystem.Models.Identity;

namespace TicketingSystem.Data.Seeder;

public class DataSeeder(
    ApplicationDbContext context,
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    IConfiguration config
)
{
    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<User> _userManager = userManager;
    private readonly RoleManager<Role> roleManager = roleManager;
    private readonly IConfiguration config = config;

    // Seed roles and users
    public async Task SeedUsers()
    {
        if (!_userManager.Users.Any())
        {
            // admin user
            var email = config["DefaultUsers:Admin:Email"];
            var password = config["DefaultUsers:Admin:Password"];
            User adminUser = new User
            {
                Email = email,
                EmailConfirmed = true,
                UserName = email,
            };

            await _userManager.CreateAsync(adminUser, password);

            await _userManager.AddToRoleAsync(adminUser, TicketingSystem.Models.Common.Roles.Admin);
        }
    }

    public async Task SeedRoles()
    {
        if (!_context.Roles.Any())
        {
            var definedRoles = typeof(Models.Common.Roles)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(fi => fi.FieldType == typeof(string))
                .Select(fi => fi.GetValue(null).ToString())
                .ToList();

            foreach (var roleName in definedRoles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new Role { Name = roleName });
                }
            }
        }
    }
}
