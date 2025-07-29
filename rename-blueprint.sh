#!/bin/bash

# OptimalyBlueprint Project Rename Script
# Usage: ./rename-blueprint.sh "NewProjectName"

set -e

if [ -z "$1" ]; then
    echo "❌ Error: Project name is required"
    echo "Usage: ./rename-blueprint.sh \"NewProjectName\""
    echo "Example: ./rename-blueprint.sh \"ClientEcommerce\""
    exit 1
fi

NEW_PROJECT_NAME="$1"
OLD_PROJECT_NAME="OptimalyBlueprint"

# Validate project name
if [[ ! "$NEW_PROJECT_NAME" =~ ^[A-Za-z][A-Za-z0-9]*$ ]]; then
    echo "❌ Error: Project name must start with a letter and contain only letters and numbers"
    echo "Invalid: $NEW_PROJECT_NAME"
    exit 1
fi

echo "🎨 OptimalyBlueprint Project Rename Tool"
echo "========================================"
echo "Old name: $OLD_PROJECT_NAME"
echo "New name: $NEW_PROJECT_NAME"
echo ""

read -p "Continue with rename? (y/N): " -n 1 -r
echo
if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    echo "❌ Rename cancelled"
    exit 1
fi

echo "🔄 Starting rename process..."

# Step 1: Rename the main project folder
echo "📁 Renaming project folder..."
if [ -d "$OLD_PROJECT_NAME" ]; then
    mv "$OLD_PROJECT_NAME" "$NEW_PROJECT_NAME"
    echo "✅ Renamed folder: $OLD_PROJECT_NAME → $NEW_PROJECT_NAME"
fi

# Step 2: Rename .csproj file
echo "📄 Renaming project file..."
if [ -f "$NEW_PROJECT_NAME/$OLD_PROJECT_NAME.csproj" ]; then
    mv "$NEW_PROJECT_NAME/$OLD_PROJECT_NAME.csproj" "$NEW_PROJECT_NAME/$NEW_PROJECT_NAME.csproj"
    echo "✅ Renamed project file: $OLD_PROJECT_NAME.csproj → $NEW_PROJECT_NAME.csproj"
fi

# Step 3: Update namespace in all .cs files
echo "🔧 Updating namespaces in C# files..."
find "$NEW_PROJECT_NAME" -name "*.cs" -type f -exec sed -i.bak "s/namespace $OLD_PROJECT_NAME/namespace $NEW_PROJECT_NAME/g" {} \;
find "$NEW_PROJECT_NAME" -name "*.cs" -type f -exec sed -i.bak "s/using $OLD_PROJECT_NAME/using $NEW_PROJECT_NAME/g" {} \;

# Clean up backup files
find "$NEW_PROJECT_NAME" -name "*.bak" -delete

echo "✅ Updated namespaces in C# files"

# Step 4: Update project references in .csproj
echo "🔗 Updating project references..."
if [ -f "$NEW_PROJECT_NAME/$NEW_PROJECT_NAME.csproj" ]; then
    sed -i.bak "s/$OLD_PROJECT_NAME/$NEW_PROJECT_NAME/g" "$NEW_PROJECT_NAME/$NEW_PROJECT_NAME.csproj"
    rm -f "$NEW_PROJECT_NAME/$NEW_PROJECT_NAME.csproj.bak"
    echo "✅ Updated project references"
fi

# Step 5: Update Views and Razor files
echo "🎨 Updating Views and Razor files..."
find "$NEW_PROJECT_NAME" -name "*.cshtml" -type f -exec sed -i.bak "s/$OLD_PROJECT_NAME/$NEW_PROJECT_NAME/g" {} \;
find "$NEW_PROJECT_NAME" -name "*.bak" -delete
echo "✅ Updated Razor views"

# Step 6: Update appsettings.json
echo "⚙️ Updating configuration files..."
if [ -f "$NEW_PROJECT_NAME/appsettings.json" ]; then
    sed -i.bak "s/$OLD_PROJECT_NAME/$NEW_PROJECT_NAME/g" "$NEW_PROJECT_NAME/appsettings.json"
    rm -f "$NEW_PROJECT_NAME/appsettings.json.bak"
fi

if [ -f "$NEW_PROJECT_NAME/appsettings.Development.json" ]; then
    sed -i.bak "s/$OLD_PROJECT_NAME/$NEW_PROJECT_NAME/g" "$NEW_PROJECT_NAME/appsettings.Development.json"
    rm -f "$NEW_PROJECT_NAME/appsettings.Development.json.bak"
