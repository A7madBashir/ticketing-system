using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TicketingSystem.Models.Common;

namespace TicketingSystem.Controllers.Web;

[Route("[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Roles = Roles.Admin)]
public class WebBaseController : Controller { }
