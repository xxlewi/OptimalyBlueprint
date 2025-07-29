using OptimalyBlueprint.Models;

namespace OptimalyBlueprint.Services;

public interface IPricingService
{
    PricingModel CalculatePricing(List<BlueprintEntity> entities, string projectName, string clientName);
    PricingBreakdown CalculateDetailedBreakdown(List<BlueprintEntity> entities);
    decimal EstimateEntityCost(BlueprintEntity entity);
    string GeneratePricingReport(PricingModel pricing);
}

public class PricingService : IPricingService
{
    private const decimal HOURLY_RATE = 50m; // €50 per hour
    
    // Base costs per entity type
    private readonly Dictionary<EntityType, (decimal cost, int hours)> _entityBaseCosts = new()
    {
        { EntityType.Core, (500m, 8) },
        { EntityType.Lookup, (200m, 4) },
        { EntityType.Junction, (300m, 6) },
        { EntityType.Audit, (400m, 6) },
        { EntityType.Complex, (800m, 16) }
    };
    
    // Additional costs per relation type
    private readonly Dictionary<RelationType, (decimal cost, int hours)> _relationCosts = new()
    {
        { RelationType.OneToOne, (200m, 3) },
        { RelationType.OneToMany, (300m, 4) },
        { RelationType.ManyToMany, (500m, 8) }
    };

    public PricingModel CalculatePricing(List<BlueprintEntity> entities, string projectName, string clientName)
    {
        var breakdown = CalculateDetailedBreakdown(entities);
        
        return new PricingModel
        {
            ProjectName = projectName,
            ClientName = clientName,
            Entities = entities,
            Breakdown = breakdown,
            CreatedAt = DateTime.Now
        };
    }

    public PricingBreakdown CalculateDetailedBreakdown(List<BlueprintEntity> entities)
    {
        var breakdown = new PricingBreakdown();
        
        // Calculate entities cost
        foreach (var entity in entities)
        {
            var (baseCost, baseHours) = _entityBaseCosts[entity.Type];
            var propertyCost = CalculatePropertiesCost(entity.Properties);
            var relationCost = CalculateRelationsCost(entity.Relations);
            
            breakdown.EntitiesCost += baseCost + propertyCost;
            breakdown.EntitiesHours += baseHours + CalculatePropertiesHours(entity.Properties);
            
            breakdown.RelationsCost += relationCost;
            breakdown.RelationsHours += CalculateRelationsHours(entity.Relations);
            
            // Add detailed items
            breakdown.DetailedItems.Add(new CostItem
            {
                Category = "Entity",
                Description = $"{entity.Name} ({entity.Type})",
                Quantity = 1,
                UnitPrice = baseCost + propertyCost,
                Hours = baseHours + CalculatePropertiesHours(entity.Properties),
                Notes = $"{entity.Properties.Count} properties"
            });
        }
        
        // Calculate UI costs
        breakdown.UICost = CalculateUICost(entities);
        breakdown.UIHours = CalculateUIHours(entities);
        
        // Calculate additional features
        breakdown.SecurityCost = CalculateSecurityCost(entities);
        breakdown.SecurityHours = CalculateSecurityHours(entities);
        
        breakdown.TestingCost = CalculateTestingCost(breakdown.CalculateTotal() * 0.2m); // 20% of development
        breakdown.TestingHours = (int)(breakdown.CalculateTotalHours() * 0.3); // 30% more time for testing
        
        breakdown.DeploymentCost = 800m; // Fixed deployment cost
        breakdown.DeploymentHours = 16; // Fixed deployment hours
        
        return breakdown;
    }

    public decimal EstimateEntityCost(BlueprintEntity entity)
    {
        var (baseCost, _) = _entityBaseCosts[entity.Type];
        var propertyCost = CalculatePropertiesCost(entity.Properties);
        var relationCost = CalculateRelationsCost(entity.Relations);
        
        return baseCost + propertyCost + relationCost;
    }

