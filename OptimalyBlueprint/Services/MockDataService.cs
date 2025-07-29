using OptimalyBlueprint.Models;

namespace OptimalyBlueprint.Services;

public interface IMockDataService
{
    List<BlueprintEntity> GetSampleEntities();
    List<ProjectTemplate> GetProjectTemplates();
    BlueprintEntity GenerateEntityFromTemplate(string entityName, EntityType type);
    List<string> GenerateMockDataForProperty(PropertyType type, int count = 10);
    MockupProject GenerateMockupProject(string projectName, List<BlueprintEntity> entities);
}

public class MockDataService : IMockDataService
{
    private readonly Random _random = new();
    
    private readonly List<string> _sampleNames = new()
    {
        "User", "Product", "Order", "Customer", "Invoice", "Category", 
        "Article", "Comment", "Review", "Event", "Booking", "Payment"
    };
    
    private readonly List<string> _sampleCompanies = new()
    {
        "TechCorp s.r.o.", "InnoSoft", "DataFlow Solutions", "CloudTech",
        "SmartBusiness", "DigitalPro", "ModernApps", "FutureSoft"
    };
    
    private readonly List<string> _sampleFirstNames = new()
    {
        "Jan", "Petr", "Pavel", "Tomáš", "Jiří", "Michal", "Lukáš", "Martin",
        "Anna", "Jana", "Tereza", "Kateřina", "Lenka", "Petra", "Hana", "Eva"
    };
    
    private readonly List<string> _sampleLastNames = new()
    {
        "Novák", "Svoboda", "Novotný", "Dvořák", "Černý", "Procházka",
        "Krejčí", "Horák", "Němec", "Pokorný", "Veselý", "Marek"
    };

    public List<BlueprintEntity> GetSampleEntities()
    {
        return new List<BlueprintEntity>
        {
            new BlueprintEntity
            {
                Id = 1,
                Name = "User",
                Description = "Systémový uživatel s přihlašovacími údaji",
                Type = EntityType.Core,
                EstimatedCost = 500,
                DevelopmentHours = 8,
                Properties = new List<BlueprintProperty>
                {
                    new() { Name = "FirstName", Type = PropertyType.String, IsRequired = true, MaxLength = 100 },
                    new() { Name = "LastName", Type = PropertyType.String, IsRequired = true, MaxLength = 100 },
                    new() { Name = "Email", Type = PropertyType.Email, IsRequired = true, IsUnique = true },
                    new() { Name = "IsActive", Type = PropertyType.Boolean, DefaultValue = "true" }
                }
            },
            new BlueprintEntity
            {
                Id = 2,
                Name = "Product",
                Description = "Produktový katalog s cenami a skladem",
                Type = EntityType.Core,
                EstimatedCost = 500,
                DevelopmentHours = 12,
                Properties = new List<BlueprintProperty>
                {
                    new() { Name = "Name", Type = PropertyType.String, IsRequired = true, MaxLength = 200 },
                    new() { Name = "Price", Type = PropertyType.Decimal, IsRequired = true },
                    new() { Name = "Stock", Type = PropertyType.Integer, DefaultValue = "0" },
                    new() { Name = "IsAvailable", Type = PropertyType.Boolean, DefaultValue = "true" }
                },
                Relations = new List<BlueprintRelation>
                {
                    new() { Name = "Category", RelatedEntityName = "Category", Type = RelationType.OneToMany, AdditionalCost = 300 }
                }
            },
            new BlueprintEntity
            {
                Id = 3,
                Name = "Order",
                Description = "Objednávky zákazníků",
                Type = EntityType.Complex,
                EstimatedCost = 800,
                DevelopmentHours = 20,
                Properties = new List<BlueprintProperty>
                {
                    new() { Name = "OrderNumber", Type = PropertyType.String, IsRequired = true, IsUnique = true },
                    new() { Name = "OrderDate", Type = PropertyType.DateTime, IsRequired = true },
                    new() { Name = "TotalAmount", Type = PropertyType.Decimal, IsRequired = true },
                    new() { Name = "Status", Type = PropertyType.String, IsRequired = true }
                },
                Relations = new List<BlueprintRelation>
                {
                    new() { Name = "Customer", RelatedEntityName = "User", Type = RelationType.OneToMany, AdditionalCost = 300 },
                    new() { Name = "Products", RelatedEntityName = "Product", Type = RelationType.ManyToMany, AdditionalCost = 500 }
                }
            }
        };
    }

