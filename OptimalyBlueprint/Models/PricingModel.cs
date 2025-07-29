namespace OptimalyBlueprint.Models;

public class PricingModel
{
    public string ProjectName { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public List<BlueprintEntity> Entities { get; set; } = new();
    public PricingBreakdown Breakdown { get; set; } = new();
    public decimal TotalCost => Breakdown.CalculateTotal();
    public int TotalHours => Breakdown.CalculateTotalHours();
    public string Currency { get; set; } = "EUR";
}

public class PricingBreakdown
{
    public decimal EntitiesCost { get; set; }
    public decimal RelationsCost { get; set; }
    public decimal UICost { get; set; }
    public decimal SecurityCost { get; set; }
    public decimal IntegrationsCost { get; set; }
    public decimal TestingCost { get; set; }
    public decimal DeploymentCost { get; set; }
    
    public int EntitiesHours { get; set; }
    public int RelationsHours { get; set; }
    public int UIHours { get; set; }
    public int SecurityHours { get; set; }
    public int IntegrationsHours { get; set; }
    public int TestingHours { get; set; }
    public int DeploymentHours { get; set; }
    
    public List<CostItem> DetailedItems { get; set; } = new();
    
    public decimal CalculateTotal()
    {
        return EntitiesCost + RelationsCost + UICost + SecurityCost + 
               IntegrationsCost + TestingCost + DeploymentCost;
    }
    
    public int CalculateTotalHours()
    {
        return EntitiesHours + RelationsHours + UIHours + SecurityHours + 
               IntegrationsHours + TestingHours + DeploymentHours;
    }
}

public class CostItem
{
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice => Quantity * UnitPrice;
    public int Hours { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class ProjectTemplate
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;
    public List<BlueprintEntity> PrebuiltEntities { get; set; } = new();
    public decimal BasePrice { get; set; }
    public string IconClass { get; set; } = "fas fa-project-diagram";
}