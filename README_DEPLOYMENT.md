# 🚀 Návod na deployment aplikací do Optimaly infrastruktury

Tento návod popisuje, jak nasadit .NET aplikaci do produkčního prostředí na serveru Optimaly pomocí Docker kontejnerů a univerzálního deployment systému.

## 📋 Předpoklady

- Přístup k serveru `147.93.120.231` přes SSH
- GitHub repository s vaší aplikací
- .NET aplikace připravená k nasazení

## 🛠️ Kroky k nasazení nové aplikace

### 1. Příprava souborů v projektu

Vytvořte následující soubory v kořenovém adresáři vašeho projektu:

#### **Dockerfile**
```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file
COPY YourApp/YourApp.csproj YourApp/

# Restore dependencies
RUN dotnet restore YourApp/YourApp.csproj

# Copy everything else and build
COPY . .
WORKDIR /src/YourApp
RUN dotnet build YourApp.csproj -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish YourApp.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copy published app
COPY --from=publish /app/publish .

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "YourApp.dll"]
```

#### **app.json**
```json
{
  "subdomain": "yourapp.optimaly.net",
  "port": 8080,
  "health_endpoint": "/health",
  "environment": {
    "ASPNETCORE_ENVIRONMENT": "${ASPNETCORE_ENVIRONMENT}",
    "ASPNETCORE_URLS": "http://+:8080",
    "ASPNETCORE_FORWARDEDHEADERS_ENABLED": "true"
  },
  "volumes": [],
  "depends_on": ["caddy"],
  "networks": ["optimaly-web", "optimaly-apps"], 
  "restart_policy": "unless-stopped",
  "database_connection_var": "YOURAPP_DB_CONNECTION"
}
```

#### **.github/workflows/docker-build.yml**
```yaml
name: Build and Push Docker Image

on:
  push:
    branches: [ main ]  # nebo vaše hlavní větev
  pull_request:
    branches: [ main ]

env:
  REGISTRY: ghcr.io
  IMAGE_NAME: ${{ github.repository }}

jobs:
  build-and-push:
    runs-on: ubuntu-latest
    permissions:
      contents: read
      packages: write

    steps:
    - name: Checkout repository
      uses: actions/checkout@v4

    - name: Log in to the Container registry
      uses: docker/login-action@v3
      with:
        registry: ${{ env.REGISTRY }}
        username: ${{ github.actor }}
        password: ${{ secrets.GITHUB_TOKEN }}

    - name: Extract metadata (tags, labels) for Docker
      id: meta
      uses: docker/metadata-action@v5
      with:
        images: ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}
        tags: |
          type=ref,event=branch
          type=ref,event=pr
          type=raw,value=latest,enable={{is_default_branch}}

    - name: Build and push Docker image
      uses: docker/build-push-action@v5
      with:
        context: .
        push: true
        tags: ${{ steps.meta.outputs.tags }}
        labels: ${{ steps.meta.outputs.labels }}
```

