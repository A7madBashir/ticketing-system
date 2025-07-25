using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Models.Integrations;
using TicketingSystem.Services.Repositories;

namespace TicketingSystem.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)] // Allow on controllers or specific methods
public class ApiKeyAuthorizeAttribute : Attribute, IAsyncActionFilter
{
    private const string API_KEY_HEADER = "X-Api-Key";

    // TODO: handle validate domain with request refere header
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    )
    {
        if (
            context.HttpContext.User.Identity is not null
            && context.HttpContext.User.Identity.IsAuthenticated
        )
        {
            await next();
        }

        // Check for the API key in the request header
        if (
            !context.HttpContext.Request.Headers.TryGetValue(
                API_KEY_HEADER,
                out var extractedApiKey
            )
        )
        {
            context.Result = new UnauthorizedObjectResult("API Key was not provided.");
            return;
        }

        // Get the DbContext from the dependency injection container
        var integrationsRepository =
            context.HttpContext.RequestServices.GetRequiredService<IIntegrationsRepository>();

        // Validate the API key
        var agency = (
            await integrationsRepository
                .Query([nameof(Integration.Agency)])
                .FirstOrDefaultAsync(a => a.ApiKey == extractedApiKey.ToString())
        )?.Agency;
        if (agency == null)
        {
            context.Result = new UnauthorizedObjectResult("Invalid API Key.");
            return;
        }

        // ✨ Pass the Agency ID to the controller, just like we did with the middleware
        context.HttpContext.Items["AgencyId"] = agency.Id;

        // If the key is valid, proceed with executing the action
        await next();
    }
}
