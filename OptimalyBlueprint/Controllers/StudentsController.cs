using Microsoft.AspNetCore.Mvc;

namespace OptimalyBlueprint.Controllers;

public class StudentsController : Controller
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
    
    public IActionResult Details(int id)
    {
        return View();
    }
    
    public IActionResult Responsibility()
    {
        return View();
    }
}