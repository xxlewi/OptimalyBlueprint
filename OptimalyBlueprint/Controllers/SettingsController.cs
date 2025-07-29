using Microsoft.AspNetCore.Mvc;

namespace OptimalyBlueprint.Controllers;

public class SettingsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}