    public List<ProjectTemplate> GetProjectTemplates()
    {
        return new List<ProjectTemplate>
        {
            new ProjectTemplate
            {
                Name = "E-commerce",
                Description = "Kompletní e-shop s košíkem a platbami",
                Industry = "Retail",
                BasePrice = 15000,
                IconClass = "fas fa-shopping-cart",
                PrebuiltEntities = GenerateEcommerceEntities()
            },
            new ProjectTemplate
            {
                Name = "CRM System",
                Description = "Správa zákazníků a obchodních příležitostí",
                Industry = "Business",
                BasePrice = 12000,
                IconClass = "fas fa-users",
                PrebuiltEntities = GenerateCrmEntities()
            },
            new ProjectTemplate
            {
                Name = "Blog Platform",
                Description = "Publikační systém s komentáři",
                Industry = "Media",
                BasePrice = 8000,
                IconClass = "fas fa-blog",
                PrebuiltEntities = GenerateBlogEntities()
            },
            new ProjectTemplate
            {
                Name = "Event Management",
                Description = "Správa událostí a rezervací",
                Industry = "Events",
                BasePrice = 10000,
                IconClass = "fas fa-calendar",
                PrebuiltEntities = GenerateEventEntities()
            }
        };
    }

    public BlueprintEntity GenerateEntityFromTemplate(string entityName, EntityType type)
    {
        var properties = GeneratePropertiesForEntityType(type);
        
        return new BlueprintEntity
        {
            Id = _random.Next(1000, 9999),
            Name = entityName,
            Description = $"Automaticky generovaná entita typu {type}",
            Type = type,
            EstimatedCost = GetBaseCostForEntityType(type),
            DevelopmentHours = GetBaseHoursForEntityType(type),
            Properties = properties,
            CreatedAt = DateTime.Now
        };
    }

    public List<string> GenerateMockDataForProperty(PropertyType type, int count = 10)
    {
        var data = new List<string>();
        
        for (int i = 0; i < count; i++)
        {
            data.Add(type switch
            {
                PropertyType.String => GenerateRandomString(),
                PropertyType.Integer => _random.Next(1, 1000).ToString(),
                PropertyType.Decimal => (_random.NextDouble() * 1000).ToString("F2"),
                PropertyType.DateTime => DateTime.Now.AddDays(_random.Next(-365, 365)).ToString("yyyy-MM-dd"),
                PropertyType.Boolean => (_random.Next(0, 2) == 1).ToString(),
                PropertyType.Email => GenerateRandomEmail(),
                PropertyType.Phone => GenerateRandomPhone(),
                PropertyType.Url => $"https://example{i}.com",
                _ => $"Sample data {i}"
            });
        }
        
        return data;
    }

    public MockupProject GenerateMockupProject(string projectName, List<BlueprintEntity> entities)
    {
        var project = new MockupProject
        {
            Name = projectName,
            Description = $"Automaticky generovaný mockup pro projekt {projectName}",
            ClientName = GetRandomCompany(),
            CreatedAt = DateTime.Now,
            Theme = new MockupTheme
            {
                CompanyName = projectName,
                PrimaryColor = GetRandomColor(),
                SecondaryColor = "#6c757d"
            }
        };

        // Generování stránek pro každou entitu
        foreach (var entity in entities)
        {
            project.Pages.AddRange(GeneratePagesForEntity(entity));
        }

        // Dashboard jako úvodní stránka
        project.Pages.Insert(0, new MockupPage
        {
            Name = "Dashboard",
            Route = "/",
            Type = PageType.Dashboard,
            Description = "Hlavní přehledová stránka",
            Components = new List<MockupComponent>
            {
                new MockupComponent
                {
                    Name = "Statistics Overview",
                    Type = ComponentType.Dashboard,
                    DevelopmentCost = 500,
                    DevelopmentHours = 8
                }
            }
        });

        return project;
    }

    private List<BlueprintEntity> GenerateEcommerceEntities()
    {
        return new List<BlueprintEntity>
        {
            GenerateEntityFromTemplate("Product", EntityType.Core),
            GenerateEntityFromTemplate("Category", EntityType.Lookup),
            GenerateEntityFromTemplate("Order", EntityType.Complex),
            GenerateEntityFromTemplate("Customer", EntityType.Core),
            GenerateEntityFromTemplate("Payment", EntityType.Complex)
        };
    }

    private List<BlueprintEntity> GenerateCrmEntities()
    {
        return new List<BlueprintEntity>
        {
            GenerateEntityFromTemplate("Contact", EntityType.Core),
            GenerateEntityFromTemplate("Company", EntityType.Core),
            GenerateEntityFromTemplate("Opportunity", EntityType.Complex),
            GenerateEntityFromTemplate("Activity", EntityType.Audit)
        };
    }

    private List<BlueprintEntity> GenerateBlogEntities()
    {
        return new List<BlueprintEntity>
        {
            GenerateEntityFromTemplate("Article", EntityType.Core),
            GenerateEntityFromTemplate("Comment", EntityType.Core),
            GenerateEntityFromTemplate("Tag", EntityType.Lookup),
            GenerateEntityFromTemplate("Author", EntityType.Core)
        };
    }

