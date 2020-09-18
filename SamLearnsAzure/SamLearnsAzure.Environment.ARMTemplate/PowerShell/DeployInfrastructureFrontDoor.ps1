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
$frontDoorName = "$appPrefix-$environment-$resourceGroupLocationShort-frontdoor"
$webSiteName = "$appPrefix-$environment-$resourceGroupLocationShort-web"
$serviceAPIName = "$appPrefix-$environment-$resourceGroupLocationShort-service"
$frontDoorBackEndAddresses = "['$webSiteName.azurewebsites.net']"  #create an array of strings for each of the back end pool resources
$timing = -join($timing, "2. Variables created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "2. Variables created: "$stopwatch.Elapsed.TotalSeconds

#Frontdoor
az deployment group create --resource-group $resourceGroupName --name $frontDoorName --template-file "$templatesLocation\FrontDoor.json" --parameters frontDoorName=$frontDoorName frontDoorBackEndAddresses=$frontDoorBackEndAddresses 
$timing = -join($timing, "14. Frontdoor created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "14. Frontdoor created: "$stopwatch.Elapsed.TotalSeconds

Write-Host "sqlServerIPAddress: "$sqlServerIPAddress
$timing = -join($timing, "15. All Done: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "15. All Done: "$stopwatch.Elapsed.TotalSeconds
Write-Host "Timing: `n$timing"
Write-Host "Were there errors? (If the next line is blank, then no!) $error"