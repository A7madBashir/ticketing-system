using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TicketingSystem.Controllers.Web;

public class SubscriptionsController : WebBaseController
{
    public IActionResult Index()
    {
        return View();
    }
}