#### **deploy.py**
```python
#!/usr/bin/env python3

"""
Deployment Script for YourApp
Builds Docker image and deploys to server using Universal Deployment System
"""

import subprocess
import sys
import os
from pathlib import Path

def run_command(command, description):
    """Run shell command with error handling"""
    print(f"🔄 {description}...")
    try:
        result = subprocess.run(command, shell=True, check=True, capture_output=True, text=True)
        if result.stdout:
            print(result.stdout.strip())
        return True
    except subprocess.CalledProcessError as e:
        print(f"❌ Error: {e}")
        if e.stderr:
            print(f"Error details: {e.stderr}")
        return False

def ensure_health_endpoint():
    """Ensure Program.cs has health endpoint configured"""
    program_cs_path = "YourApp/Program.cs"  # Upravte cestu podle struktury projektu
    
    try:
        with open(program_cs_path, 'r') as f:
            content = f.read()
        
        # Check if health endpoint is already configured
        if "AddHealthChecks()" in content and "MapHealthChecks(\"/health\")" in content:
            print("✅ Health endpoint already configured")
            return True
            
        print("🔧 Adding health endpoint to Program.cs...")
        
        # Add health checks service if not present
        if "AddHealthChecks()" not in content:
            content = content.replace(
                "builder.Services.AddControllersWithViews();",
                "builder.Services.AddControllersWithViews();\nbuilder.Services.AddHealthChecks();"
            )
        
        # Add health endpoint mapping if not present
        if "MapHealthChecks(\"/health\")" not in content:
            # Find the last MapControllerRoute and add health endpoint after it
            import re
            pattern = r'(app\.MapControllerRoute\([^)]+\);)'
            if re.search(pattern, content):
                content = re.sub(pattern, r'\1\n\napp.MapHealthChecks("/health");', content)
            else:
                # Fallback: add before app.Run()
                content = content.replace("app.Run();", "app.MapHealthChecks(\"/health\");\n\napp.Run();")
        
        with open(program_cs_path, 'w') as f:
            f.write(content)
        
        print("✅ Health endpoint added to Program.cs")
        return True
        
    except Exception as e:
        print(f"❌ Failed to update Program.cs: {e}")
        return False

def auto_commit_and_push():
    """Automatically commit and push changes if any"""
    try:
        # Check if there are any changes
        result = subprocess.run(["git", "status", "--porcelain"], capture_output=True, text=True)
        if not result.stdout.strip():
            print("✅ Žádné změny k commitnutí")
            return False  # No need to wait for build
        
        print("📝 Committing changes...")
        
        # Add all changes
        if not run_command("git add .", "Adding changes"):
            return False
        
        # Commit with automated message with timestamp
        from datetime import datetime
        timestamp = datetime.now().strftime("%Y%m%d-%H%M%S")
        commit_msg = f"{timestamp}-autodeploy yourapp\n\n🤖 Generated with Claude Code\n\nCo-Authored-By: Claude <noreply@anthropic.com>"
        if not run_command(f'git commit -m "{commit_msg}"', "Creating commit"):
            return False
        
        # Push to origin (assumes SSH key or credentials are already configured)
        if not run_command("git push", "Pushing to GitHub"):
            return False
        
        print("✅ Changes committed and pushed!")
        return True  # Changes were pushed, need to wait for build
        
    except Exception as e:
        print(f"❌ Failed to commit/push: {e}")
        return False

def wait_for_github_actions():
    """Wait for GitHub Actions to complete the build"""
    import time
    
    print("⏳ Čekání na GitHub Actions pro sestavení nového Docker image...")
    print("💡 Tím zajistíme, že do produkce se dostane nejnovější verze")
    
    # Wait 5 minutes for GitHub Actions to start and complete build
    for i in range(60):  # 60 * 5 seconds = 5 minutes
        print(f"🔄 Čekání... ({i*5}/300 sekund)")
        time.sleep(5)
        
        # After 3 minutes, try to pull image with authentication
        if i >= 36:  # After 3 minutes (36 * 5 seconds)
            try:
                # Test if new image is available (simplified check)
                pull_cmd = "docker pull ghcr.io/YOURUSERNAME/YOURREPO:YOURBRANCH"  # Upravte podle vašeho repozitáře
                
                test_result = subprocess.run([
                    "ssh", "-o", "StrictHostKeyChecking=no", "lewi@147.93.120.231", 
                    pull_cmd
                ], capture_output=True, text=True)
                
                if test_result.returncode == 0:
                    print("✅ Nový Docker image je připraven!")
                    return True
                else:
                    print(f"⚠️ Docker pull stále neúspěšný, čekám dál...")
                    
            except Exception as e:
                print(f"⚠️ Chyba při testování docker pull: {e}")
    
    print("❌ GitHub Actions build nedoběhl ani za 5 minut!")
    print("🛑 Deployment neúspěšný - nová verze není dostupná")
    return False

def main():
    """Main deployment function"""
    print("🚀 Starting YourApp automated deployment...")
    
    # Step 1: Ensure health endpoint is configured
    if not ensure_health_endpoint():
        print("❌ Failed to configure health endpoint")
        return False
    
    # Step 2: Auto-commit and push if needed
    changes_pushed = auto_commit_and_push()
    if changes_pushed is None:
        print("❌ Failed to commit/push changes")
        return False
    
    # Step 3: Wait for GitHub Actions if changes were pushed
    if changes_pushed:
        if not wait_for_github_actions():
            print("❌ Deployment se nezdařil - GitHub Actions nedoběhly včas")
            return False
    
    # Step 4: Deploy to server
    print("🚀 Nasazování na server...")
    
    # Configuration - UPRAVTE PODLE VAŠÍ APLIKACE
    REGISTRY_IMAGE = "ghcr.io/YOURUSERNAME/YOURREPO"
    TAG = "YOURBRANCH"  # nebo "latest" pro hlavní větev
    SERVER_USER = "lewi"
    SERVER_HOST = "147.93.120.231"
    SERVER_PATH = "/srv/docker/OptimalyDocker/apps/yourapp/"  # Název aplikace malými písmeny
    UNIVERSAL_DEPLOY_PATH = "/srv/docker/OptimalyDocker/"
    
    print("🔗 Connecting to server and deploying...")
    
    # SSH command to execute deployment on server using universal deployment
    ssh_commands = [
        f"echo 'Pulling latest image from GitHub Container Registry...'",
        f"docker pull {REGISTRY_IMAGE}:{TAG} || echo 'Pull failed, using local build'",
        f"echo 'Tagging image for local use...'",
        f"docker tag {REGISTRY_IMAGE}:{TAG} yourapp:{TAG} || echo 'Tag failed, keeping local image'",
        f"echo 'Creating TAR file from Docker image...'",
        f"mkdir -p {SERVER_PATH}",
        f"docker save yourapp:{TAG} > {SERVER_PATH}yourapp.tar",
        f"echo 'Copying app.json to server...'",
        f"echo 'Running universal deployment...'",
        f"cd {UNIVERSAL_DEPLOY_PATH}",
        f"echo 'y' | python3 deploy_universal.py yourapp",
        f"echo 'Cleaning up unused images...'",
        "docker system prune -f",
        f"echo 'Deployment completed!'"
    ]
    
    ssh_command = f"ssh -o StrictHostKeyChecking=no {SERVER_USER}@{SERVER_HOST} '{'; '.join(ssh_commands)}'"
    
    if run_command(ssh_command, "Deploying to server"):
        print("✅ Deployment completed successfully!")
        print(f"📱 Application available at: https://yourapp.optimaly.net")
        print(f"🐳 Image: {REGISTRY_IMAGE}:{TAG}")
    else:
        print("❌ Deployment failed!")
        return False
    
    return True

if __name__ == "__main__":
    main()
```

