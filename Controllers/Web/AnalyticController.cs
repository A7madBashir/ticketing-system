using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace TicketingSystem.Controllers.Web
{
    public class AnalyticController(ILogger<AnalyticController> logger) : WebBaseController
    {
         private readonly ILogger<AnalyticController> _logger = logger;

    public IActionResult Index()
    {
        return View();
    }

    }

}