param
(
	[string] $appPrefix,
	[string] $environment,
	[string] $webAppEnvironment,
	[string] $resourceGroupName,
	[string] $resourceGroupLocation,
	[string] $resourceGroupLocationShort,
	[string] $dataKeyVaultName,
	[string] $templatesLocation,
	[string] $contactEmailAddress,
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
Write-Host "letsEncryptAppServiceContributerClientSecret: $letsEncryptAppServiceContributerClientSecret"

#Variables
$webSiteName = "$appPrefix-$webAppEnvironment-$resourceGroupLocationShort-web"
$webhostingName = "$appPrefix-$environment-$resourceGroupLocationShort-hostingplan"
$storageAccountName = "$appPrefix$environment$($resourceGroupLocationShort)storage" #Must be <= 24 lowercase letters and numbers.
$actionGroupName = "$appPrefix-$webAppEnvironment-$resourceGroupLocationShort-actionGroup"
if ($webAppEnvironment -ne "Prod")
{
	$websiteDomainName = "$webAppEnvironment.samlearnsazure.com"
}
else
{
	$websiteDomainName = "samlearnsazure.com"
}
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
$timing = -join($timing, "3. Website created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "3. Website created: "$stopwatch.Elapsed.TotalSeconds

Write-Host "websitePrincipalId: "$websiteProdSlotIdentityPrincipalId
Write-Host "websiteStagingSlotPrincipalId: "$websiteStagingSlotIdentityPrincipalId
$timing = -join($timing, "4. All Done: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "4. All Done: "$stopwatch.Elapsed.TotalSeconds
Write-Host "Timing: `n$timing"
Write-Host "Were there errors? (If the next line is blank, then no!) $error"