using Microsoft.AspNetCore.Mvc;
using OptimalyBlueprint.Models;
using OptimalyBlueprint.Services;

namespace OptimalyBlueprint.Controllers;

public class BlueprintController : Controller
{
    private readonly IMockDataService _mockDataService;
    private readonly IPricingService _pricingService;

    public BlueprintController(IMockDataService mockDataService, IPricingService pricingService)
    {
        _mockDataService = mockDataService;
        _pricingService = pricingService;
    }

    public IActionResult Index()
    {
        var entities = GetSessionEntities();
        var templates = _mockDataService.GetProjectTemplates();
        
        ViewBag.Templates = templates;
        return View(entities);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var entity = new BlueprintEntity();
        return View(entity);
    }

    [HttpPost]
    public IActionResult Create(BlueprintEntity entity)
    {
        if (ModelState.IsValid)
        {
            entity.Id = new Random().Next(1000, 9999);
            entity.CreatedAt = DateTime.Now;
            entity.EstimatedCost = _pricingService.EstimateEntityCost(entity);
            
            var entities = GetSessionEntities();
            entities.Add(entity);
            SetSessionEntities(entities);
            
            TempData["Success"] = $"Entita '{entity.Name}' byla úspěšně vytvořena!";
            return RedirectToAction(nameof(Index));
        }
        
        return View(entity);
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var entities = GetSessionEntities();
        var entity = entities.FirstOrDefault(e => e.Id == id);
        
        if (entity == null)
        {
            TempData["Error"] = "Entita nebyla nalezena.";
            return RedirectToAction(nameof(Index));
        }
        
        return View(entity);
    }

    [HttpPost]
    public IActionResult Edit(BlueprintEntity entity)
    {
        if (ModelState.IsValid)
        {
            var entities = GetSessionEntities();
            var existingEntity = entities.FirstOrDefault(e => e.Id == entity.Id);
            
            if (existingEntity != null)
            {
                // Update properties
                existingEntity.Name = entity.Name;
                existingEntity.Description = entity.Description;
                existingEntity.Type = entity.Type;
                existingEntity.Properties = entity.Properties;
                existingEntity.Relations = entity.Relations;
                existingEntity.EstimatedCost = _pricingService.EstimateEntityCost(existingEntity);
                
                SetSessionEntities(entities);
                TempData["Success"] = $"Entita '{entity.Name}' byla úspěšně aktualizována!";
            }
            else
            {
                TempData["Error"] = "Entita nebyla nalezena.";
            }
            
            return RedirectToAction(nameof(Index));
        }
        
        return View(entity);
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var entities = GetSessionEntities();
        var entity = entities.FirstOrDefault(e => e.Id == id);
        
        if (entity != null)
        {
            entities.Remove(entity);
            SetSessionEntities(entities);
            TempData["Success"] = $"Entita '{entity.Name}' byla smazána.";
        }
        else
        {
            TempData["Error"] = "Entita nebyla nalezena.";
        }
        
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult LoadTemplate(string templateName)
    {
        var templates = _mockDataService.GetProjectTemplates();
        var template = templates.FirstOrDefault(t => t.Name == templateName);
        
        if (template != null)
        {
            SetSessionEntities(template.PrebuiltEntities);
            TempData["Success"] = $"Template '{templateName}' byl načten s {template.PrebuiltEntities.Count} entitami.";
        }
        else
        {
            TempData["Error"] = "Template nebyl nalezen.";
        }
        
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult ClearAll()
    {
        HttpContext.Session.Remove("BlueprintEntities");
        TempData["Success"] = "Všechny entity byly vymazány.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult QuickAdd(string entityName, EntityType entityType)
    {
        if (string.IsNullOrWhiteSpace(entityName))
        {
            TempData["Error"] = "Název entity je povinný.";
            return RedirectToAction(nameof(Index));
        }

        var entity = _mockDataService.GenerateEntityFromTemplate(entityName, entityType);
        entity.EstimatedCost = _pricingService.EstimateEntityCost(entity);
        
        var entities = GetSessionEntities();
        entities.Add(entity);
        SetSessionEntities(entities);
        
        TempData["Success"] = $"Entita '{entityName}' byla rychle přidána!";
        return RedirectToAction(nameof(Index));
    }

    // AJAX endpoints
    [HttpGet]
    public IActionResult GetEntityTypes()
    {
        var types = Enum.GetValues<EntityType>()
            .Select(t => new { value = (int)t, text = t.ToString() })
            .ToList();
            
        return Json(types);
    }

    [HttpGet]
    public IActionResult GetPropertyTypes()
    {
        var types = Enum.GetValues<PropertyType>()
            .Select(t => new { value = (int)t, text = t.ToString() })
            .ToList();
            
        return Json(types);
    }

    [HttpGet]
    public IActionResult GetRelationTypes()
    {
        var types = Enum.GetValues<RelationType>()
            .Select(t => new { value = (int)t, text = t.ToString() })
            .ToList();
            
        return Json(types);
    }

    [HttpPost]
    public IActionResult EstimateCost([FromBody] BlueprintEntity entity)
    {
        var cost = _pricingService.EstimateEntityCost(entity);
        return Json(new { cost = cost, formatted = cost.ToString("C") });
    }

    [HttpGet]
    public IActionResult GenerateMockData(PropertyType propertyType, int count = 5)
    {
        var mockData = _mockDataService.GenerateMockDataForProperty(propertyType, count);
        return Json(mockData);
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

    private void SetSessionEntities(List<BlueprintEntity> entities)
    {
        var entitiesJson = System.Text.Json.JsonSerializer.Serialize(entities);
        HttpContext.Session.SetString("BlueprintEntities", entitiesJson);
    }
}