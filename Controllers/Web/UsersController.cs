using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TicketingSystem.Controllers.Web;

public class UsersController(ILogger<UsersController> logger) : WebBaseController
{
    private readonly ILogger<UsersController> _logger = logger;

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Create()
    {
        return View();
    }

    public IActionResult Edit()
    {
        return View();
    }
}