fi
echo "✅ Updated configuration files"

# Step 7: Update launchSettings.json
echo "🚀 Updating launch settings..."
if [ -f "$NEW_PROJECT_NAME/Properties/launchSettings.json" ]; then
    sed -i.bak "s/$OLD_PROJECT_NAME/$NEW_PROJECT_NAME/g" "$NEW_PROJECT_NAME/Properties/launchSettings.json"
    rm -f "$NEW_PROJECT_NAME/Properties/launchSettings.json.bak"
    echo "✅ Updated launch settings"
fi

# Step 8: Update README files
echo "📚 Updating documentation..."
if [ -f "README.md" ]; then
    sed -i.bak "s/$OLD_PROJECT_NAME/$NEW_PROJECT_NAME/g" "README.md"
    sed -i.bak "s/OptimalyBlueprint/$NEW_PROJECT_NAME/g" "README.md"
    rm -f "README.md.bak"
    echo "✅ Updated README.md"
fi

# Step 9: Create new README for the renamed project
cat > "README.md" << EOF
# 🎨 $NEW_PROJECT_NAME

**Prototypovací projekt** vytvořený z OptimalyBlueprint template.

## 🚀 Quick Start

\`\`\`bash
dotnet run --project $NEW_PROJECT_NAME
\`\`\`

**🎉 Aplikace běží na http://localhost:5000!**

## 📋 Funkcionalita

- **Blueprint Designer** - Rychlé modelování entit
- **Pricing Calculator** - Automatické cenové nabídky  
- **UI Mockups** - Vizuální prototypy
- **Export Reports** - PDF/Markdown dokumentace

## 🔧 Development

\`\`\`bash
# Spuštění aplikace
dotnet run --project $NEW_PROJECT_NAME

# Build
dotnet build

# Publish
dotnet publish -c Release
\`\`\`

---

*Vytvořeno pomocí [OptimalyBlueprint](https://github.com/yourusername/OptimalyBlueprint) template*
EOF

# Step 10: Update VS Code configuration
echo "🔧 Updating VS Code configuration..."
if [ -f ".vscode/launch.json" ]; then
    sed -i.bak "s/$OLD_PROJECT_NAME/$NEW_PROJECT_NAME/g" ".vscode/launch.json"
    rm -f ".vscode/launch.json.bak"
fi

if [ -f ".vscode/tasks.json" ]; then
    sed -i.bak "s/$OLD_PROJECT_NAME/$NEW_PROJECT_NAME/g" ".vscode/tasks.json"
    rm -f ".vscode/tasks.json.bak"
fi

if [ -f "$OLD_PROJECT_NAME.code-workspace" ]; then
    sed -i.bak "s/$OLD_PROJECT_NAME/$NEW_PROJECT_NAME/g" "$OLD_PROJECT_NAME.code-workspace"
    mv "$OLD_PROJECT_NAME.code-workspace" "$NEW_PROJECT_NAME.code-workspace"
    rm -f "$NEW_PROJECT_NAME.code-workspace.bak"
fi

if [ -f "kill-dotnet.sh" ]; then
    sed -i.bak "s/$OLD_PROJECT_NAME/$NEW_PROJECT_NAME/g" "kill-dotnet.sh"
    rm -f "kill-dotnet.sh.bak"
fi

echo "✅ Updated VS Code configuration"

# Step 11: Update this rename script for future use
echo "🔄 Updating rename script..."
sed -i.bak "s/OLD_PROJECT_NAME=\"$OLD_PROJECT_NAME\"/OLD_PROJECT_NAME=\"$NEW_PROJECT_NAME\"/" "rename-blueprint.sh"
rm -f "rename-blueprint.sh.bak"

echo ""
echo "🎉 Rename completed successfully!"
echo "==============================="
echo "✅ Project renamed from '$OLD_PROJECT_NAME' to '$NEW_PROJECT_NAME'"
echo "✅ All files and references updated"
echo "✅ Ready for development"
echo ""
echo "📋 Next steps:"
echo "1. cd $NEW_PROJECT_NAME"
echo "2. dotnet run"
echo "3. Open http://localhost:5000"
echo ""
echo "🚀 Happy prototyping with $NEW_PROJECT_NAME!"