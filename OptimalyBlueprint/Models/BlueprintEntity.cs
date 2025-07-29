namespace OptimalyBlueprint.Models;

public class BlueprintEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public EntityType Type { get; set; }
    public List<BlueprintProperty> Properties { get; set; } = new();
    public List<BlueprintRelation> Relations { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public decimal EstimatedCost { get; set; }
    public int DevelopmentHours { get; set; }
}

public class BlueprintProperty
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public PropertyType Type { get; set; }
    public bool IsRequired { get; set; }
    public bool IsUnique { get; set; }
    public int? MaxLength { get; set; }
    public string? DefaultValue { get; set; }
}

public class BlueprintRelation
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int RelatedEntityId { get; set; }
    public string RelatedEntityName { get; set; } = string.Empty;
    public RelationType Type { get; set; }
    public decimal AdditionalCost { get; set; }
}

public enum EntityType
{
    Core = 1,           // €500 - základní entita
    Lookup = 2,         // €200 - číselník
    Junction = 3,       // €300 - vazební tabulka
    Audit = 4,          // €400 - auditní entita
    Complex = 5         // €800 - složitá business entita
}

public enum PropertyType
{
    String = 1,
    Integer = 2,
    Decimal = 3,
    DateTime = 4,
    Boolean = 5,
    Email = 6,
    Phone = 7,
    Url = 8,
    Json = 9,
    File = 10
}

public enum RelationType
{
    OneToOne = 1,       // €200
    OneToMany = 2,      // €300
    ManyToMany = 3      // €500
}