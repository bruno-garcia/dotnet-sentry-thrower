if exist %userprofile%\.nuget\packages\sharpraven (
	rd /s /q %userprofile%\.nuget\packages\sharpraven || exit /b %errorlevel%
)
dotnet restore --ignore-failed-sources || exit /b %errorlevel%
dotnet build -c Release  || exit /b %errorlevel%
dotnet bin/Release/netcoreapp2.0/CoreThrower.dll
