param
(
	[string] $appPrefix,
	[string] $environment,
	[string] $webAppEnvironment,
	[string] $resourceGroupName,
	[string] $resourceGroupLocation,
	[string] $resourceGroupLocationShort
)

$stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
$timing = ""
$timing = -join($timing, "1. Deployment started: ", $stopwatch.Elapsed.TotalSeconds, "`n")
Write-Host "1. Deployment started: "$stopwatch.Elapsed.TotalSeconds
Write-Host "Parameters:"
Write-Host "appPrefix: $appPrefix"
Write-Host "environment: $environment"
Write-Host "webAppEnvironment: $webAppEnvironment"
Write-Host "resourceGroupName: $resourceGroupName"
Write-Host "resourceGroupLocation: $resourceGroupLocation"
Write-Host "resourceGroupLocationShort: $resourceGroupLocationShort"

#Variables
$webSiteName = "$appPrefix-$webAppEnvironment-$resourceGroupLocationShort-web"
#$webSite2Name = "$appPrefix-$(webAppEnvironment)2-$resourceGroupLocationShort-web"
$webServiceURL = "https://$appPrefix-$webAppEnvironment-$resourceGroupLocationShort-service.azurewebsites.net/"
#$webService2URL = "https://$appPrefix-$(webAppEnvironment)2-$resourceGroupLocationShort-service-staging.azurewebsites.net/"

$timing = -join($timing, "2. Variables created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "2. Variables created: "$stopwatch.Elapsed.TotalSeconds

#Set secrets into appsettings 
Write-Host "Setting appsettings $webSiteName connectionString: $WebServiceURL"

az webapp config appsettings set --resource-group $resourceGroupName --name $webSiteName --settings "AppSettings.WebServiceURL=$WebServiceURL" 
#az webapp config appsettings set --resource-group $resourceGroupName --name $webSite2Name --settings "AppSettings.WebServiceURL=$WebService2URL" 

$timing = -join($timing, "14.5. Web configuration set: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "14.5. Web configuration set: "$stopwatch.Elapsed.TotalSeconds

$timing = -join($timing, "15. All Done: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "15. All Done: "$stopwatch.Elapsed.TotalSeconds
Write-Host "Timing: `n$timing"
Write-Host "Were there errors? (If the next line is blank, then no!) $error"