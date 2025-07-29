#!/bin/bash
# Kill all dotnet processes for OptimalyBlueprint project
pkill -f "dotnet.*OptimalyBlueprint" 2>/dev/null || true
echo "OptimalyBlueprint dotnet processes killed (if any were running)"