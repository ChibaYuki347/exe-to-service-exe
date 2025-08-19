$ErrorActionPreference="Stop"
$root = Resolve-Path (Join-Path (Split-Path -Parent $MyInvocation.MyCommand.Path) "..")
Push-Location (Join-Path $root "Common/Contoso.Common")
dotnet pack -c Release
Pop-Location
New-Item -ItemType Directory -Force -Path (Join-Path $root "local-packages") | Out-Null
Get-ChildItem -Recurse (Join-Path $root "Common/Contoso.Common/bin/Release") -Filter *.nupkg | Where-Object { $_.Name -notlike "*.snupkg" } | ForEach-Object {
  Copy-Item $_.FullName (Join-Path $root "local-packages") -Force
}
Write-Host "copied to local-packages"
