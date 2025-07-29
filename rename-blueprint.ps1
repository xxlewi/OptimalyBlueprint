# OptimalyBlueprint Project Rename Script for Windows
# Usage: .\rename-blueprint.ps1 "NewProjectName"

param(
    [Parameter(Mandatory=$true)]
    [string]$NewProjectName
)

$ErrorActionPreference = "Stop"

$OldProjectName = "OptimalyBlueprint"

# Validate project name
if ($NewProjectName -notmatch "^[A-Za-z][A-Za-z0-9]*$") {
    Write-Host "❌ Error: Project name must start with a letter and contain only letters and numbers" -ForegroundColor Red
    Write-Host "Invalid: $NewProjectName" -ForegroundColor Red
    exit 1
}

Write-Host "🎨 OptimalyBlueprint Project Rename Tool" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Old name: $OldProjectName" -ForegroundColor Yellow
Write-Host "New name: $NewProjectName" -ForegroundColor Green
Write-Host ""

$confirmation = Read-Host "Continue with rename? (y/N)"
if ($confirmation -ne 'y' -and $confirmation -ne 'Y') {
    Write-Host "❌ Rename cancelled" -ForegroundColor Red
    exit 1
}

Write-Host "🔄 Starting rename process..." -ForegroundColor Cyan

# Step 1: Rename the main project folder
Write-Host "📁 Renaming project folder..." -ForegroundColor Blue
if (Test-Path $OldProjectName) {
    Rename-Item $OldProjectName $NewProjectName
    Write-Host "✅ Renamed folder: $OldProjectName → $NewProjectName" -ForegroundColor Green
}

# Step 2: Rename .csproj file
Write-Host "📄 Renaming project file..." -ForegroundColor Blue
$oldCsprojPath = "$NewProjectName\$OldProjectName.csproj"
$newCsprojPath = "$NewProjectName\$NewProjectName.csproj"
if (Test-Path $oldCsprojPath) {
    Rename-Item $oldCsprojPath $newCsprojPath
    Write-Host "✅ Renamed project file: $OldProjectName.csproj → $NewProjectName.csproj" -ForegroundColor Green
}

# Step 3: Update namespace in all .cs files
Write-Host "🔧 Updating namespaces in C# files..." -ForegroundColor Blue
Get-ChildItem -Path $NewProjectName -Filter "*.cs" -Recurse | ForEach-Object {
    (Get-Content $_.FullName) -replace "namespace $OldProjectName", "namespace $NewProjectName" -replace "using $OldProjectName", "using $NewProjectName" | Set-Content $_.FullName
}
Write-Host "✅ Updated namespaces in C# files" -ForegroundColor Green

# Step 4: Update project references in .csproj
Write-Host "🔗 Updating project references..." -ForegroundColor Blue
if (Test-Path $newCsprojPath) {
    (Get-Content $newCsprojPath) -replace $OldProjectName, $NewProjectName | Set-Content $newCsprojPath
    Write-Host "✅ Updated project references" -ForegroundColor Green
}

# Step 5: Update Views and Razor files
Write-Host "🎨 Updating Views and Razor files..." -ForegroundColor Blue
Get-ChildItem -Path $NewProjectName -Filter "*.cshtml" -Recurse | ForEach-Object {
    (Get-Content $_.FullName) -replace $OldProjectName, $NewProjectName | Set-Content $_.FullName
}
Write-Host "✅ Updated Razor views" -ForegroundColor Green

# Step 6: Update appsettings.json
Write-Host "⚙️ Updating configuration files..." -ForegroundColor Blue
$appsettingsPath = "$NewProjectName\appsettings.json"
if (Test-Path $appsettingsPath) {
    (Get-Content $appsettingsPath) -replace $OldProjectName, $NewProjectName | Set-Content $appsettingsPath
}

$appsettingsDevPath = "$NewProjectName\appsettings.Development.json"
if (Test-Path $appsettingsDevPath) {
    (Get-Content $appsettingsDevPath) -replace $OldProjectName, $NewProjectName | Set-Content $appsettingsDevPath
}
Write-Host "✅ Updated configuration files" -ForegroundColor Green

