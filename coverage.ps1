# http://stackoverflow.com/questions/36460254/vs2015-code-coverage-not-working-with-tests-in-asp-net-core-1-0-formerly-known/39324025#39324025

Write-Host "Installing Dependencies`n" -foregroundcolor green
nuget install OpenCover -Version 4.6.519 -OutputDirectory .\tools
nuget install coveralls.net -Version 0.7.0 -OutputDirectory .\tools
nuget install ReportGenerator -Version 2.4.5 -OutputDirectory .\tools

Write-Host "`nCleaning Output Directory`n" -foregroundcolor red
If (Test-Path opencover.xml){
	Remove-Item opencover.xml -Confirm:$false
}

If (Test-Path reports){
	Remove-Item reports -Recurse -Confirm:$false
}

Write-Host "`nSwitching to Full Debug Output - Only valid on .NET Full Framework`n" -foregroundcolor red

Get-ChildItem -Path src -Recurse -Filter project.json | % {
	$_.CopyTo("$($_.FullName).bak", $true);
	$project = Get-Content "$($_.FullName).bak" -raw | ConvertFrom-Json;
	$project.buildOptions.debugType = "full";
	$project | ConvertTo-Json -Compress | Set-Content "$($_.FullName)"
}

Write-Host "`nCalculating Code Coverage" -foregroundcolor green
Get-ChildItem -Path test -Recurse -Filter project.json | % { Write-Host $_.FullName; .\tools\OpenCover.4.6.519\tools\OpenCover.Console.exe -target:"C:\Program Files\dotnet\dotnet.exe" -targetargs:" test ""$($_.FullName)"" " -register:user -filter:"+[*]* -[xunit*]* -[FluentAssertions*]* -[*.Tests]*" -returntargetcode -mergeoutput -output:opencover_results.xml -oldstyle } 

.\tools\ReportGenerator.2.4.5.0\tools\ReportGenerator.exe -reports:"opencover.xml" -targetdir:".\reports"

.\tools\coveralls.net.0.7.0\tools\csmacnz.Coveralls.exe --opencover -i .\opencover_results.xml