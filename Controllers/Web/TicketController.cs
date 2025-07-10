using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace TicketingSystem.Controllers.Web;

public class TicketController(ILogger<TicketController> logger) : WebBaseController
{
    private readonly ILogger<TicketController> _logger = logger;

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
