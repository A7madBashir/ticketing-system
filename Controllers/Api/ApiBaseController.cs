using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketingSystem.Models.Common;

namespace TicketingSystem.Controllers.Api;

[ApiController] // Indicates that the class is an API controller and enables opinionated behaviors
[Route("api/[controller]")] // Defines the base route for all actions in this controller
[Authorize(
    AuthenticationSchemes = AuthenticationSchema.Identity + ", " + AuthenticationSchema.Bearer
)] // Requires authentication for all actions in this controller by default
public class ApiBaseController : ControllerBase { }
