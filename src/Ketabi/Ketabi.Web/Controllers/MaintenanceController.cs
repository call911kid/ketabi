using Microsoft.AspNetCore.Mvc;

namespace Ketabi.Web.Controllers;

public class MaintenanceController : BaseController
{
    [HttpGet("/Maintenance/UnderDevelopment")]
    [HttpGet("/UnderDevelopment")]
    public IActionResult UnderDevelopment()
    {
        ViewData["Title"] = "Under Development";
        return View();
    }
}