    public string GeneratePricingReport(PricingModel pricing)
    {
        var report = $@"
# Cenová nabídka - {pricing.ProjectName}

**Klient:** {pricing.ClientName}  
**Datum:** {pricing.CreatedAt:dd.MM.yyyy}  
**Celková cena:** {pricing.TotalCost:C} EUR  
**Odhadované hodiny:** {pricing.TotalHours}h  

## Rozpis nákladů

### Datové entity ({pricing.Entities.Count}x)
- **Náklady:** {pricing.Breakdown.EntitiesCost:C} EUR
- **Hodiny:** {pricing.Breakdown.EntitiesHours}h

### Relace mezi entitami
- **Náklady:** {pricing.Breakdown.RelationsCost:C} EUR  
- **Hodiny:** {pricing.Breakdown.RelationsHours}h

### Uživatelské rozhraní
- **Náklady:** {pricing.Breakdown.UICost:C} EUR
- **Hodiny:** {pricing.Breakdown.UIHours}h

### Bezpečnost a autentizace
- **Náklady:** {pricing.Breakdown.SecurityCost:C} EUR
- **Hodiny:** {pricing.Breakdown.SecurityHours}h

### Testování
- **Náklady:** {pricing.Breakdown.TestingCost:C} EUR
- **Hodiny:** {pricing.Breakdown.TestingHours}h

### Nasazení a deployment
- **Náklady:** {pricing.Breakdown.DeploymentCost:C} EUR
- **Hodiny:** {pricing.Breakdown.DeploymentHours}h

## Detailní rozpis entit

";

        foreach (var entity in pricing.Entities)
        {
            report += $@"
### {entity.Name} ({entity.Type})
- **Popis:** {entity.Description}
- **Vlastnosti:** {entity.Properties.Count}x
- **Relace:** {entity.Relations.Count}x
- **Náklady:** {EstimateEntityCost(entity):C} EUR

";
        }

        report += $@"

## Podmínky

- Ceny jsou uvedeny v EUR bez DPH
- Odhad času je orientační a může se lišit podle složitosti požadavků
- Zahrnuje základní funkcionalität bez specifických integrací
- Finální cena může být upravena po detailní analýze požadavků

**Kontakt:** info@optimalyblueprint.com  
**Platnost nabídky:** {pricing.CreatedAt.AddDays(30):dd.MM.yyyy}
";

        return report;
    }

    private decimal CalculatePropertiesCost(List<BlueprintProperty> properties)
    {
        decimal cost = 0;
        
        foreach (var property in properties)
        {
            cost += property.Type switch
            {
                PropertyType.String => 20m,
                PropertyType.Integer => 15m,
                PropertyType.Decimal => 15m,
                PropertyType.DateTime => 25m,
                PropertyType.Boolean => 10m,
                PropertyType.Email => 30m,
                PropertyType.Phone => 30m,
                PropertyType.Url => 25m,
                PropertyType.Json => 50m,
                PropertyType.File => 80m,
                _ => 20m
            };
            
            // Additional cost for complex properties
            if (property.IsUnique) cost += 20m;
            if (property.IsRequired) cost += 10m;
        }
        
        return cost;
    }
    
    private int CalculatePropertiesHours(List<BlueprintProperty> properties)
    {
        int hours = 0;
        
        foreach (var property in properties)
        {
            hours += property.Type switch
            {
                PropertyType.Json => 2,
                PropertyType.File => 3,
                PropertyType.Email => 1,
                PropertyType.Phone => 1,
                _ => 0 // Basic properties included in base cost
            };
        }
        
        return hours;
    }

    private decimal CalculateRelationsCost(List<BlueprintRelation> relations)
    {
        return relations.Sum(r => _relationCosts[r.Type].cost);
    }
    
    private int CalculateRelationsHours(List<BlueprintRelation> relations)
    {
        return relations.Sum(r => _relationCosts[r.Type].hours);
    }

    private decimal CalculateUICost(List<BlueprintEntity> entities)
    {
        // Base UI cost per entity: List (200) + Create (300) + Edit (300) + Detail (150)
        const decimal baseUICostPerEntity = 950m;
        return entities.Count * baseUICostPerEntity;
    }
    
    private int CalculateUIHours(List<BlueprintEntity> entities)
    {
        // Base UI hours per entity: List (4h) + Create (6h) + Edit (6h) + Detail (3h)
        const int baseUIHoursPerEntity = 19;
        return entities.Count * baseUIHoursPerEntity;
    }

    private decimal CalculateSecurityCost(List<BlueprintEntity> entities)
    {
        // Base security implementation
        decimal baseCost = 400m;
        
        // Additional cost if there are user-related entities
        bool hasUserEntities = entities.Any(e => 
            e.Name.ToLower().Contains("user") || 
            e.Name.ToLower().Contains("account") ||
            e.Properties.Any(p => p.Type == PropertyType.Email));
            
        if (hasUserEntities)
        {
            baseCost += 600m; // Advanced authentication
        }
        
        return baseCost;
    }
    
    private int CalculateSecurityHours(List<BlueprintEntity> entities)
    {
        int baseHours = 8;
        
        bool hasUserEntities = entities.Any(e => 
            e.Name.ToLower().Contains("user") || 
            e.Name.ToLower().Contains("account"));
            
        if (hasUserEntities)
        {
            baseHours += 12; // Advanced authentication
        }
        
        return baseHours;
    }

    private decimal CalculateTestingCost(decimal developmentCost)
    {
        return Math.Max(500m, developmentCost * 0.15m); // Minimum 500 EUR, otherwise 15% of development
    }
}