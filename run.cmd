@echo off
if exist %userprofile%\.nuget\packages\sharpraven (
	rd /s /q %userprofile%\.nuget\packages\sharpraven || exit /b %errorlevel%
)
dotnet restore --ignore-failed-sources || exit /b %errorlevel%
dotnet build -c Release  || exit /b %errorlevel%

setlocal enabledelayedexpansion
for /f %%a in ('dir /s /b CoreThrower.exe CoreThrower.dll ^| find /v "obj" ^| find /v "Debug"') do (
	set throwerPath=%%a
	echo.
	echo Running: !throwerPath!
	if "!throwerPath:~-3!" == "exe" (
		!throwerPath!
	) else (
		dotnet !throwerPath!
	)
)
endlocal