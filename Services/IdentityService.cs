using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using TicketingSystem.Models.Common;
using TicketingSystem.Models.DTO.Responses.User;
using TicketingSystem.Models.Identity;
using TicketingSystem.Services.Repositories;

namespace TicketingSystem.Services;

public interface IIdentityService
{
    Task<User?> GetUserByPhoneNumberAsync(string phoneNumber);

    Task<User> RegisterUserAsync(string phoneNumber, string role);

    Task<bool> RegisterUserAsync(string username, string password, string role);

    Task<LoginResponse> LoginUserAsync(string username, string password, string ipAddress = "");

    Task<LoginResponse> LoginUserByPhoneAsync(string phone, string code, string ipAddress = "");

    Task<LoginResponse> LoginUserByEmailAsync(string email, string password, string ipAddress = "");

    Task<LoginResponse> LoginUserByFirebaseAsync(
        string idToken,
        string role,
        string ipAddress = ""
    );

    Task<bool> ResetPasswordAsync(string username, string newPassword);

    Task<bool> DeleteAccountAsync(string username);

    Task<bool> RevokeRefreshToken(string username, string token, string ipAddress = "");

    Task<LoginResponse> RefreshUserRefreshToken(string refreshToken, string ipAddress = "");

    Task<User?> GetUser(ClaimsPrincipal claims);
    Task<bool> IsAgent(ClaimsPrincipal claims);
    Task<bool> IsAdmin(ClaimsPrincipal claims);
    Task<bool> IsAgent(User user);
    Task<bool> IsAdmin(User user);
    Task<User> GetOrAddUser(string email, string? phone, string name);
}

public class IdentityService(UserManager<User> userManager, IUserRepository repository)
    : IIdentityService
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly IUserRepository _repository = repository;

    public Task<bool> DeleteAccountAsync(string username)
    {
        throw new NotImplementedException();
    }

    public async Task<User?> GetUser(ClaimsPrincipal claims)
    {
        if (claims is null)
            throw new Exception("Claims are null");

        var username = claims?.Identity?.Name ?? "";
        var userId = claims?.FindFirst(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

        bool validUlid = Ulid.TryParse(userId, out Ulid userUlid);
        if (!string.IsNullOrEmpty(userId) && !validUlid)
        {
            throw new Exception(ErrorCodes.InvalidUlid);
        }

        User user;
        if (!string.IsNullOrEmpty(userId))
        {
            user = await _repository.GetByIdAsync(userUlid);
        }
        else
        {
            user = await _repository.FirstOrDefaultAsync(u => u.UserName == username);
        }

        if (user == null)
        {
            return null;
        }

        return user;
    }

    public Task<User?> GetUserByPhoneNumberAsync(string phoneNumber)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> IsAdmin(ClaimsPrincipal claims)
    {
        var user = await GetUser(claims);
        if (user is null)
            return false;

        return await _userManager.IsInRoleAsync(user, Roles.Admin);
    }

    public async Task<bool> IsAgent(ClaimsPrincipal claims)
    {
        var user = await GetUser(claims);
        if (user is null)
            return false;

        return await _userManager.IsInRoleAsync(user, Roles.Agent);
    }

    public async Task<bool> IsAdmin(User user)
    {
        if (user is null)
            return false;

        return await _userManager.IsInRoleAsync(user, Roles.Admin);
    }

    public async Task<bool> IsAgent(User user)
    {
        if (user is null)
            return false;

        return await _userManager.IsInRoleAsync(user, Roles.Agent);
    }

    public Task<LoginResponse> LoginUserAsync(
        string username,
        string password,
        string ipAddress = ""
    )
    {
        throw new NotImplementedException();
    }

    public Task<LoginResponse> LoginUserByEmailAsync(
        string email,
        string password,
        string ipAddress = ""
    )
    {
        throw new NotImplementedException();
    }

    public Task<LoginResponse> LoginUserByFirebaseAsync(
        string idToken,
        string role,
        string ipAddress = ""
    )
    {
        throw new NotImplementedException();
    }

    public Task<LoginResponse> LoginUserByPhoneAsync(
        string phone,
        string code,
        string ipAddress = ""
    )
    {
        throw new NotImplementedException();
    }

    public Task<LoginResponse> RefreshUserRefreshToken(string refreshToken, string ipAddress = "")
    {
        throw new NotImplementedException();
    }

    public Task<User> RegisterUserAsync(string phoneNumber, string role)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RegisterUserAsync(string username, string password, string role)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ResetPasswordAsync(string username, string newPassword)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RevokeRefreshToken(string username, string token, string ipAddress = "")
    {
        throw new NotImplementedException();
    }

    public async Task<User> GetOrAddUser(string email, string? phone, string name)
    {
        var ut = await _repository.FirstOrDefaultAsync(u =>
            u.UserName == email
        );
        if (ut is not null)
        {
            return ut;
        }

        var newUser = new User
        {
            Email = email,
            PhoneNumber = phone,
            FirstName = name,
            UserName = email,
            CreatedAt = DateTime.Now,
        };

        var re = await _userManager.CreateAsync(newUser);
        if (re.Succeeded)
        {
            await _userManager.AddToRoleAsync(newUser, Roles.User);
            return newUser;
        }

        throw new Exception("Failed to add or get user");
    }
}
