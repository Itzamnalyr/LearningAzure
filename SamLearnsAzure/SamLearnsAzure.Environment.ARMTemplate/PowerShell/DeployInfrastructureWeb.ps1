param
(
	[string] $appPrefix,
	[string] $environment,
	[string] $resourceGroupName,
	[string] $resourceGroupLocation,
	[string] $resourceGroupLocationShort,
	[string] $dataKeyVaultName,
	[string] $templatesLocation,
	[string] $contactEmailAddress,
	[string] $sqlDatabaseName,
	[string] $sqlAdministratorLoginUser, 
	[string] $sqlAdministratorLoginPassword
	[string] $letsEncryptAppServiceContributerClientSecret
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
Write-Host "sqlDatabaseName: $sqlDatabaseName"
Write-Host "sqlAdministratorLoginUser: $sqlAdministratorLoginUser"
Write-Host "sqlAdministratorLoginPassword: $sqlAdministratorLoginPassword"
Write-Host "letsEncryptAppServiceContributerClientSecret: $letsEncryptAppServiceContributerClientSecret"

#Variables
$storageAccountName = "$appPrefix$environment$($resourceGroupLocationShort)storage" #Must be <= 24 lowercase letters and numbers.
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
$serviceAPIName = "$appPrefix-$environment-$resourceGroupLocationShort-service"
$webSiteName = "$appPrefix-$environment-$resourceGroupLocationShort-web"
$sqlServerName = "$appPrefix-$environment-$resourceGroupLocationShort-sqlserver"
$sqlServerAddress = "$sqlServerName.database.windows.net"
if ($storageAccountName.Length -gt 24)
{
    Write-Host "Storage account name must be 3-24 characters in length"
    Break
}
if ($letsEncryptAppServiceContributerClientSecret -eq $null)
{
    Write-Host "$letsEncryptAppServiceContributerClientSecret is null. Please set this secret before continuing"
    Break
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


#Web service
az deployment group create --resource-group $resourceGroupName --name $serviceAPIName --template-file "$templatesLocation\WebService.json" --parameters serviceAPIName=$serviceAPIName hostingPlanName=$webhostingName actionGroupName=$actionGroupName sqlServerName=$sqlServerName sqlServerAddress=$sqlServerAddress sqlDatabaseName=$sqlDatabaseName sqlDatabaseLoginName=$sqlAdministratorLoginUser sqlDatabaseLoginPassword=$sqlAdministratorLoginPassword
#web service managed identity and setting keyvault access permissions
$serviceAPIProdSlotIdentity = az webapp identity assign --resource-group $resourceGroupName --name $serviceAPIName 
$serviceAPIStagingSlotIdentity = az webapp identity assign --resource-group $resourceGroupName --name $serviceAPIName  --slot staging
$serviceAPIProdSlotIdentityPrincipalId = ($serviceAPIProdSlotIdentity | ConvertFrom-Json | SELECT PrincipalId).PrincipalId
$serviceAPIStagingSlotIdentityPrincipalId =($serviceAPIStagingSlotIdentity | ConvertFrom-Json | SELECT PrincipalId).PrincipalId
Write-Host "Setting access policies for key vault"
az keyvault set-policy --name $dataKeyVaultName --object-id $serviceAPIProdSlotIdentityPrincipalId --secret-permissions list get
az keyvault set-policy --name $dataKeyVaultName --object-id $serviceAPIStagingSlotIdentityPrincipalId --secret-permissions list get
#Web service alerts
az deployment group create --resource-group $resourceGroupName --name "webServiceAlerts" --template-file "$templatesLocation\WebAppAlerts.json" --parameters webAppName=$serviceAPIName actionGroupName=$actionGroupName 
$timing = -join($timing, "6. Web service created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "6. Web service created: "$stopwatch.Elapsed.TotalSeconds


#Web site
az deployment group create --resource-group $resourceGroupName --name $webSiteName --template-file "$templatesLocation\Website.json" --parameters webSiteName=$webSiteName hostingPlanName=$webhostingName actionGroupName=$actionGroupName storageAccountName=$storageAccountName websiteDomainName=$websiteDomainName contactEmailAddress=$contactEmailAddress letsEncryptAppServiceContributerClientSecret=$letsEncryptAppServiceContributerClientSecret
#web site managed identity and setting keyvault access permissions
$websiteProdSlotIdentity = az webapp identity assign --resource-group $resourceGroupName --name $webSiteName 
$websiteStagingSlotIdentity = az webapp identity assign --resource-group $resourceGroupName --name $webSiteName  --slot staging
$websiteProdSlotIdentityPrincipalId = ($websiteProdSlotIdentity | ConvertFrom-Json | SELECT PrincipalId).PrincipalId
$websiteStagingSlotIdentityPrincipalId =($websiteStagingSlotIdentity | ConvertFrom-Json | SELECT PrincipalId).PrincipalId
Write-Host "prod: " $websiteProdSlotIdentityPrincipalId
Write-Host "staging: " $websiteStagingSlotIdentityPrincipalId
Write-Host "Setting access policies for key vault"
az keyvault set-policy --name $dataKeyVaultName --object-id $websiteProdSlotIdentityPrincipalId --secret-permissions list get
az keyvault set-policy --name $dataKeyVaultName --object-id $websiteStagingSlotIdentityPrincipalId --secret-permissions list get
#Website alerts
az deployment group create --resource-group $resourceGroupName --name "webSiteAlerts" --template-file "$templatesLocation\WebAppAlerts.json" --parameters webAppName=$webSiteName actionGroupName=$actionGroupName 
$timing = -join($timing, "7. Website created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "7. Website created: "$stopwatch.Elapsed.TotalSeconds

Write-Host "servicePrincipalId: "$serviceAPIProdSlotIdentityPrincipalId
Write-Host "serviceStagingSlotPrincipalId: "$serviceAPIStagingSlotIdentityPrincipalId
Write-Host "applicationInsightsInstrumentationKey: "$applicationInsightsInstrumentationKey
Write-Host "websitePrincipalId: "$websiteProdSlotIdentityPrincipalId
Write-Host "websiteStagingSlotPrincipalId: "$websiteStagingSlotIdentityPrincipalId
$timing = -join($timing, "8. All Done created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "8. All Done: "$stopwatch.Elapsed.TotalSeconds
Write-Host "Timing: `n$timing"
Write-Host "Were there errors? (If the next line is blank, then no!) $error"