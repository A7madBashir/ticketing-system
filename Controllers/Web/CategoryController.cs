using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TicketingSystem.Models.Common;

namespace TicketingSystem.Controllers.Web;

[Authorize(Policy = AuthenticationPolicy.AgentAccess)]
public class CategoryController(ILogger<CategoryController> logger) : WebBaseController
{
    private readonly ILogger<CategoryController> _logger = logger;

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
