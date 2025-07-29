using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OptimalyBlueprint.Models;
using OptimalyBlueprint.Services;

namespace OptimalyBlueprint.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IMockDataService _mockDataService;
    private readonly IPricingService _pricingService;

    public HomeController(ILogger<HomeController> logger, IMockDataService mockDataService, IPricingService pricingService)
    {
        _logger = logger;
        _mockDataService = mockDataService;
        _pricingService = pricingService;
    }

    public IActionResult Index()
    {
        var templates = _mockDataService.GetProjectTemplates();
        var entities = GetSessionEntities();
        
        ViewBag.Templates = templates;
        ViewBag.EntityCount = entities.Count;
        ViewBag.HasEntities = entities.Any();
        
        if (entities.Any())
        {
            var pricing = _pricingService.CalculatePricing(entities, "Current Project", "Client");
            ViewBag.EstimatedCost = pricing.TotalCost;
            ViewBag.EstimatedHours = pricing.TotalHours;
        }
        
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    
    public IActionResult About()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
    private List<BlueprintEntity> GetSessionEntities()
    {
        var entitiesJson = HttpContext.Session.GetString("BlueprintEntities");
        if (string.IsNullOrEmpty(entitiesJson))
        {
            return new List<BlueprintEntity>();
        }
        
        return System.Text.Json.JsonSerializer.Deserialize<List<BlueprintEntity>>(entitiesJson) ?? new List<BlueprintEntity>();
    }
}
