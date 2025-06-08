using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using TicketingSystem.Models.DTO.Requests.User;
using TicketingSystem.Models.DTO.Responses.User;
using TicketingSystem.Models.Identity;
using TicketingSystem.Services;
using TicketingSystem.Services.Repositories;

namespace TicketingSystem.Endpoints.Account;

/// <summary>
/// User account endpoint
/// </summary>
public static class AccountEndpoints
{
    private static readonly EmailAddressAttribute _emailAddressAttribute = new();

    /// <summary>
    /// Mapping custom user account endpoint provided by base identity endpoints
    /// </summary>
    /// <param name="endpoints"></param>
    /// <returns></returns>
    public static IEndpointConventionBuilder MapUserAccountEndpoints(
        this IEndpointRouteBuilder endpoints
    )
    {
        var emailSender = endpoints.ServiceProvider.GetRequiredService<IEmailSender<User>>();
        var linkGenerator = endpoints.ServiceProvider.GetRequiredService<LinkGenerator>();

        var routeGroup = endpoints.MapGroup("/api/account").WithTags("Account");
        routeGroup.MapIdentityApi<User>();

        endpoints
            .MapPost(
                "/api/account/v2/register",
                async Task<
                    Results<
                        Ok<LoginResponse>,
                        EmptyHttpResult,
                        ProblemHttpResult,
                        ValidationProblem
                    >
                > (
                    [FromBody] RegisterUser registration,
                    [FromQuery] bool? useCookies,
                    [FromQuery] bool? useSessionCookies,
                    HttpContext context,
                    [FromServices] IServiceProvider sp
                ) =>
                {
                    var userManager = sp.GetRequiredService<UserManager<User>>();
                    var tokenService = sp.GetRequiredService<ITokenService>();
                    var refreshTokenService = sp.GetRequiredService<IRefreshTokenService>();

                    if (!userManager.SupportsUserEmail)
                    {
                        throw new NotSupportedException(
                            $"{nameof(MapUserAccountEndpoints)} requires a user store with email support."
                        );
                    }

                    var userStore = sp.GetRequiredService<IUserStore<User>>();
                    var emailStore = (IUserEmailStore<User>)userStore;
                    var email = registration.Email;

                    if (string.IsNullOrEmpty(email) || !_emailAddressAttribute.IsValid(email))
                    {
                        return CreateValidationProblem(
                            IdentityResult.Failed(userManager.ErrorDescriber.InvalidEmail(email))
                        );
                    }

                    var user = new User()
                    {
                        FirstName = registration.FirstName,
                        LastName = registration.LastName,
                        Nationality = registration.Nationality,
                        PhoneNumber = registration.PhoneNumber,
                        Gender = registration.Gender,
                        PassportNumber = registration.PassportNumber,
                        Job = registration.Job,
                        DateOfBirth = registration.DateOfBirth,
                    };
                    await userStore.SetUserNameAsync(user, email, CancellationToken.None);
                    await emailStore.SetEmailAsync(user, email, CancellationToken.None);
                    var newUser = await userManager.CreateAsync(user, registration.Password);

                    if (!newUser.Succeeded)
                    {
                        return CreateValidationProblem(newUser);
                    }

                    var signInManager = sp.GetRequiredService<SignInManager<User>>();

                    var useCookieScheme = (useCookies == true) || (useSessionCookies == true);
                    var isPersistent = (useCookies == true) && (useSessionCookies != true);
                    signInManager.AuthenticationScheme = useCookieScheme
                        ? IdentityConstants.ApplicationScheme
                        : IdentityConstants.BearerScheme;

                    var result = await signInManager.PasswordSignInAsync(
                        email,
                        registration.Password,
                        isPersistent,
                        lockoutOnFailure: true
                    );
                    if (!result.Succeeded)
                    {
                        return TypedResults.Problem(
                            result.ToString(),
                            statusCode: StatusCodes.Status401Unauthorized
                        );
                    }

                    //  await SendConfirmationEmailAsync(user, userManager, context, email);
                    var tokenResponse = await tokenService.GenerateTokenAsync(user.UserName);
                    var refreshTokenResponse = await refreshTokenService.GenerateRefreshToken(
                        user.UserName
                    );

                    var res = new LoginResponse
                    {
                        Succeeded = true,
                        Token = tokenResponse.Token,
                        TokenValidTo = tokenResponse.TokenValidTo,
                        RefreshToken = refreshTokenResponse.RefreshToken,
                        RefreshTokenValidTo = refreshTokenResponse.RefreshTokenValidTo,
                    };

                    return TypedResults.Ok(res);
                }
            )
            .WithDescription(
                "Modified version identity endpoints which includes more attributes for the user"
            )
            .WithTags("Account");

        // --- Custom Login Endpoint (JWT Issuance) ---
        endpoints
            .MapPost(
                "/api/account/v2/login",
                async Task<
                    Results<
                        Ok<LoginResponse>,
                        EmptyHttpResult,
                        ProblemHttpResult,
                        UnauthorizedHttpResult,
                        BadRequest<string>
                    >
                > (
                    [FromBody] LoginRequest request,
                    [FromQuery] bool? useCookies,
                    [FromQuery] bool? useSessionCookies,
                    [FromServices] SignInManager<User> signInManager,
                    [FromServices] UserManager<User> userManager,
                    [FromServices] ITokenService tokenService,
                    [FromServices] IRefreshTokenService refreshService,
                    HttpContext httpContext
                ) =>
                {
                    if (
                        string.IsNullOrEmpty(request.Email)
                        || string.IsNullOrEmpty(request.Password)
                    )
                    {
                        return TypedResults.BadRequest("Email and password are required.");
                    }

                    var user = await userManager.FindByEmailAsync(request.Email);
                    if (user == null)
                    {
                        return TypedResults.Unauthorized();
                    }

                    var useCookieScheme = (useCookies == true) || (useSessionCookies == true);
                    var isPersistent = (useCookies == true) && (useSessionCookies != true);
                    signInManager.AuthenticationScheme = useCookieScheme
                        ? IdentityConstants.ApplicationScheme
                        : JwtBearerDefaults.AuthenticationScheme;

                    Microsoft.AspNetCore.Identity.SignInResult userSignIn;

                    // Handling user login by different auth schema
                    if (useCookieScheme)
                    {
                        userSignIn = await signInManager.PasswordSignInAsync(
                            request.Email,
                            request.Password,
                            isPersistent,
                            lockoutOnFailure: true
                        );
                    }
                    else
                    {
                        userSignIn = await signInManager.CheckPasswordSignInAsync(
                            user,
                            request.Password,
                            false
                        );
                    }

                    if (userSignIn.Succeeded)
                    {
                        var accessToken = await tokenService.GenerateTokenAsync(user.UserName);
                        var refreshToken = await refreshService.GenerateRefreshToken(user.UserName);

                        if (useCookieScheme)
                            return TypedResults.Empty;
                        else
                            return TypedResults.Ok(
                                new LoginResponse
                                {
                                    Succeeded = true,
                                    Token = accessToken.Token,
                                    TokenValidTo = accessToken.TokenValidTo,
                                    RefreshToken = refreshToken.RefreshToken,
                                    RefreshTokenValidTo = refreshToken.RefreshTokenValidTo,
                                }
                            );
                    }
                    else if (userSignIn.IsLockedOut)
                    {
                        // Account locked out.
                        return TypedResults.Unauthorized();
                    }
                    else if (userSignIn.IsNotAllowed)
                    {
                        // Login not allowed (e.g., email not confirmed).
                        return TypedResults.Unauthorized();
                    }
                    else
                    {
                        // Invalid credentials.
                        return TypedResults.Unauthorized();
                    }
                }
            )
            .WithTags("Account");

        routeGroup
            .MapPost(
                "/api/account/v2/refresh",
                async Task<
                    Results<
                        Ok<LoginResponse>,
                        EmptyHttpResult,
                        ProblemHttpResult,
                        UnauthorizedHttpResult,
                        BadRequest<string>
                    >
                > (
                    [FromBody] RefreshRequest refreshRequest,
                    [FromServices] IRefreshTokenRepository repository,
                    [FromServices] UserManager<User> userManager,
                    [FromServices] IRefreshTokenService refreshTokenService,
                    [FromServices] ITokenService tokenService
                ) =>
                {
                    // validate user then revoke exist token and regenerate new one

                    var refreshToken = await repository.FirstOrDefaultAsync(t =>
                        t.Token == refreshRequest.RefreshToken
                    );

                    if (refreshToken is null)
                        return TypedResults.BadRequest("Refresh token not found exception");

                    if (!refreshToken.IsActive)
                    {
                        return TypedResults.BadRequest("Refresh token already revoked exception");
                    }

                    var user = await userManager.FindByIdAsync(refreshToken.UserId.ToString());
                    var newRefreshToken = await refreshTokenService.GenerateRefreshToken(
                        user!.UserName!
                    );
                    refreshToken.ReplacedByToken = newRefreshToken.RefreshToken;
                    refreshToken.Revoked = DateTime.UtcNow;
                    await repository.UpdateAsync(refreshToken);
                    var tokenResult = await tokenService.GenerateTokenAsync(user!.UserName!);

                    var res = new LoginResponse
                    {
                        Succeeded = true,
                        Token = tokenResult.Token,
                        TokenValidTo = tokenResult.TokenValidTo,
                        RefreshToken = newRefreshToken.RefreshToken,
                        RefreshTokenValidTo = newRefreshToken.RefreshTokenValidTo,
                    };

                    return TypedResults.Ok(res);
                }
            )
            .WithTags("Account");

        return routeGroup;
    }

    private static ValidationProblem CreateValidationProblem(
        string errorCode,
        string errorDescription
    ) =>
        TypedResults.ValidationProblem(
            new Dictionary<string, string[]> { { errorCode, [errorDescription] } }
        );

    private static ValidationProblem CreateValidationProblem(IdentityResult result)
    {
        // We expect a single error code and description in the normal case.
        // This could be golfed with GroupBy and ToDictionary, but perf! :P
        Debug.Assert(!result.Succeeded);
        var errorDictionary = new Dictionary<string, string[]>(1);

        foreach (var error in result.Errors)
        {
            string[] newDescriptions;

            if (errorDictionary.TryGetValue(error.Code, out var descriptions))
            {
                newDescriptions = new string[descriptions.Length + 1];
                Array.Copy(descriptions, newDescriptions, descriptions.Length);
                newDescriptions[descriptions.Length] = error.Description;
            }
            else
            {
                newDescriptions = [error.Description];
            }

            errorDictionary[error.Code] = newDescriptions;
        }

        return TypedResults.ValidationProblem(errorDictionary);
    }
}
