using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TicketingSystem.Models.DTO.Requests.Subscriptions;
using TicketingSystem.Models.DTO.Responses.Subscriptions;
using TicketingSystem.Models.Entities.Agency;
using TicketingSystem.Services.Repositories;

namespace TicketingSystem.Controllers.Api;

public class SubscriptionController
    : CrudController<
        Subscription,
        Ulid,
        SubscriptionResponse,
        CreateSubscriptionRequest,
        UpdateSubscriptionRequest
    >
{
    public SubscriptionController(ISubscriptionRepository repository, Mapper mapper)
        : base(repository, mapper) { }
}
