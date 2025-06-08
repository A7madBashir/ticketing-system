using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using TicketingSystem.Data;
using TicketingSystem.Models.DTO.Responses.JWT;
using TicketingSystem.Models.DTO.Responses.User;
using TicketingSystem.Models.Identity;
using TicketingSystem.Services.Repositories;
using TicketingSystem.Settings;

namespace TicketingSystem.Services;

public interface IRefreshTokenService
{
    Task<RefreshTokenResponse> GenerateRefreshToken(string username);
    Task<bool> ValidateRefreshToken(string username, string token);
    Task<bool> RevokeRefreshToken(string username, string refreshTok);
    Task<LoginResponse> RefreshUserRefreshToken(string refreshToken);
}

public class RefreshTokenService : IRefreshTokenService
{
    private readonly TokenSettings _tokenSettings;
    private readonly IRefreshTokenRepository _repository;
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;

    public RefreshTokenService(
        IOptions<TokenSettings> options,
        IRefreshTokenRepository repository,
        UserManager<User> userManager,
        ITokenService tokenService
    )
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _repository = repository;
        _tokenSettings = options.Value;
    }

    public async Task<RefreshTokenResponse> GenerateRefreshToken(string username)
    {
        var user =
            await _userManager.FindByNameAsync(username) ?? throw new Exception("User not exist");

        using var randomNumberGenerator = RandomNumberGenerator.Create();
        double expireIn = _tokenSettings.RefreshTokeExpireInMinutes;
        var randomBytes = new byte[256];
        RandomNumberGenerator.Fill(randomBytes);
        var refreshTokenValidTo = DateTime.UtcNow.AddDays(expireIn);
        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(randomBytes),
            Expires = refreshTokenValidTo,
            CreateTime = DateTime.UtcNow,
            CreatedByIp = "",
            UserId = user.Id,
        };

        await _repository.AddAsync(refreshToken);
        return new RefreshTokenResponse
        {
            Succeeded = true,
            RefreshToken = refreshToken.Token,
            RefreshTokenValidTo = refreshTokenValidTo,
        };
    }

    public async Task<bool> RevokeRefreshToken(string username, string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException(
                $"'{nameof(username)}' cannot be null or whitespace.",
                nameof(username)
            );
        }

        if (refreshToken is null)
        {
            throw new ArgumentNullException(nameof(refreshToken));
        }

        var user =
            await _userManager.FindByNameAsync(username) ?? throw new Exception("User not exist");

        var token =
            await _repository.FirstOrDefaultAsync(Token =>
                Token.UserId == user.Id && Token.Token == refreshToken
            ) ?? throw new Exception("Refresh token not exist");

        token.Revoked = DateTime.UtcNow;
        token.RevokedByIp = "";
        await _repository.UpdateAsync(token);
        // Revoked successfully
        return true;
    }

    public async Task<LoginResponse> RefreshUserRefreshToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException(
                $"'{nameof(token)}' cannot be null or whitespace.",
                nameof(token)
            );
        }

        var refreshToken =
            await _repository.FirstOrDefaultAsync(t => t.Token == token)
            ?? throw new Exception("Refresh token not exist");

        if (!refreshToken.IsActive)
        {
            throw new Exception("Refresh token already revoked");
        }

        var user = await _userManager.FindByIdAsync(refreshToken.UserId.ToString());
        var newRefreshToken = await GenerateRefreshToken(user!.UserName!);
        refreshToken.ReplacedByToken = newRefreshToken.RefreshToken;
        refreshToken.Revoked = DateTime.UtcNow;
        refreshToken.RevokedByIp = "";
        await _repository.UpdateAsync(refreshToken);
        var tokenResult = await _tokenService.GenerateTokenAsync(user!.UserName!);
        // Refreshed successfully
        return new LoginResponse
        {
            Succeeded = true,
            Token = tokenResult.Token,
            TokenValidTo = tokenResult.TokenValidTo,
            RefreshToken = newRefreshToken.RefreshToken,
            RefreshTokenValidTo = newRefreshToken.RefreshTokenValidTo,
        };
    }

    public async Task<bool> ValidateRefreshToken(string username, string token)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException(
                $"'{nameof(username)}' cannot be null or whitespace.",
                nameof(username)
            );
        }

        var user =
            await _userManager.FindByNameAsync(username) ?? throw new Exception("User not exist");

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException(
                $"'{nameof(token)}' cannot be null or whitespace.",
                nameof(token)
            );
        }

        return await _repository.ExistAsync(refreshToken =>
            refreshToken.UserId == user.Id && refreshToken.Token == token
        );
    }
}
