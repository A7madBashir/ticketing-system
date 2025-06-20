using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TicketingSystem.Controllers.Web;
using TicketingSystem.Models;

namespace TicketingSystem.Controllers;

[Route("")]
public class HomeController(ILogger<HomeController> logger) : WebBaseController
{
    private readonly ILogger<HomeController> _logger = logger;

    public IActionResult Index()
    {
        return View();
    }
}
