# 🚀 Template pro rychlé nasazení nových aplikací

Tento soubor slouží jako rychlá reference pro deployment nových .NET aplikací do Optimaly infrastruktury.

## 📝 Rychlý checklist pro novou aplikaci

### 1. Příprava projektu (5 minut)
- [ ] Zkopírovat `Dockerfile` z knihovna větve
- [ ] Upravit názvy projektů v Dockerfile
- [ ] Zkopírovat `.github/workflows/docker-build.yml`
- [ ] Upravit název větve v workflow (obvykle `main`)
- [ ] Zkopírovat `deploy.py` 
- [ ] Upravit všechny názvy a cesty v deploy.py

### 2. Konfigurace app.json (2 minuty)
```json
{
  "subdomain": "NAZEVAPLIKACE.optimaly.net",
  "port": 8080,
  "health_endpoint": "/health",
  "environment": {
    "ASPNETCORE_ENVIRONMENT": "Production",
    "ASPNETCORE_URLS": "http://+:8080",
    "ASPNETCORE_FORWARDEDHEADERS_ENABLED": "true"
  },
  "volumes": [],
  "depends_on": ["caddy"],
  "networks": ["optimaly-web", "optimaly-apps"],
  "restart_policy": "unless-stopped",
  "database_connection_var": "NAZEVAPLIKACE_DB_CONNECTION"
}
```

### 3. Health endpoint v Program.cs (1 minuta)
```csharp
// Přidat do services
builder.Services.AddHealthChecks();

// Přidat do middleware
app.MapHealthChecks("/health");
```

### 4. První deployment (2 minuty)
```bash
# Zkopírovat app.json na server
scp app.json lewi@147.93.120.231:/srv/docker/OptimalyDocker/apps/NAZEVAPLIKACE/

# Spustit deployment
python3 deploy.py
```

## 🔧 Co upravit pro novou aplikace

**deploy.py:**
- `YourApp` → název vaší aplikace
- `YOURUSERNAME/YOURREPO` → váš GitHub repo
- `YOURBRANCH` → název větve (obvykle `main`)
- `yourapp` → název aplikace malými písmeny

**Dockerfile:**
- `YourApp/YourApp.csproj` → cesta k vašemu projektu
- `YourApp.dll` → název DLL souboru

**GitHub Actions:**
- `branches: [ main ]` → vaše hlavní větev

## 🎯 Výsledek
- Aplikace dostupná na `https://NAZEVAPLIKACE.optimaly.net`
- Automatické HTTPS certifikáty
- Health monitoring
- Zero-downtime deployment

## 💡 Pro další poptávky
Tento template umožní nasadit novou aplikaci za **10 minut** místo hodin!

---
*Vytvořeno pro Optimaly s.r.o. - jednodušší deployment pro budoucí projekty*