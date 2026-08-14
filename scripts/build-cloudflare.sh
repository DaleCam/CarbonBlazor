#!/usr/bin/env bash
set -euo pipefail

export DOTNET_ROOT="$PWD/.dotnet"
export PATH="$DOTNET_ROOT:$PATH"

if ! command -v dotnet >/dev/null 2>&1; then
  curl -sSL https://dot.net/v1/dotnet-install.sh -o dotnet-install.sh
  bash dotnet-install.sh --channel 10.0 --install-dir "$DOTNET_ROOT"
fi

dotnet publish CarbonBlazor.Demo/CarbonBlazor.Demo.csproj \
  -c Release \
  -o publish \
  -p:RunAOTCompilation=false \
  -p:WasmBuildNative=false
