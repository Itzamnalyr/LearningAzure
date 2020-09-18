param
(
	[string] $appPrefix,
	[string] $environment,
	[string] $resourceGroupName,
	[string] $resourceGroupLocation,
	[string] $resourceGroupLocationShort,
	[string] $dataKeyVaultName,
	[string] $templatesLocation,
	[string] $contactEmailAddress
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
Write-Host "contactEmailAddress: $contactEmailAddress"

#Variables
$webhostingName = "$appPrefix-$environment-$resourceGroupLocationShort-hostingplan"
$actionGroupName = "$appPrefix-$environment-$resourceGroupLocationShort-actionGroup"
$actionGroupShortName = "$environment-actgrp"
$applicationInsightsName = "$appPrefix-$environment-$resourceGroupLocationShort-appinsights"
$applicationInsightsAvailablityTestName = "Availability home page test-$applicationInsightsName"
if ($environment -ne "Prod")
{
	$websiteDomainName = "$environment.samlearnsazure.com"
}
else
{
	$websiteDomainName = "samlearnsazure.com"
}
$timing = -join($timing, "2. Variables created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "2. Variables created: "$stopwatch.Elapsed.TotalSeconds

#Action Group
az deployment group create --resource-group $resourceGroupName --name $actionGroupName --template-file "$templatesLocation\ActionGroup.json" --parameters actionGroupName=$actionGroupName appPrefix=$appPrefix environment=$environment contactEmailAddress=$contactEmailAddress
$timing = -join($timing, "3. Action group created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "3. Action group created: "$stopwatch.Elapsed.TotalSeconds

#Application Insights
$applicationInsightsOutput = az deployment group create --resource-group $resourceGroupName --name $applicationInsightsName --template-file "$templatesLocation\ApplicationInsights.json" --parameters applicationInsightsName=$applicationInsightsName applicationInsightsAvailablityTestName="$applicationInsightsAvailablityTestName" websiteDomainName=$websiteDomainName 
$applicationInsightsJSON = $applicationInsightsOutput | ConvertFrom-Json
$applicationInsightsInstrumentationKey = $applicationInsightsJSON.properties.outputs.applicationInsightsInstrumentationKeyOutput.value
$applicationInsightsInstrumentationKeyName = "ApplicationInsights--InstrumentationKey$Environment"
Write-Host "Setting value $ApplicationInsightsInstrumentationKey for $applicationInsightsInstrumentationKeyName to key vault"
az keyvault secret set --vault-name $dataKeyVaultName --name "$applicationInsightsInstrumentationKeyName" --value $ApplicationInsightsInstrumentationKey #Upload the secret into the key vault
$timing = -join($timing, "4. Application created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "4. Application insights created: "$stopwatch.Elapsed.TotalSeconds

#Web hosting
az deployment group create --resource-group $resourceGroupName --name $webhostingName --template-file "$templatesLocation\WebHosting.json" --parameters hostingPlanName=$webhostingName actionGroupName=$actionGroupName 
$timing = -join($timing, "5. Web hosting created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "5. Web hosting created: "$stopwatch.Elapsed.TotalSeconds

Write-Host "applicationInsightsInstrumentationKey: "$applicationInsightsInstrumentationKey
$timing = -join($timing, "6. All Done: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "6. All Done: "$stopwatch.Elapsed.TotalSeconds
Write-Host "Timing: `n$timing"
Write-Host "Were there errors? (If the next line is blank, then no!) $error"