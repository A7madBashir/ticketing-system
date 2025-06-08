using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity; // For IdentityUser
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TicketingSystem.Models.DTO.Responses.JWT;
using TicketingSystem.Models.Identity;
using TicketingSystem.Settings;

namespace TicketingSystem.Services;

public interface ITokenService
{
    Task<TokenResponse> GenerateTokenAsync(string username);
    Task<bool> ValidateTokenAsync(string token);
    Task<User?> GetUser(ClaimsPrincipal claims);
}

public class TokenService(IOptions<TokenSettings> tokenSettings, UserManager<User> userManager)
    : ITokenService
{
    private readonly TokenSettings _tokenSettings = tokenSettings.Value;
    private readonly UserManager<User> _userManager = userManager;

    public async Task<TokenResponse> GenerateTokenAsync(string username)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_tokenSettings.Secret!);

        var user = await _userManager.FindByNameAsync(username);
        var roles = await _userManager.GetRolesAsync(user!);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, user!.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, username),
        };

        foreach (var r in roles)
        {
            claims.Add(new Claim("roles", r));
        }

        var tokenValidTo = DateTime.UtcNow.AddMinutes(_tokenSettings.ExpiryInMinutes);
        var token = new JwtSecurityToken(
            _tokenSettings.Issuer,
            _tokenSettings.Audience,
            claims,
            expires: tokenValidTo,
            
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        );

        return new TokenResponse
        {
            Succeeded = true,
            Token = tokenHandler.WriteToken(token),
            TokenValidTo = tokenValidTo,
        };
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_tokenSettings.Secret!);

        var result = await tokenHandler.ValidateTokenAsync(
            token,
            new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero, // Disable token expiration tolerance (optional)
            }
        );

        return result.Exception is null;
    }

    public async Task<User?> GetUser(ClaimsPrincipal claims)
    {
        var userId = claims.FindFirst(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

        if (userId is null)
        {
            return null;
        }

        return await _userManager.Users.FirstOrDefaultAsync(u => u.Id == Ulid.Parse(userId));
    }
}
