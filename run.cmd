@echo off

if "%1" == "--no-build" (
	set message=%2
	goto run
) else (
	set message=%1
)

if exist %userprofile%\.nuget\packages\sharpraven (
	rd /s /q %userprofile%\.nuget\packages\sharpraven || exit /b %errorlevel%
)
if exist bin\Release (
	rd /s /q bin\Release || exit /b %errorlevel%
)
dotnet restore --ignore-failed-sources || exit /b %errorlevel%
dotnet build -c Release  || exit /b %errorlevel%

:run
setlocal enabledelayedexpansion
for /f %%a in ('dir /s /b CoreThrower.exe CoreThrower.dll ^| find /v "obj" ^| find /v "Debug"') do (
	set throwerPath=%%a
	echo.
	echo Running: !throwerPath!
	if "!throwerPath:~-3!" == "exe" (
		!throwerPath! %message%
	) else (
		dotnet !throwerPath! %message%
	)
)
endlocal