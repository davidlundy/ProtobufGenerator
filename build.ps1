Write-Host "`nStarting ProtobufGenerator Build `n" -foregroundcolor DarkGreen
Write-Host "Restoring NuGet Packages `n" -foregroundcolor DarkGreen
dotnet restore
Write-Host "`nBuilding Solution`n" -foregroundcolor DarkGreen
dotnet build -c Release --no-incremental src/**/project.json
Write-Host "`nRunning Tests" -foregroundcolor DarkGreen
Get-ChildItem -Path test -Recurse -Filter project.json | % { Write-Host "`nTesting $($_.FullName)`n"; dotnet test $_.FullName; } 
Write-Host "`nBuild finished. Press Any Key to Continue.`n" -foregroundcolor Green
$x = $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")