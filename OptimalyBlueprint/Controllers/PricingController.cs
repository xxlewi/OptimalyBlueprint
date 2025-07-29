using Microsoft.AspNetCore.Mvc;
using OptimalyBlueprint.Models;
using OptimalyBlueprint.Services;
using System.Text;

namespace OptimalyBlueprint.Controllers;

public class PricingController : Controller
{
    private readonly IPricingService _pricingService;
    private readonly IMockDataService _mockDataService;

    public PricingController(IPricingService pricingService, IMockDataService mockDataService)
    {
        _pricingService = pricingService;
        _mockDataService = mockDataService;
    }

    public IActionResult Index(string projectName = "Nový Projekt", string clientName = "Klient")
    {
        var entities = GetSessionEntities();
        
        if (!entities.Any())
        {
            TempData["Warning"] = "Nejdříve vytvořte některé entity v Blueprint designeru.";
            return RedirectToAction("Index", "Blueprint");
        }

        var pricing = _pricingService.CalculatePricing(entities, projectName, clientName);
        return View(pricing);
    }

    [HttpPost]
    public IActionResult Calculate(string projectName, string clientName)
    {
        var entities = GetSessionEntities();
        
        if (!entities.Any())
        {
            TempData["Error"] = "Žádné entity k výpočtu ceny.";
            return RedirectToAction("Index", "Blueprint");
        }

        var pricing = _pricingService.CalculatePricing(entities, projectName, clientName);
        
        // Store in session for export
        SetSessionPricing(pricing);
        
        return View("Index", pricing);
    }

    [HttpGet]
    public IActionResult ExportReport()
    {
        var pricing = GetSessionPricing();
        
        if (pricing == null)
        {
            TempData["Error"] = "Nejdříve vygenerujte cenovou nabídku.";
            return RedirectToAction(nameof(Index));
        }

        var report = _pricingService.GeneratePricingReport(pricing);
        var fileName = $"Cenova_nabidka_{pricing.ProjectName}_{DateTime.Now:yyyyMMdd}.md";
        
        var bytes = Encoding.UTF8.GetBytes(report);
        return File(bytes, "text/markdown", fileName);
    }

    [HttpGet]
    public IActionResult Preview()
    {
        var pricing = GetSessionPricing();
        
        if (pricing == null)
        {
            TempData["Error"] = "Nejdříve vygenerujte cenovou nabídku.";
            return RedirectToAction(nameof(Index));
        }

        var report = _pricingService.GeneratePricingReport(pricing);
        ViewBag.Report = report;
        
        return View(pricing);
    }

    [HttpGet]
    public IActionResult Compare()
    {
        var entities = GetSessionEntities();
        var templates = _mockDataService.GetProjectTemplates();
        
        var comparisons = new List<TemplateComparison>();
        
        foreach (var template in templates)
        {
            var templatePricing = _pricingService.CalculatePricing(template.PrebuiltEntities, template.Name, "Template");
            comparisons.Add(new TemplateComparison
            {
                Template = template,
                Pricing = templatePricing,
                EntityCount = template.PrebuiltEntities.Count,
                IsCurrentProject = false
            });
        }
        
        if (entities.Any())
        {
            var currentPricing = _pricingService.CalculatePricing(entities, "Váš projekt", "Aktuální");
            comparisons.Insert(0, new TemplateComparison
            {
                Template = new ProjectTemplate { Name = "Váš aktuální projekt", Description = "Aktuálně navržené entity" },
                Pricing = currentPricing,
                EntityCount = entities.Count,
                IsCurrentProject = true
            });
        }
        
        return View(comparisons);
    }

    // AJAX endpoints
    [HttpPost]
    public IActionResult GetQuickEstimate([FromBody] QuickEstimateRequest request)
    {
        var mockEntities = new List<BlueprintEntity>();
        
        for (int i = 0; i < request.EntityCount; i++)
        {
            mockEntities.Add(_mockDataService.GenerateEntityFromTemplate($"Entity{i + 1}", EntityType.Core));
        }
        
        var pricing = _pricingService.CalculatePricing(mockEntities, "Quick Estimate", "Client");
        
        return Json(new
        {
            totalCost = pricing.TotalCost,
            totalHours = pricing.TotalHours,
            formattedCost = pricing.TotalCost.ToString("C"),
            breakdown = new
            {
                entities = pricing.Breakdown.EntitiesCost,
                ui = pricing.Breakdown.UICost,
                security = pricing.Breakdown.SecurityCost,
                testing = pricing.Breakdown.TestingCost
            }
        });
    }

    [HttpGet]
    public IActionResult GetPricingBreakdown()
    {
        var entities = GetSessionEntities();
        
        if (!entities.Any())
        {
            return Json(new { error = "No entities found" });
        }

        var breakdown = _pricingService.CalculateDetailedBreakdown(entities);
        
        return Json(new
        {
            entities = breakdown.EntitiesCost,
            relations = breakdown.RelationsCost,
            ui = breakdown.UICost,
            security = breakdown.SecurityCost,
            testing = breakdown.TestingCost,
            deployment = breakdown.DeploymentCost,
            total = breakdown.CalculateTotal(),
            totalHours = breakdown.CalculateTotalHours(),
            items = breakdown.DetailedItems.Select(item => new
            {
                category = item.Category,
                description = item.Description,
                quantity = item.Quantity,
                unitPrice = item.UnitPrice,
                totalPrice = item.TotalPrice,
                hours = item.Hours
            })
        });
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

    private PricingModel? GetSessionPricing()
    {
        var pricingJson = HttpContext.Session.GetString("CurrentPricing");
        if (string.IsNullOrEmpty(pricingJson))
        {
            return null;
        }
        
        return System.Text.Json.JsonSerializer.Deserialize<PricingModel>(pricingJson);
    }

    private void SetSessionPricing(PricingModel pricing)
    {
        var pricingJson = System.Text.Json.JsonSerializer.Serialize(pricing);
        HttpContext.Session.SetString("CurrentPricing", pricingJson);
    }
}

public class QuickEstimateRequest
{
    public int EntityCount { get; set; }
    public bool IncludeSecurity { get; set; } = true;
    public bool IncludeUI { get; set; } = true;
}

public class TemplateComparison
{
    public ProjectTemplate Template { get; set; } = new();
    public PricingModel Pricing { get; set; } = new();
    public int EntityCount { get; set; }
    public bool IsCurrentProject { get; set; }
}