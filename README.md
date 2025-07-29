# 🎨 OptimalyBlueprint

**Rychlý prototypovací template** pro vizuální modelování a cenové nabídky .NET aplikací.

## 🎯 Co je OptimalyBlueprint?

OptimalyBlueprint je **jednovrstvý MVC template** navržený pro:

- ✅ **Rychlé prototypování** - Od nápadu k vizuálnímu modelu za minuty
- ✅ **Blueprint Pattern** - Fake služby pro okamžité mockování
- ✅ **Entity-Driven Pricing** - Automatické cenové nabídky podle složitosti
- ✅ **Client Presentation** - Profesionální UI pro prezentace klientům
- ✅ **One-Command Rename** - Transformace na klientský projekt
- ✅ **Visual Mockups** - Rychlé vytváření UI komponent

Ideální pro **konzultanty**, **vývojářské agentury** a **prototypování** před vývojem skutečné aplikace.

## 🚀 Quick Start

### 🔥 Vytvoření klientského prototypu (30 sekund)

```bash
# Klonování a přejmenování
git clone https://github.com/yourusername/OptimalyBlueprint.git ClientProjectName
cd ClientProjectName

# Automatické přejmenování (brzy)
./rename-blueprint.sh "ClientProjectName"

# Spuštění
dotnet run
```

**🎉 Váš klientský prototyp běží na http://localhost:5000!**

## 🎨 Blueprint Pattern Features

### Fake Data Services
- **MockEntityService** - Generování fake entit s realistickými daty
- **PricingCalculator** - Automatické cenotvorba podle složitosti
- **VisualMockupGenerator** - UI komponenty s placeholder daty

### Rychlé Modelování
- **Entity Builder** - Drag & drop tvorba datových modelů
- **UI Mockup Generator** - Okamžité vytváření CRUD rozhraní
- **Pricing Estimator** - Real-time kalkulace nákladů

### Client Presentation
- **Professional Dashboard** - Clean UI pro prezentace
- **Interactive Mockups** - Klikatelné prototypy
- **Cost Breakdown** - Detailní rozpis cenové nabídky

## 📊 Pricing Model

Automatické generování cenových nabídek podle:

- **Počet entit** (€500 za entitu)
- **Složitost relací** (€200 za foreign key)
- **UI složitost** (€300 za CRUD sadu)
- **Integrace** (€800 za externí API)
- **Bezpečnost** (€400 za auth systém)

## 🛠️ Struktur Projektu

```
OptimalyBlueprint/
├── 🎯 Controllers/
│   ├── HomeController.cs          # Dashboard a přehled
│   ├── BlueprintController.cs     # Entity modelování  
│   ├── MockupController.cs        # UI mockupy
│   └── PricingController.cs       # Cenové kalkulace
├── 🎨 Models/
│   ├── BlueprintEntity.cs         # Model pro entity design
│   ├── MockupComponent.cs         # UI komponenty
│   └── PricingModel.cs            # Cenové modely
├── 🖼️ Views/
│   ├── Blueprint/                 # Entity designer
│   ├── Mockup/                    # UI mockup generátor
│   └── Pricing/                   # Cenové nabídky
└── 🔧 Services/
    ├── MockDataService.cs         # Fake data generátor
    ├── PricingService.cs          # Cenové kalkulace
    └── ExportService.cs           # Export do PDF/Word
```

## 🎯 Use Cases

### Pro Konzultanty
- Rychlé vytvoření vizuálního návrhu během meetingu s klientem
- Okamžité generování cenové nabídky
- Profesionální prezentace konceptu

### Pro Agentury
- Prototypování před začátkem vývoje
- A/B testování UI konceptů s klientem
- Validace requirements před implementací

### Pro Freelancery
- Rychlé mockupy pro získání zakázek
- Transparentní cenotvorba pro klienty
- Template pro opakované typy projektů

## ✅ Implementované Funkce

- [x] **Blueprint Pattern** - Fake služby pro rychlé prototypování
- [x] **Entity Designer** - CRUD pro vytváření entit s vlastnostmi a relacemi
- [x] **Pricing Calculator** - Automatické generování cenových nabídek
- [x] **Mock Data Generator** - Realistická fake data pro vizualizace
- [x] **Project Templates** - Předpřipravené šablony (E-commerce, CRM, Blog, Events)
- [x] **Session Management** - Persistentní data během práce
- [x] **Export Reports** - Markdown export cenových nabídek
- [x] **Rename Script** - Automatické přejmenování na klientský projekt
- [x] **Template Comparison** - Porovnání různých projektových šablon

## 🚀 Spuštění Projektu

### 🔥 VS Code F5 Debug (Doporučeno)
```bash
# Klonování a otevření ve VS Code
git clone https://github.com/yourusername/OptimalyBlueprint.git
cd OptimalyBlueprint
code OptimalyBlueprint.code-workspace

# Stiskněte F5 - automaticky se spustí build, server a otevře prohlížeč!
```

### 📋 Manuální spuštění
```bash
# Build a spuštění
dotnet build OptimalyBlueprint
dotnet run --project OptimalyBlueprint

# Otevřete http://localhost:5000
```

### 🐛 Debugging Features
- **F5** - Spustí debug režim s automatickým otevřením prohlížeče
- **Ctrl+F5** - Spustí bez debuggingu
- **Auto-kill** - Automaticky ukončí běžící procesy před startem
- **Hot reload** - Změny v kódu se automaticky aplikují

## 📋 Roadmap

- [ ] **Základní Views** - HTML rozhraní pro všechny controllery
- [ ] **Bootstrap/AdminLTE UI** - Profesionální vzhled
- [ ] **Interactive Charts** - Grafy pro dashboard a pricing
- [ ] **PDF Export** - Profesionální dokumentace s logy
- [ ] **Real-time Updates** - AJAX pro dynamické pricing
- [ ] **Project Workspace** - Ukládání projektů na disk
- [ ] **Template Gallery** - Více industry templates
- [ ] **Client Portal** - Sdílené workspace s klientem

---

**Ready to prototype fast?** 🚀 Start building visual concepts that sell!