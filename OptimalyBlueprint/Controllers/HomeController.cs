using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OptimalyBlueprint.Models;

namespace OptimalyBlueprint.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        // Handle AJAX requests
        if (Request.Headers.ContainsKey("X-Requested-With"))
        {
            return Json(new { success = true, message = "Dashboard načten úspěšně" });
        }
        
        return View();
    }

    public IActionResult Privacy()
    {
        // Handle AJAX requests
        if (Request.Headers.ContainsKey("X-Requested-With"))
        {
            return Json(new { success = false, message = "Stránka Privacy zatím není implementována" });
        }
        
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
