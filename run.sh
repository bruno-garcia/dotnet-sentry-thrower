#!/bin/bash
set -e

rm -rf ~/.nuget/packages/sharpraven
dotnet restore --ignore-failed-sources
dotnet build -c Release
dotnet bin/Release/netcoreapp2.0/CoreThrower.dll
