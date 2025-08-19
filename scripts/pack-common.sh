#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
pushd "$ROOT/Common/Contoso.Common" >/dev/null
dotnet pack -c Release
popd >/dev/null
mkdir -p "$ROOT/local-packages"
find "$ROOT/Common/Contoso.Common/bin/Release" -name "*.nupkg" ! -name "*.snupkg" -exec cp -f {} "$ROOT/local-packages/" \;
echo "copied to local-packages"
