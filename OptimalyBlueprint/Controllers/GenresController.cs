using Microsoft.AspNetCore.Mvc;

namespace OptimalyBlueprint.Controllers;

public class GenresController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
    
    public IActionResult Create()
    {
        return View();
    }
    
    public IActionResult Edit(int id)
    {
        return View();
    }
}