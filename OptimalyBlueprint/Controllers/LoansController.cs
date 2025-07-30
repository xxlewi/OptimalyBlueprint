using Microsoft.AspNetCore.Mvc;

namespace OptimalyBlueprint.Controllers;

public class LoansController : BaseController
{
    public IActionResult Index()
    {
        if (Request.Headers.ContainsKey("X-Requested-With"))
        {
            return View();
        }
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