using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TicketingSystem.Controllers.Api;

[ApiController] // Indicates that the class is an API controller and enables opinionated behaviors
[Route("api/[controller]")] // Defines the base route for all actions in this controller
[Authorize] // Requires authentication for all actions in this controller by default
public class ApiBaseController : ControllerBase { }
