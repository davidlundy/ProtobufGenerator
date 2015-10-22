@echo off
cd %~dp0

SETLOCAL
SET NUGET_VERSION=latest
SET CACHED_NUGET=%LocalAppData%\NuGet\nuget.%NUGET_VERSION%.exe

IF EXIST %CACHED_NUGET% goto copynuget
echo Downloading latest version of NuGet.exe...
IF NOT EXIST %LocalAppData%\NuGet md %LocalAppData%\NuGet
@powershell -NoProfile -ExecutionPolicy unrestricted -Command "$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest 'https://dist.nuget.org/win-x86-commandline/%NUGET_VERSION%/nuget.exe' -OutFile '%CACHED_NUGET%'"

:copynuget
IF EXIST .nuget\nuget.exe goto restore
md .nuget
copy %CACHED_NUGET% .nuget\nuget.exe > nul

:restore
IF NOT EXIST packages\Sake (
	.nuget\nuget.exe install Sake -Version 0.2.2 -ExcludeVersion -Out packages -Source https://api.nuget.org/v3/index.json
)
IF NOT EXIST packages\dnx-clr-win-x64 (
	.nuget\nuget.exe install dnx-clr-win-x64 -Version 1.0.0-beta8 -ExcludeVersion -Out packages -Source https://www.myget.org/F/aspnetmaster/api/v3/index.json
)
IF NOT EXIST packages\dnx-coreclr-win-x64 (
	.nuget\nuget.exe install dnx-coreclr-win-x64 -Version 1.0.0-beta8 -ExcludeVersion -Out packages -Source https://www.myget.org/F/aspnetmaster/api/v3/index.json
)

:run
packages\Sake\tools\Sake.exe -f makefile.shade %*