### 2. Přidání health endpoint do Program.cs

V souboru `Program.cs` přidejte:

```csharp
// Přidejte health checks službu
builder.Services.AddHealthChecks();

// Po mapování routes přidejte health endpoint
app.MapHealthChecks("/health");
```

### 3. První deployment

1. **Nastavte oprávnění pro deploy script:**
   ```bash
   chmod +x deploy.py
   ```

2. **Upravte deploy.py podle vaší aplikace:**
   - Změňte `YourApp` na název vaší aplikace
   - Upravte `YOURUSERNAME/YOURREPO` na váš GitHub username a repository
   - Upravte `YOURBRANCH` na název vaší větve (nebo `latest` pro hlavní větev)
   - Upravte `yourapp` na název vaší aplikace (malými písmeny)

3. **Zkopírujte app.json na server:**
   ```bash
   scp app.json lewi@147.93.120.231:/srv/docker/OptimalyDocker/apps/yourapp/
   ```

4. **Spusťte deployment:**
   ```bash
   python3 deploy.py
   ```

## 📝 Důležité poznámky

### GitHub Container Registry
- Docker images se automaticky buildují při každém push do GitHubu
- Image je dostupný jako `ghcr.io/username/repository:branch`
- Pro hlavní větev se používá tag `latest`

### Univerzální deployment systém
- Automaticky vytvoří Docker službu v docker-compose.yml
- Nastaví reverse proxy přes Caddy
- Zajistí HTTPS certifikát přes Let's Encrypt
- Spravuje health checks a monitoring

### Struktura na serveru
```
/srv/docker/OptimalyDocker/
├── apps/
│   ├── yourapp/
│   │   ├── app.json          # Konfigurace aplikace
│   │   ├── yourapp.tar       # Docker image
│   │   └── appsettings.json  # Volitelné nastavení
│   ├── knihovna/
│   └── mycaravanlife/
├── docker-compose.yml        # Hlavní Docker compose
├── Caddyfile                 # Reverse proxy konfigurace
└── deploy_universal.py       # Deployment script
```

### Řešení problémů

**GitHub blokuje push kvůli tokenu:**
- Nikdy nedávejte tokeny přímo do kódu
- Použijte SSH klíče nebo GitHub credential manager
- Vytvořte novou větev bez problematické historie

**Docker image se nenačítá:**
- Zkontrolujte, že GitHub Actions doběhly úspěšně
- Ověřte správný název image a tag
- Image musí být public nebo musíte mít nastavené přihlášení

**Aplikace nefunguje:**
- Zkontrolujte logy: `ssh lewi@147.93.120.231 "docker logs optimaly-yourapp"`
- Ověřte health endpoint: `curl https://yourapp.optimaly.net/health`
- Zkontrolujte, že aplikace poslouchá na portu 8080

## 🔧 Užitečné příkazy

```bash
# Zobrazit běžící kontejnery
ssh lewi@147.93.120.231 "docker ps"

# Zobrazit logy aplikace
ssh lewi@147.93.120.231 "docker logs optimaly-yourapp"

# Restartovat aplikaci
ssh lewi@147.93.120.231 "cd /srv/docker/OptimalyDocker && docker-compose restart yourapp"

# Zobrazit docker-compose konfiguraci
ssh lewi@147.93.120.231 "cat /srv/docker/OptimalyDocker/docker-compose.yml"

# Ruční deployment
ssh lewi@147.93.120.231 "cd /srv/docker/OptimalyDocker && echo 'y' | python3 deploy_universal.py yourapp"
```

## ✅ Checklist před deploymentem

- [ ] Dockerfile je v kořenovém adresáři
- [ ] app.json má správnou subdoménu
- [ ] GitHub Actions workflow je nastaven
- [ ] Health endpoint funguje (`/health`)
- [ ] Aplikace poslouchá na portu 8080
- [ ] Deploy script má správné názvy a cesty
- [ ] SSH klíč je nastaven pro přístup na server

---

💡 **Tip:** Pro inspiraci se podívejte na existující aplikace jako `knihovna` nebo `mycaravanlife` v `/srv/docker/OptimalyDocker/apps/`