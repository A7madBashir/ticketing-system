using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TicketingSystem.Models.Common;

namespace TicketingSystem.Controllers.Web;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Roles = Roles.Admin)]
public class WebBaseController : Controller
{
    public override RedirectToActionResult RedirectToAction(string? actionName)
    {
        string controllerName =
            "/" + this.ControllerContext.RouteData.Values["controller"]!.ToString()!;
        return base.RedirectToAction(actionName, controllerName);
    }
}
