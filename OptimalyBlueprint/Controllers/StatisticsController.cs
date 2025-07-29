using Microsoft.AspNetCore.Mvc;

namespace OptimalyBlueprint.Controllers;

public class StatisticsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
    
    public IActionResult Reports()
    {
        return View();
    }
    
    public IActionResult ExcelExport()
    {
        return View();
    }
    
    public IActionResult PopularGenres()
    {
        return View();
    }
}