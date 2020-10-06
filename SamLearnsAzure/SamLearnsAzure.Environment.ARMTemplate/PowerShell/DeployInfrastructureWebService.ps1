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
	[string] $sqlDatabaseName,
	[string] $sqlAdministratorLoginUser, 
	[string] $sqlAdministratorLoginPassword
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
Write-Host "dataKeyVaultName: $dataKeyVaultName"
Write-Host "templatesLocation: $templatesLocation"
Write-Host "sqlDatabaseName: $sqlDatabaseName"
Write-Host "sqlAdministratorLoginUser: $sqlAdministratorLoginUser"
Write-Host "sqlAdministratorLoginPassword: $sqlAdministratorLoginPassword"

#Variables
$serviceAPIName = "$appPrefix-$webAppEnvironment-$resourceGroupLocationShort-service"
$webhostingName = "$appPrefix-$environment-$resourceGroupLocationShort-hostingplan"
$actionGroupName = "$appPrefix-$environment-$resourceGroupLocationShort-actionGroup"
$sqlServerName = "$appPrefix-$environment-$resourceGroupLocationShort-sqlserver"
$sqlServerAddress = "$sqlServerName.database.windows.net"
$timing = -join($timing, "2. Variables created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "2. Variables created: "$stopwatch.Elapsed.TotalSeconds

#Web service
az deployment group create --resource-group $resourceGroupName --name $serviceAPIName --template-file "$templatesLocation\WebService.json" --parameters serviceAPIName=$serviceAPIName hostingPlanName=$webhostingName sqlServerName=$sqlServerName sqlServerAddress=$sqlServerAddress sqlDatabaseName=$sqlDatabaseName sqlDatabaseLoginName=$sqlAdministratorLoginUser sqlDatabaseLoginPassword=$sqlAdministratorLoginPassword
#web service managed identity and setting keyvault access permissions
$serviceAPIProdSlotIdentity = az webapp identity assign --resource-group $resourceGroupName --name $serviceAPIName 
$serviceAPIStagingSlotIdentity = az webapp identity assign --resource-group $resourceGroupName --name $serviceAPIName --slot staging
$serviceAPIProdSlotIdentityPrincipalId = ($serviceAPIProdSlotIdentity | ConvertFrom-Json | SELECT PrincipalId).PrincipalId
$serviceAPIStagingSlotIdentityPrincipalId =($serviceAPIStagingSlotIdentity | ConvertFrom-Json | SELECT PrincipalId).PrincipalId
Write-Host "Setting access policies for key vault"
az keyvault set-policy --name $dataKeyVaultName --object-id $serviceAPIProdSlotIdentityPrincipalId --secret-permissions list get
az keyvault set-policy --name $dataKeyVaultName --object-id $serviceAPIStagingSlotIdentityPrincipalId --secret-permissions list get

#Web service alerts
az deployment group create --resource-group $resourceGroupName --name "$($serviceAPIName)Alerts" --template-file "$templatesLocation\WebAppAlerts.json" --parameters webAppName=$serviceAPIName actionGroupName=$actionGroupName 

#Get Redis connection string from key vault
$redisCacheConnectionStringName = "AppSettings--RedisCacheConnectionString$environment"
Write-Host "Getting value redis $redisCacheConnectionStringName connectionString from key vault"
$redisConnectionStringJson = az keyvault secret show --vault-name $dataKeyVaultName --name $redisCacheConnectionStringName
$redisConnectionString = ($redisConnectionStringJson | ConvertFrom-Json).value
#Get SQL connection string from key vault
$samsAppConnectionStringName = "ConnectionStrings--SamsAppConnectionString$environment"
Write-Host "Getting value sql $samsAppConnectionStringName connectionString from key vault"
$samsAppConnectionStringJson = az keyvault secret show --vault-name $dataKeyVaultName --name $samsAppConnectionStringName
$samsAppConnectionString = ($samsAppConnectionStringJson | ConvertFrom-Json).value
#Get application insights from key vault
$applicationInsightsName = "ApplicationInsights--InstrumentationKey$environment"
Write-Host "Getting value application insights $applicationInsightsName secret from key vault"
$applicationInsightsJson = az keyvault secret show --vault-name $dataKeyVaultName --name $applicationInsightsName 
$applicationInsightsKey = ($applicationInsightsJson | ConvertFrom-Json).value
#Set secrets into appsettings 
Write-Host "Setting appsettings $redisCacheConnectionStringName connectionString: $redisConnectionString"
Write-Host "Setting appsettings $applicationInsightsName connectionString: $applicationInsightsKey"
az webapp config appsettings set --resource-group $resourceGroupName --name $serviceAPIName --slot staging --settings "RedisCacheConnectionString=$redisConnectionString" "APPINSIGHTS_INSTRUMENTATIONKEY=$applicationInsightsKey" 
#Set secrets into configuration strings 
Write-Host "Setting appsettings $samsAppConnectionStringName connectionString: $samsAppConnectionString"
az webapp config connection-string set --resource-group $resourceGroupName --name $serviceAPIName --connection-string-type SQLAzure --settings DefaultConnection="$samsAppConnectionString"


$timing = -join($timing, "3. Web service created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "3. Web service created: "$stopwatch.Elapsed.TotalSeconds

Write-Host "servicePrincipalId: "$serviceAPIProdSlotIdentityPrincipalId
Write-Host "serviceStagingSlotPrincipalId: "$serviceAPIStagingSlotIdentityPrincipalId
$timing = -join($timing, "4. All Done: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "4. All Done: "$stopwatch.Elapsed.TotalSeconds
Write-Host "Timing: `n$timing"
Write-Host "Were there errors? (If the next line is blank, then no!) $error"