    private List<BlueprintEntity> GenerateEventEntities()
    {
        return new List<BlueprintEntity>
        {
            GenerateEntityFromTemplate("Event", EntityType.Core),
            GenerateEntityFromTemplate("Booking", EntityType.Complex),
            GenerateEntityFromTemplate("Venue", EntityType.Lookup),
            GenerateEntityFromTemplate("Participant", EntityType.Core)
        };
    }

    private List<MockupPage> GeneratePagesForEntity(BlueprintEntity entity)
    {
        return new List<MockupPage>
        {
            new MockupPage
            {
                Name = $"{entity.Name} List",
                Route = $"/{entity.Name.ToLower()}",
                Type = PageType.List,
                Description = $"Seznam všech {entity.Name}",
                Components = new List<MockupComponent>
                {
                    new MockupComponent
                    {
                        Name = $"{entity.Name} Table",
                        Type = ComponentType.List,
                        EntityName = entity.Name,
                        DevelopmentCost = 200,
                        DevelopmentHours = 4
                    }
                }
            },
            new MockupPage
            {
                Name = $"Create {entity.Name}",
                Route = $"/{entity.Name.ToLower()}/create",
                Type = PageType.Create,
                Description = $"Vytvoření nového {entity.Name}",
                Components = new List<MockupComponent>
                {
                    new MockupComponent
                    {
                        Name = $"{entity.Name} Form",
                        Type = ComponentType.Form,
                        EntityName = entity.Name,
                        DevelopmentCost = 300,
                        DevelopmentHours = 6
                    }
                }
            }
        };
    }

    private List<BlueprintProperty> GeneratePropertiesForEntityType(EntityType type)
    {
        var baseProperties = new List<BlueprintProperty>
        {
            new() { Name = "Id", Type = PropertyType.Integer, IsRequired = true },
            new() { Name = "Name", Type = PropertyType.String, IsRequired = true, MaxLength = 100 },
            new() { Name = "CreatedAt", Type = PropertyType.DateTime, IsRequired = true }
        };

        return type switch
        {
            EntityType.Core => baseProperties.Concat(new[]
            {
                new BlueprintProperty { Name = "Description", Type = PropertyType.String, MaxLength = 500 },
                new BlueprintProperty { Name = "IsActive", Type = PropertyType.Boolean, DefaultValue = "true" }
            }).ToList(),
            
            EntityType.Lookup => baseProperties.Concat(new[]
            {
                new BlueprintProperty { Name = "Code", Type = PropertyType.String, MaxLength = 10, IsUnique = true },
                new BlueprintProperty { Name = "SortOrder", Type = PropertyType.Integer, DefaultValue = "0" }
            }).ToList(),
            
            EntityType.Complex => baseProperties.Concat(new[]
            {
                new BlueprintProperty { Name = "Status", Type = PropertyType.String, MaxLength = 50 },
                new BlueprintProperty { Name = "Data", Type = PropertyType.Json },
                new BlueprintProperty { Name = "UpdatedAt", Type = PropertyType.DateTime }
            }).ToList(),
            
            _ => baseProperties
        };
    }

    private decimal GetBaseCostForEntityType(EntityType type) => type switch
    {
        EntityType.Core => 500,
        EntityType.Lookup => 200,
        EntityType.Junction => 300,
        EntityType.Audit => 400,
        EntityType.Complex => 800,
        _ => 500
    };

    private int GetBaseHoursForEntityType(EntityType type) => type switch
    {
        EntityType.Core => 8,
        EntityType.Lookup => 4,
        EntityType.Junction => 6,
        EntityType.Audit => 6,
        EntityType.Complex => 16,
        _ => 8
    };

    private string GenerateRandomString()
    {
        var words = new[] { "Sample", "Test", "Demo", "Example", "Mock", "Fake", "Generated" };
        return words[_random.Next(words.Length)] + " " + _random.Next(100, 999);
    }

    private string GenerateRandomEmail()
    {
        var firstName = _sampleFirstNames[_random.Next(_sampleFirstNames.Count)];
        var lastName = _sampleLastNames[_random.Next(_sampleLastNames.Count)];
        return $"{firstName.ToLower()}.{lastName.ToLower()}@example.com";
    }

    private string GenerateRandomPhone()
    {
        return $"+420 {_random.Next(600, 799)} {_random.Next(100, 999)} {_random.Next(100, 999)}";
    }

    private string GetRandomCompany()
    {
        return _sampleCompanies[_random.Next(_sampleCompanies.Count)];
    }

    private string GetRandomColor()
    {
        var colors = new[] { "#007bff", "#28a745", "#dc3545", "#ffc107", "#6f42c1", "#20c997" };
        return colors[_random.Next(colors.Length)];
    }
}