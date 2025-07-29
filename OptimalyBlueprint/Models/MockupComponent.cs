namespace OptimalyBlueprint.Models;

public class MockupComponent
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ComponentType Type { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public List<MockField> Fields { get; set; } = new();
    public string PreviewHtml { get; set; } = string.Empty;
    public decimal DevelopmentCost { get; set; }
    public int DevelopmentHours { get; set; }
}

public class MockField
{
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public FieldType Type { get; set; }
    public bool IsRequired { get; set; }
    public string PlaceholderText { get; set; } = string.Empty;
    public List<string> Options { get; set; } = new(); // pro select, radio
    public string MockValue { get; set; } = string.Empty;
}

public class MockupProject
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<MockupPage> Pages { get; set; } = new();
    public string ClientName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public MockupTheme Theme { get; set; } = new();
}

public class MockupPage
{
    public string Name { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public PageType Type { get; set; }
    public List<MockupComponent> Components { get; set; } = new();
    public string Description { get; set; } = string.Empty;
}

public class MockupTheme
{
    public string PrimaryColor { get; set; } = "#007bff";
    public string SecondaryColor { get; set; } = "#6c757d";
    public string FontFamily { get; set; } = "Arial, sans-serif";
    public string LogoUrl { get; set; } = "/images/logo-placeholder.png";
    public string CompanyName { get; set; } = "Client Company";
}

public enum ComponentType
{
    List = 1,           // €200 - seznam s paginací
    Form = 2,           // €300 - formulář pro vytvoření/editaci
    Detail = 3,         // €150 - detail záznamu
    Dashboard = 4,      // €500 - dashboard s grafy
    Search = 5,         // €250 - vyhledávání s filtry
    Navigation = 6,     // €100 - navigace
    Report = 7,         // €400 - reporty a export
    Chart = 8           // €300 - grafy a statistiky
}

public enum FieldType
{
    Text = 1,
    Email = 2,
    Password = 3,
    Number = 4,
    Date = 5,
    Select = 6,
    Checkbox = 7,
    Radio = 8,
    Textarea = 9,
    File = 10
}

public enum PageType
{
    Dashboard = 1,
    List = 2,
    Create = 3,
    Edit = 4,
    Detail = 5,
    Login = 6,
    Profile = 7,
    Settings = 8,
    Report = 9
}