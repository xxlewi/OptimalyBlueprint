using Microsoft.AspNetCore.Mvc;

namespace OptimalyBlueprint.Controllers;

public class BooksController : BaseController
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
    
    public IActionResult Inventory()
    {
        return View();
    }
}