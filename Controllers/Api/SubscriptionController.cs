using Microsoft.AspNetCore.Authorization;
using TicketingSystem.Models.Common;
using TicketingSystem.Models.DTO.Requests.Subscriptions;
using TicketingSystem.Models.DTO.Responses.Subscriptions;
using TicketingSystem.Models.Entities.Agency;
using TicketingSystem.Services.Repositories;

namespace TicketingSystem.Controllers.Api;

[Authorize(Policy = AuthenticationPolicy.AdminOnly)]
public class SubscriptionController(ISubscriptionRepository repository, Mapper mapper)
    : CrudController<
        Subscription,
        Ulid,
        SubscriptionResponse,
        CreateSubscriptionRequest,
        UpdateSubscriptionRequest
    >(repository, mapper)
{
    protected override string[]? GetSearchableProperties()
    {
        return new string[] { nameof(Subscription.PlanName), nameof(Subscription.Price) };
    }
}
