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
	.nuget\nuget.exe install dnx-clr-win-x64 -Version 1.0.0-rc1-final -ExcludeVersion -Out packages -Source https://www.myget.org/F/aspnetmaster/api/v3/index.json
)
IF NOT EXIST packages\dnx-coreclr-win-x64 (
	.nuget\nuget.exe install dnx-coreclr-win-x64 -Version 1.0.0-rc1-final -ExcludeVersion -Out packages -Source https://www.myget.org/F/aspnetmaster/api/v3/index.json
)
IF NOT EXIST packages\OpenCover (
	.nuget\nuget.exe install OpenCover -Version 4.6.210-rc -ExcludeVersion -Out packages -Source https://api.nuget.org/v3/index.json
)
IF NOT EXIST packages\ReportGenerator (
	.nuget\nuget.exe install ReportGenerator -Version 2.3.4 -ExcludeVersion -Out packages -Source https://api.nuget.org/v3/index.json
)
IF NOT EXIST packages\coveralls.net (
	.nuget\nuget.exe install coveralls.net -Version 0.6.0 -ExcludeVersion -Out packages -Source https://api.nuget.org/v3/index.json
)

:run
packages\Sake\tools\Sake.exe -f makefile.shade %*

IF EXIST %APPVEYOR% (
	packages\coveralls.net\tools\csmacnz.Coveralls.exe --opencover -i ./artifacts/reports/coverage.xml --repoToken %COVERALLS_REPO_TOKEN% --commitId %APPVEYOR_REPO_COMMIT% --commitBranch %APPVEYOR_REPO_BRANCH% --commitAuthor %APPVEYOR_REPO_COMMIT_AUTHOR% --commitEmail %APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL% --commitMessage %APPVEYOR_REPO_COMMIT_MESSAGE% --jobId %APPVEYOR_JOB_ID%
)
