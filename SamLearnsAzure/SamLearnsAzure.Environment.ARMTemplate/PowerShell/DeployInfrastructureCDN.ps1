param
(
	[string] $appPrefix,
	[string] $environment,
	[string] $resourceGroupName,
	[string] $resourceGroupLocation,
	[string] $resourceGroupLocationShort,
	[string] $dataKeyVaultName,
	[string] $templatesLocation
)

$stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
$timing = ""
$timing = -join($timing, "1. Deployment started: ", $stopwatch.Elapsed.TotalSeconds, "`n")
Write-Host "1. Deployment started: "$stopwatch.Elapsed.TotalSeconds
Write-Host "Parameters:"
Write-Host "appPrefix: $appPrefix"
Write-Host "environment: $environment"
Write-Host "resourceGroupName: $resourceGroupName"
Write-Host "resourceGroupLocation: $resourceGroupLocation"
Write-Host "resourceGroupLocationShort: $resourceGroupLocationShort"
Write-Host "dataKeyVaultName: $dataKeyVaultName"
Write-Host "templatesLocation: $templatesLocation"

#Variables
$storageAccountName = "$appPrefix$environment$($resourceGroupLocationShort)storage" #Must be <= 24 lowercase letters and numbers.
$cdnName = "$appPrefix-$environment-$resourceGroupLocationShort-cdn"   
if ($storageAccountName.Length -gt 24)
{
    Write-Host "Storage account name must be 3-24 characters in length"
    Break
}
$CheckWhatIfs = $true
$timing = -join($timing, "2. Variables created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "2. Variables created: "$stopwatch.Elapsed.TotalSeconds

#CDN
if ($CheckWhatIfs -eq $true)
{
    $whatifResultsJson = az deployment group what-if --no-pretty-print --only-show-errors --resource-group $resourceGroupName --name $cdnName --template-file "$templatesLocation\CDN.json" --parameters cdnName=$cdnName storageAccountName=$storageAccountName
    $whatifResults = $whatifResultsJson | ConvertFrom-Json 
    $ChangeResults6 = $whatifResults.changes 

    #$ChangeResults6b = $ChangeResults6 | Where-Object { $_.changeType -eq "Modify" }
    #$ChangeResults6b.delta.path
}
if ($CheckWhatIfs -eq $false -or $ChangeResults6.changeType -eq "Create" -or $ChangeResults6.changeType -eq "Modify")
{
    az deployment group create --resource-group $resourceGroupName --name $cdnName --template-file "$templatesLocation\CDN.json" --parameters cdnName=$cdnName storageAccountName=$storageAccountName
}
else
{
    Write-Host "6. CDN CheckWhatIf: $CheckWhatIfs and change type: $($ChangeResults6.changeType) results"
}
$timing = -join($timing, "6. CDN  created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "6. CDN created: "$stopwatch.Elapsed.TotalSeconds

$timing = -join($timing, "15. All Done: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "15. All Done: "$stopwatch.Elapsed.TotalSeconds
Write-Host "Timing: `n$timing"
Write-Host "Were there errors? (If the next line is blank, then no!) $error"