# Step 7: Update launchSettings.json
Write-Host "🚀 Updating launch settings..." -ForegroundColor Blue
$launchSettingsPath = "$NewProjectName\Properties\launchSettings.json"
if (Test-Path $launchSettingsPath) {
    (Get-Content $launchSettingsPath) -replace $OldProjectName, $NewProjectName | Set-Content $launchSettingsPath
    Write-Host "✅ Updated launch settings" -ForegroundColor Green
}

# Step 8: Update README files
Write-Host "📚 Updating documentation..." -ForegroundColor Blue
if (Test-Path "README.md") {
    (Get-Content "README.md") -replace $OldProjectName, $NewProjectName -replace "OptimalyBlueprint", $NewProjectName | Set-Content "README.md"
    Write-Host "✅ Updated README.md" -ForegroundColor Green
}

# Step 9: Create new README for the renamed project
$newReadmeContent = @"
# 🎨 $NewProjectName

**Prototypovací projekt** vytvořený z OptimalyBlueprint template.

## 🚀 Quick Start

``````bash
dotnet run --project $NewProjectName
``````

**🎉 Aplikace běží na http://localhost:5000!**

## 📋 Funkcionalita

- **Blueprint Designer** - Rychlé modelování entit
- **Pricing Calculator** - Automatické cenové nabídky  
- **UI Mockups** - Vizuální prototypy
- **Export Reports** - PDF/Markdown dokumentace

## 🔧 Development

``````bash
# Spuštění aplikace
dotnet run --project $NewProjectName

# Build
dotnet build

# Publish
dotnet publish -c Release
``````

---

*Vytvořeno pomocí [OptimalyBlueprint](https://github.com/yourusername/OptimalyBlueprint) template*
"@

Set-Content -Path "README.md" -Value $newReadmeContent

# Step 10: Update VS Code configuration
Write-Host "🔧 Updating VS Code configuration..." -ForegroundColor Blue
if (Test-Path ".vscode\launch.json") {
    (Get-Content ".vscode\launch.json") -replace $OldProjectName, $NewProjectName | Set-Content ".vscode\launch.json"
}

if (Test-Path ".vscode\tasks.json") {
    (Get-Content ".vscode\tasks.json") -replace $OldProjectName, $NewProjectName | Set-Content ".vscode\tasks.json"
}

if (Test-Path "$OldProjectName.code-workspace") {
    (Get-Content "$OldProjectName.code-workspace") -replace $OldProjectName, $NewProjectName | Set-Content "$NewProjectName.code-workspace"
    Remove-Item "$OldProjectName.code-workspace"
}

if (Test-Path "kill-dotnet.sh") {
    (Get-Content "kill-dotnet.sh") -replace $OldProjectName, $NewProjectName | Set-Content "kill-dotnet.sh"
}

Write-Host "✅ Updated VS Code configuration" -ForegroundColor Green

# Step 11: Update this rename script for future use
Write-Host "🔄 Updating rename script..." -ForegroundColor Blue
(Get-Content "rename-blueprint.ps1") -replace "`$OldProjectName = `"$OldProjectName`"", "`$OldProjectName = `"$NewProjectName`"" | Set-Content "rename-blueprint.ps1"

Write-Host ""
Write-Host "🎉 Rename completed successfully!" -ForegroundColor Green
Write-Host "===============================" -ForegroundColor Green
Write-Host "✅ Project renamed from '$OldProjectName' to '$NewProjectName'" -ForegroundColor Green
Write-Host "✅ All files and references updated" -ForegroundColor Green
Write-Host "✅ Ready for development" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Next steps:" -ForegroundColor Cyan
Write-Host "1. cd $NewProjectName" -ForegroundColor White
Write-Host "2. dotnet run" -ForegroundColor White
Write-Host "3. Open http://localhost:5000" -ForegroundColor White
Write-Host ""
Write-Host "🚀 Happy prototyping with $NewProjectName!" -ForegroundColor Magenta