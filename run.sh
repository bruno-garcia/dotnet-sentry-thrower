#!/bin/bash
set -e

rm -rf ~/.nuget/packages/sharpraven
dotnet restore --ignore-failed-sources
dotnet build -c Release

#dotnet bin/Release/netcoreapp2.0/CoreThrower.dll

for throwerPathDll in bin/Release/*/CoreThrower.dll; do
    [ -e "$throwerPathDll" ] || continue
    echo $throwerPathDll
    dotnet $throwerPathDll
done
for throwerPathExe in bin/Release/*/CoreThrower.exe; do
    [ -e "$throwerPathExe" ] || continue
    echo $throwerPathExe
    mono $throwerPathExe
done
