using Microsoft.AspNetCore.Mvc;

namespace OptimalyBlueprint.Controllers;

public class LoansController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
    
    public IActionResult New()
    {
        return View();
    }
    
    public IActionResult Return()
    {
        return View();
    }
    
    public IActionResult Overdue()
    {
        return View();
    }
    
    public IActionResult Management()
    {
        return View();
    }
    
    public IActionResult Today()
    {
        return View();
    }
}