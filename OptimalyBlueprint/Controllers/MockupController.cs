using Microsoft.AspNetCore.Mvc;
using OptimalyBlueprint.Models;
using OptimalyBlueprint.Services;

namespace OptimalyBlueprint.Controllers;

public class MockupController : Controller
{
    private readonly IMockDataService _mockDataService;

    public MockupController(IMockDataService mockDataService)
    {
        _mockDataService = mockDataService;
    }

    public IActionResult Index()
    {
        var entities = GetSessionEntities();
        
        if (!entities.Any())
        {
            TempData["Warning"] = "Nejdříve vytvořte některé entity v Blueprint designeru.";
            return RedirectToAction("Index", "Blueprint");
        }

        var mockupProject = GetOrCreateMockupProject(entities);
        return View(mockupProject);
    }

    [HttpGet]
    public IActionResult Preview(string pageName)
    {
        var mockupProject = GetSessionMockupProject();
        
        if (mockupProject == null)
        {
            return RedirectToAction(nameof(Index));
        }

        var page = mockupProject.Pages.FirstOrDefault(p => p.Name == pageName);
        if (page == null)
        {
            TempData["Error"] = "Stránka nebyla nalezena.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.MockupProject = mockupProject;
        return View(page);
    }

    [HttpGet]
    public IActionResult EntityList(string entityName)
    {
        var entities = GetSessionEntities();
        var entity = entities.FirstOrDefault(e => e.Name.Equals(entityName, StringComparison.OrdinalIgnoreCase));
        
        if (entity == null)
        {
            TempData["Error"] = "Entita nebyla nalezena.";
            return RedirectToAction(nameof(Index));
        }

        // Generování mock dat pro entitu
        var mockData = GenerateMockDataForEntity(entity, 10);
        
        ViewBag.Entity = entity;
        ViewBag.MockData = mockData;
        
        return View(mockData);
    }

    [HttpGet]
    public IActionResult EntityForm(string entityName, int? id = null)
    {
        var entities = GetSessionEntities();
        var entity = entities.FirstOrDefault(e => e.Name.Equals(entityName, StringComparison.OrdinalIgnoreCase));
        
        if (entity == null)
        {
            TempData["Error"] = "Entita nebyla nalezena.";
            return RedirectToAction(nameof(Index));
        }

        var mockFields = GenerateMockFieldsForEntity(entity);
        
        ViewBag.Entity = entity;
        ViewBag.IsEdit = id.HasValue;
        ViewBag.RecordId = id;
        
        return View(mockFields);
    }

    [HttpGet]
    public IActionResult EntityDetail(string entityName, int id)
    {
        var entities = GetSessionEntities();
        var entity = entities.FirstOrDefault(e => e.Name.Equals(entityName, StringComparison.OrdinalIgnoreCase));
        
        if (entity == null)
        {
            TempData["Error"] = "Entita nebyla nalezena.";
            return RedirectToAction(nameof(Index));
        }

        var mockRecord = GenerateMockDataForEntity(entity, 1).First();
        mockRecord["Id"] = id.ToString();
        
        ViewBag.Entity = entity;
        
        return View(mockRecord);
    }

    [HttpGet]
    public IActionResult Dashboard()
    {
        var entities = GetSessionEntities();
        var dashboardData = GenerateDashboardData(entities);
        
        return View(dashboardData);
    }

    [HttpPost]
    public IActionResult CustomizeTheme(MockupTheme theme)
    {
        var mockupProject = GetSessionMockupProject();
        
        if (mockupProject != null)
        {
            mockupProject.Theme = theme;
            SetSessionMockupProject(mockupProject);
            TempData["Success"] = "Téma bylo aktualizováno.";
        }
        
        return RedirectToAction(nameof(Index));
    }

    // AJAX endpoints
    [HttpGet]
    public IActionResult GetMockData(string entityName, int count = 5)
    {
        var entities = GetSessionEntities();
        var entity = entities.FirstOrDefault(e => e.Name.Equals(entityName, StringComparison.OrdinalIgnoreCase));
        
        if (entity == null)
        {
            return Json(new { error = "Entity not found" });
        }

        var mockData = GenerateMockDataForEntity(entity, count);
        return Json(mockData);
    }

    [HttpPost]
    public IActionResult GenerateComponent([FromBody] ComponentRequest request)
    {
        var entities = GetSessionEntities();
        var entity = entities.FirstOrDefault(e => e.Name.Equals(request.EntityName, StringComparison.OrdinalIgnoreCase));
        
        if (entity == null)
        {
            return Json(new { error = "Entity not found" });
        }

        var component = GenerateComponentForEntity(entity, request.ComponentType);
        
        return Json(new
        {
            name = component.Name,
            type = component.Type.ToString(),
            html = component.PreviewHtml,
            cost = component.DevelopmentCost,
            hours = component.DevelopmentHours
        });
    }

    private MockupProject GetOrCreateMockupProject(List<BlueprintEntity> entities)
    {
        var existing = GetSessionMockupProject();
        if (existing != null)
        {
            return existing;
        }

        var project = _mockDataService.GenerateMockupProject("Nový Projekt", entities);
        SetSessionMockupProject(project);
        
        return project;
    }

    private List<Dictionary<string, string>> GenerateMockDataForEntity(BlueprintEntity entity, int count)
    {
        var records = new List<Dictionary<string, string>>();
        
        for (int i = 1; i <= count; i++)
        {
            var record = new Dictionary<string, string>
            {
                ["Id"] = i.ToString()
            };
            
            foreach (var property in entity.Properties)
            {
                var mockValues = _mockDataService.GenerateMockDataForProperty(property.Type, 1);
                record[property.Name] = mockValues.FirstOrDefault() ?? "N/A";
            }
            
            records.Add(record);
        }
        
        return records;
    }

    private List<MockField> GenerateMockFieldsForEntity(BlueprintEntity entity)
    {
        var fields = new List<MockField>();
        
        foreach (var property in entity.Properties)
        {
            var mockValues = _mockDataService.GenerateMockDataForProperty(property.Type, 1);
            
            fields.Add(new MockField
            {
                Name = property.Name,
                Label = property.Name,
                Type = ConvertToFieldType(property.Type),
                IsRequired = property.IsRequired,
                PlaceholderText = $"Zadejte {property.Name.ToLower()}",
                MockValue = mockValues.FirstOrDefault() ?? ""
            });
        }
        
        return fields;
    }

    private MockupComponent GenerateComponentForEntity(BlueprintEntity entity, ComponentType type)
    {
        var component = new MockupComponent
        {
            Name = $"{entity.Name} {type}",
            Type = type,
            EntityName = entity.Name,
            DevelopmentCost = GetComponentCost(type),
            DevelopmentHours = GetComponentHours(type)
        };

        component.PreviewHtml = type switch
        {
            ComponentType.List => GenerateListComponentHtml(entity),
            ComponentType.Form => GenerateFormComponentHtml(entity),
            ComponentType.Detail => GenerateDetailComponentHtml(entity),
            ComponentType.Dashboard => GenerateDashboardComponentHtml(entity),
            _ => $"<div class='component-placeholder'>{type} component for {entity.Name}</div>"
        };

        return component;
    }

    private DashboardData GenerateDashboardData(List<BlueprintEntity> entities)
    {
        var random = new Random();
        
        return new DashboardData
        {
            TotalEntities = entities.Count,
            TotalRecords = entities.Sum(e => random.Next(50, 500)),
            ActiveUsers = random.Next(10, 100),
            RecentActivity = GenerateRecentActivity(entities, 5),
            EntityStats = entities.Select(e => new EntityStat
            {
                Name = e.Name,
                Count = random.Next(10, 200),
                TrendPercentage = random.Next(-20, 50)
            }).ToList()
        };
    }

    private List<ActivityItem> GenerateRecentActivity(List<BlueprintEntity> entities, int count)
    {
        var activities = new List<ActivityItem>();
        var random = new Random();
        var actions = new[] { "vytvořil", "upravil", "smazal", "zobrazil" };
        
        for (int i = 0; i < count; i++)
        {
            var entity = entities[random.Next(entities.Count)];
            var action = actions[random.Next(actions.Length)];
            
            activities.Add(new ActivityItem
            {
                Description = $"Uživatel {action} {entity.Name}",
                Timestamp = DateTime.Now.AddMinutes(-random.Next(1, 60)),
                EntityName = entity.Name,
                Action = action
            });
        }
        
        return activities.OrderByDescending(a => a.Timestamp).ToList();
    }

    private FieldType ConvertToFieldType(PropertyType propertyType) => propertyType switch
    {
        PropertyType.String => FieldType.Text,
        PropertyType.Integer => FieldType.Number,
        PropertyType.Decimal => FieldType.Number,
        PropertyType.DateTime => FieldType.Date,
        PropertyType.Boolean => FieldType.Checkbox,
        PropertyType.Email => FieldType.Email,
        PropertyType.Phone => FieldType.Text,
        PropertyType.Url => FieldType.Text,
        PropertyType.Json => FieldType.Textarea,
        PropertyType.File => FieldType.File,
        _ => FieldType.Text
    };

    private decimal GetComponentCost(ComponentType type) => type switch
    {
        ComponentType.List => 200m,
        ComponentType.Form => 300m,
        ComponentType.Detail => 150m,
        ComponentType.Dashboard => 500m,
        ComponentType.Search => 250m,
        ComponentType.Navigation => 100m,
        ComponentType.Report => 400m,
        ComponentType.Chart => 300m,
        _ => 200m
    };

    private int GetComponentHours(ComponentType type) => type switch
    {
        ComponentType.List => 4,
        ComponentType.Form => 6,
        ComponentType.Detail => 3,
        ComponentType.Dashboard => 10,
        ComponentType.Search => 5,
        ComponentType.Navigation => 2,
        ComponentType.Report => 8,
        ComponentType.Chart => 6,
        _ => 4
    };

    private string GenerateListComponentHtml(BlueprintEntity entity)
    {
        var properties = entity.Properties.Take(4); // Show first 4 properties
        var headers = string.Join("", properties.Select(p => $"<th>{p.Name}</th>"));
        
        return $@"
<div class='table-responsive'>
    <table class='table table-striped'>
        <thead>
            <tr>
                <th>ID</th>
                {headers}
                <th>Akce</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>1</td>
                {string.Join("", properties.Select(p => "<td>Sample data</td>"))}
                <td>
                    <button class='btn btn-sm btn-primary'>Edit</button>
                    <button class='btn btn-sm btn-danger'>Delete</button>
                </td>
            </tr>
        </tbody>
    </table>
</div>";
    }

    private string GenerateFormComponentHtml(BlueprintEntity entity)
    {
        var fields = entity.Properties.Take(6).Select(p => $@"
<div class='form-group'>
    <label>{p.Name}</label>
    <input type='text' class='form-control' placeholder='{p.Name}' {(p.IsRequired ? "required" : "")} />
</div>").ToList();

        return $@"
<form>
    {string.Join("", fields)}
    <div class='form-group'>
        <button type='submit' class='btn btn-primary'>Uložit</button>
        <button type='button' class='btn btn-secondary'>Zrušit</button>
    </div>
</form>";
    }

    private string GenerateDetailComponentHtml(BlueprintEntity entity)
    {
        var fields = entity.Properties.Take(6).Select(p => $@"
<div class='row mb-2'>
    <div class='col-sm-3'><strong>{p.Name}:</strong></div>
    <div class='col-sm-9'>Sample {p.Name.ToLower()} value</div>
</div>").ToList();

        return $@"
<div class='card'>
    <div class='card-header'>
        <h3>{entity.Name} Detail</h3>
    </div>
    <div class='card-body'>
        {string.Join("", fields)}
    </div>
</div>";
    }

    private string GenerateDashboardComponentHtml(BlueprintEntity entity)
    {
        return $@"
<div class='row'>
    <div class='col-md-3'>
        <div class='card bg-primary text-white'>
            <div class='card-body'>
                <h4>125</h4>
                <p>Total {entity.Name}s</p>
            </div>
        </div>
    </div>
    <div class='col-md-9'>
        <div class='card'>
            <div class='card-header'>{entity.Name} Statistics</div>
            <div class='card-body'>
                <canvas id='chart-{entity.Name.ToLower()}' width='400' height='200'></canvas>
            </div>
        </div>
    </div>
</div>";
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

    private MockupProject? GetSessionMockupProject()
    {
        var projectJson = HttpContext.Session.GetString("MockupProject");
        if (string.IsNullOrEmpty(projectJson))
        {
            return null;
        }
        
        return System.Text.Json.JsonSerializer.Deserialize<MockupProject>(projectJson);
    }

    private void SetSessionMockupProject(MockupProject project)
    {
        var projectJson = System.Text.Json.JsonSerializer.Serialize(project);
        HttpContext.Session.SetString("MockupProject", projectJson);
    }
}

public class ComponentRequest
{
    public string EntityName { get; set; } = string.Empty;
    public ComponentType ComponentType { get; set; }
}

public class DashboardData
{
    public int TotalEntities { get; set; }
    public int TotalRecords { get; set; }
    public int ActiveUsers { get; set; }
    public List<ActivityItem> RecentActivity { get; set; } = new();
    public List<EntityStat> EntityStats { get; set; } = new();
}

public class ActivityItem
{
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
}

public class EntityStat
{
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
    public int TrendPercentage { get; set; }
}