#!/usr/bin/env bash
set -euo pipefail
RUNTIME="${1:-win-x64}"
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
for p in "Launcher/StartLauncher/StartLauncher.csproj" "Services/Order/Order.csproj" "Services/Inventory/Inventory.csproj"; do
  dotnet restore "$ROOT/$p" --configfile "$ROOT/nuget.config"
done
mkdir -p "$ROOT/dist/Services/Order" "$ROOT/dist/Services/Inventory" "$ROOT/dist/Launcher"
dotnet publish "$ROOT/Services/Order/Order.csproj" -c Release -r "$RUNTIME" --self-contained false -o "$ROOT/dist/Services/Order"
dotnet publish "$ROOT/Services/Inventory/Inventory.csproj" -c Release -r "$RUNTIME" --self-contained false -o "$ROOT/dist/Services/Inventory"
dotnet publish "$ROOT/Launcher/StartLauncher/StartLauncher.csproj" -c Release -r "$RUNTIME" --self-contained false -o "$ROOT/dist/Launcher"
echo "Published under dist/. Run Launcher/StartLauncher.exe"
