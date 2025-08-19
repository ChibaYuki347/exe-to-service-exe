param([string]$Runtime="win-x64")
$ErrorActionPreference="Stop"
$root = Resolve-Path (Join-Path (Split-Path -Parent $MyInvocation.MyCommand.Path) "..")
dotnet restore (Join-Path $root "Launcher/StartLauncher/StartLauncher.csproj") --configfile (Join-Path $root "nuget.config")
dotnet restore (Join-Path $root "Services/Order/Order.csproj") --configfile (Join-Path $root "nuget.config")
dotnet restore (Join-Path $root "Services/Inventory/Inventory.csproj") --configfile (Join-Path $root "nuget.config")
New-Item -ItemType Directory -Force -Path (Join-Path $root "dist/Services/Order") | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $root "dist/Services/Inventory") | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $root "dist/Launcher") | Out-Null
dotnet publish (Join-Path $root "Services/Order/Order.csproj") -c Release -r $Runtime --self-contained false -o (Join-Path $root "dist/Services/Order")
dotnet publish (Join-Path $root "Services/Inventory/Inventory.csproj") -c Release -r $Runtime --self-contained false -o (Join-Path $root "dist/Services/Inventory")
dotnet publish (Join-Path $root "Launcher/StartLauncher/StartLauncher.csproj") -c Release -r $Runtime --self-contained false -o (Join-Path $root "dist/Launcher")
Write-Host "Published under dist/. Run Launcher/StartLauncher.exe"
