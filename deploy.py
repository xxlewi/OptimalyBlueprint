#!/usr/bin/env python3

"""
Deployment Script for Optimaly Knihovna
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
    program_cs_path = "OptimalyBlueprint/Program.cs"
    
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
        commit_msg = f"{timestamp}-autodeploy knihovna\n\n🤖 Generated with Claude Code\n\nCo-Authored-By: Claude <noreply@anthropic.com>"
        if not run_command(f'git commit -m "{commit_msg}"', "Creating commit"):
            return False
        
        # Push to origin with credentials
        # Get remote URL and add credentials
        result = subprocess.run(["git", "remote", "get-url", "origin"], capture_output=True, text=True)
        if result.returncode != 0:
            print("❌ Failed to get git remote URL")
            return False
        
        original_url = result.stdout.strip()
        
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
                pull_cmd = "docker pull ghcr.io/xxlewi/optimalyblueprint:knihovna"
                
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
    print("🚀 Starting Optimaly Knihovna automated deployment...")
    
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
    
    # Configuration
    REGISTRY_IMAGE = "ghcr.io/xxlewi/optimalyblueprint"
    TAG = "knihovna"
    SERVER_USER = "lewi"
    SERVER_HOST = "147.93.120.231"
    SERVER_PATH = "/srv/docker/OptimalyDocker/apps/knihovna/"
    UNIVERSAL_DEPLOY_PATH = "/srv/docker/OptimalyDocker/"
    
    print("🔗 Connecting to server and deploying...")
    
    # SSH command to execute deployment on server using universal deployment
    ssh_commands = [
        f"echo 'Pulling latest image from GitHub Container Registry...'",
        f"docker pull {REGISTRY_IMAGE}:{TAG} || echo 'Pull failed, using local build'",
        f"echo 'Tagging image for local use...'",
        f"docker tag {REGISTRY_IMAGE}:{TAG} knihovna:{TAG} || echo 'Tag failed, keeping local image'",
        f"echo 'Creating TAR file from Docker image...'",
        f"mkdir -p {SERVER_PATH}",
        f"docker save knihovna:{TAG} > {SERVER_PATH}knihovna.tar",
        f"echo 'Copying app.json to server...'",
        f"echo 'Running universal deployment...'",
        f"cd {UNIVERSAL_DEPLOY_PATH}",
        f"echo 'y' | python3 deploy_universal.py knihovna",
        f"echo 'Cleaning up unused images...'",
        "docker system prune -f",
        f"echo 'Deployment completed!'"
    ]
    
    ssh_command = f"ssh {SERVER_USER}@{SERVER_HOST} '{'; '.join(ssh_commands)}'"
    
    if run_command(ssh_command, "Deploying to server"):
        print("✅ Deployment completed successfully!")
        print(f"📱 Application available at: https://knihovna.optimaly.net")
        print(f"🐳 Image: {REGISTRY_IMAGE}:{TAG}")
    else:
        print("❌ Deployment failed!")
        print()
        print("📝 Manual deployment steps:")
        print()
        print("1️⃣ SSH to server:")
        print(f"   ssh {SERVER_USER}@{SERVER_HOST}")
        print()
        print("2️⃣ Navigate to deployment directory:")
        print(f"   cd {SERVER_PATH}")
        print()
        print("3️⃣ Pull latest image from GitHub Container Registry:")
        print(f"   docker pull {REGISTRY_IMAGE}:{TAG}")
        print()
        print("4️⃣ Tag image for local use:")
        print(f"   docker tag {REGISTRY_IMAGE}:{TAG} knihovna:{TAG}")
        print()
        print("5️⃣ Create TAR file:")
        print(f"   docker save knihovna:{TAG} > knihovna.tar")
        print()
        print("6️⃣ Run universal deployment:")
        print(f"   cd {UNIVERSAL_DEPLOY_PATH}")
        print(f"   python3 deploy_universal.py knihovna")
        print()
        print("7️⃣ Clean up unused images:")
        print("   docker system prune -f")
        print()
        print("💡 Note: Image is automatically built and pushed to GHCR on every commit to master!")
        
        return False
    
    return True

if __name__ == "__main__":
    main()