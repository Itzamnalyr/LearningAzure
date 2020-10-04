#Always start with a login: https://docs.microsoft.com/en-us/cli/azure/authenticate-azure-cli?view=azure-cli-latest
#az login
#And create a resource group
#az group create --location eastus --name SamLearnsAzurePR456  

# Instantiate and start a new stopwatch
$stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
CLS
$error.clear()
$timing = ""
$timing = -join($timing, "1. Deployment started: ", $stopwatch.Elapsed.TotalSeconds, "`n")
Write-Host "1. Deployment started: "$stopwatch.Elapsed.TotalSeconds

$resourceGroupName = "SamLearnsAzurePR456"
$resourceGroupLocation = "eastus"
$appPrefix = "samsapp"
$environment = "pr456"
$locationShort = "eu"

$keyVaultName = "$appPrefix-$environment-$locationShort-vault"
$storageAccountName = "$appPrefix$environment$($locationShort)storage"
$redisCacheName = "$appPrefix-$environment-$locationShort-redis"
$cdnName = "$appPrefix-$environment-$locationShort-cdn"                

$sqlServerName = "$appPrefix-$environment-$locationShort-sqlserver"
$sqlDatabaseName = "samsdb" 
$sqlAdministratorLoginUser = "admin123"
$sqlAdministratorLoginPassword = "ThisIsnotTheR3alpwdItIsJustAnExampl3!" #The password is case-sensitive and must contain lower case, upper case, numbers and special characters. The default Azure password complexity rules: minimum length of 8 characters, minimum of 1 uppercase character, minimum of 1 lowercase character, minimum of 1 number.
$administratorUserLogin = "c6193b13-08e7-4519-b7b4-e6b1875b15a8"
$administratorUserSid = "076f7430-ef4f-44e0-aaa7-d00c0f75b0b8"

$webhostingName = "$appPrefix-$environment-$locationShort-hostingplan"
$actionGroupName = "$appPrefix-$environment-$locationShort-actionGroup"
$serviceAPIName = "$appPrefix-$environment-$locationShort-service"
$webSiteName = "$appPrefix-$environment-$locationShort-web"
$websiteDomainName = "$environment.samlearnsazure.com"

$letsEncryptUniqueRoleAssignmentGuid = '6e4cff57-e63a-403e-822c-e98e5ba0484a'
$letsEncryptAppServiceContributerClientSecret="RSRf?J_z+1t6W*EPpxkVhXTs9Szirku5"

$applicationInsightsName = "$appPrefix-$environment-$locationShort-appinsights"
$applicationInsightsAvailablityTestName = "$applicationInsightsName-availability-home-page-test"

$azureDevOpsPrincipalId = "e60b0582-1d81-4ab3-92db-fbdc53ddeb92"
$contactEmailAddress="samsmithnz@gmail.com"

$frontDoorName = "$appPrefix-$environment-$locationShort-frontdoor"
$frontDoorBackEndAddresses = "['$webSiteName.azurewebsites.net']"  #create an array of strings for each of the back end pool resources

$templatesLocation = "C:\Users\samsmit\source\repos\SamLearnsAzure\SamLearnsAzure\SamLearnsAzure.Environment.ARMTemplate\Templates"

if ($keyVaultName.Length -gt 24)
{
    Write-Host "Key vault name must be 3-24 characters in length"
    Break
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

#Resource group
az group create --name $resourceGroupName --location $resourceGroupLocation
$timing = -join($timing, "3. Resource group created:: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "3. Resource group created: "$stopwatch.Elapsed.TotalSeconds

#key vault
az deployment group create --resource-group $resourceGroupName --name $keyVaultName --template-file "$templatesLocation\KeyVault.json" --parameters keyVaultName=$keyVaultName administratorUserPrincipalId=$administratorUserSid azureDevOpsPrincipalId=$azureDevOpsPrincipalId
if($error)
{
    #purge any existing key vault because of soft delete
    Write-Host "Purging existing keyvault"
    az keyvault purge --name $keyVaultName 
    Write-Host "Creating keyvault, round 2"
    az deployment group create --resource-group $resourceGroupName --name $keyVaultName --template-file "$templatesLocation\KeyVault.json" --parameters keyVaultName=$keyVaultName administratorUserPrincipalId=$administratorUserSid azureDevOpsPrincipalId=$azureDevOpsPrincipalId
    $error.clear()
}
$timing = -join($timing, "4. Key vault created:: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "4. Key vault created: "$stopwatch.Elapsed.TotalSeconds

#storage
$storageOutput = az deployment group create --resource-group $resourceGroupName --name $storageAccountName --template-file "$templatesLocation\Storage.json" --parameters storageAccountName=$storageAccountName
$storageJSON = $storageOutput | ConvertFrom-Json
$storageAccountAccessKey = $storageJSON.properties.outputs.storageAccountKey.value
$storageAccountNameKV = "StorageAccountKey$Environment"
Write-Host "Setting value $storageAccountAccessKey for $storageAccountNameKV to key vault"
az keyvault secret set --vault-name $dataKeyVaultName --name "$storageAccountNameKV" --value $storageAccountAccessKey #Upload the secret into the key vault
$timing = -join($timing, "5. Storage created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "5. Storage created: "$stopwatch.Elapsed.TotalSeconds

#CDN
az deployment group create --resource-group $resourceGroupName --name $cdnName --template-file "$templatesLocation\CDN.json" --parameters cdnName=$cdnName storageAccountName=$storageAccountName
$timing = -join($timing, "6. CDN  created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "6. CDN created: "$stopwatch.Elapsed.TotalSeconds

#Redis  
$redisOutput = az deployment group create --resource-group $resourceGroupName --name $redisCacheName --template-file "$templatesLocation\Redis.json" --parameters redisCacheName=$redisCacheName
$redisJSON = $redisOutput | ConvertFrom-Json
$redisConnectionString = $redisJSON.properties.outputs.redisConnectionStringOutput.value
$redisCacheConnectionStringName = "AppSettings--RedisCacheConnectionString$Environment"
Write-Host "Setting value $redisConnectionString for $redisCacheConnectionStringName to key vault"
az keyvault secret set --vault-name $dataKeyVaultName --name "$redisCacheConnectionStringName" --value $redisConnectionString #Upload the secret into the key vault
$timing = -join($timing, "7. Redis created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "7. Redis created: "$stopwatch.Elapsed.TotalSeconds

#SQL
$sqlOutput = az deployment group create --resource-group $resourceGroupName --name $sqlServerName --template-file "$templatesLocation\SQL.json" --parameters sqlServerName=$sqlServerName databaseName=$sqlDatabaseName sqlAdministratorLogin=$sqlAdministratorLoginUser sqlAdministratorLoginPassword=$sqlAdministratorLoginPassword administratorUserLogin=$administratorUserLogin administratorUserSid=$administratorUserSid storageAccountName=$storageAccountName storageAccountAccessKey=$storageAccountAccessKey
$sqlJSON = $sqlOutput | ConvertFrom-Json
$sqlServerAddress = $sqlJSON.properties.outputs.sqlServerIPAddress.value
$sqlConnectionStringName = "ConnectionStrings--SamsAppConnectionString$Environment"
$sqlConnectionStringValue = "Server=tcp:$sqlServerName.database.windows.net,1433;Initial Catalog=$sqlDatabaseName;Persist Security Info=False;User ID=$sqlAdministratorLoginUser;Password=$sqlAdministratorLoginPassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
Write-Host "Setting value $sqlConnectionStringValue for $sqlConnectionStringName to key vault"
az keyvault secret set --vault-name $dataKeyVaultName --name "$sqlConnectionStringName" --value $sqlConnectionStringValue #Upload the secret into the key vault
$timing = -join($timing, "8. SQL created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "8. SQL created: "$stopwatch.Elapsed.TotalSeconds

#Action Group
az deployment group create --resource-group $resourceGroupName --name $actionGroupName --template-file "$templatesLocation\ActionGroup.json" --parameters actionGroupName=$actionGroupName appPrefix=$appPrefix environment=$environment contactEmailAddress=$contactEmailAddress
$timing = -join($timing, "9. Action group created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "9. Action group created: "$stopwatch.Elapsed.TotalSeconds

#Application Insights
$applicationInsightsOutput = az deployment group create --resource-group $resourceGroupName --name $applicationInsightsName --template-file "$templatesLocation\ApplicationInsights.json" --parameters applicationInsightsName=$applicationInsightsName applicationInsightsAvailablityTestName="$applicationInsightsAvailablityTestName" websiteDomainName=$websiteDomainName 
$applicationInsightsJSON = $applicationInsightsOutput | ConvertFrom-Json
$applicationInsightsInstrumentationKey = $applicationInsightsJSON.properties.outputs.applicationInsightsInstrumentationKeyOutput.value
$applicationInsightsInstrumentationKeyName = "ApplicationInsights--InstrumentationKey$Environment"
Write-Host "Setting value $ApplicationInsightsInstrumentationKey for $applicationInsightsInstrumentationKeyName to key vault"
az keyvault secret set --vault-name $dataKeyVaultName --name "$applicationInsightsInstrumentationKeyName" --value $ApplicationInsightsInstrumentationKey #Upload the secret into the key vault
$timing = -join($timing, "10. Application created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "10. Application insights created: "$stopwatch.Elapsed.TotalSeconds

#Web hosting
az deployment group create --resource-group $resourceGroupName --name $webhostingName --template-file "$templatesLocation\WebHosting.json" --parameters hostingPlanName=$webhostingName actionGroupName=$actionGroupName 
$timing = -join($timing, "11. Web hosting created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "11. Web hosting created: "$stopwatch.Elapsed.TotalSeconds

#Web service
az deployment group create --resource-group $resourceGroupName --name $serviceAPIName --template-file "$templatesLocation\WebService.json" --parameters serviceAPIName=$serviceAPIName hostingPlanName=$webhostingName sqlServerName=$sqlServerName sqlServerAddress=$sqlServerAddress sqlDatabaseName=$sqlDatabaseName sqlDatabaseLoginName=$sqlAdministratorLoginUser sqlDatabaseLoginPassword=$sqlAdministratorLoginPassword
#web service managed identity and setting keyvault access permissions
$serviceAPIProdSlotIdentity = az webapp identity assign --resource-group $resourceGroupName --name $serviceAPIName 
$serviceAPIStagingSlotIdentity = az webapp identity assign --resource-group $resourceGroupName --name $serviceAPIName  --slot staging
$serviceAPIProdSlotIdentityPrincipalId = ($serviceAPIProdSlotIdentity | ConvertFrom-Json | SELECT PrincipalId).PrincipalId
$serviceAPIStagingSlotIdentityPrincipalId =($serviceAPIStagingSlotIdentity | ConvertFrom-Json | SELECT PrincipalId).PrincipalId
Write-Host "Setting access policies for key vault"
az keyvault set-policy --name $keyVaultName --object-id $serviceAPIProdSlotIdentityPrincipalId --secret-permissions list,get
az keyvault set-policy --name $keyVaultName --object-id $serviceAPIStagingSlotIdentityPrincipalId --secret-permissions list,get
#Set-AzKeyVaultAccessPolicy -VaultName "$keyVaultName" -ObjectId "$serviceAPIProdSlotIdentityPrincipalId" -PermissionsToSecrets list,get -PassThru -BypassObjectIdValidation
#Set-AzKeyVaultAccessPolicy -VaultName "$keyVaultName" -ObjectId "$serviceAPIStagingSlotIdentityPrincipalId" -PermissionsToSecrets list,get -PassThru -BypassObjectIdValidation
#Web service alerts
az deployment group create --resource-group $resourceGroupName --name "webServiceAlerts" --template-file "$templatesLocation\WebAppAlerts.json" --parameters webAppName=$serviceAPIName actionGroupName=$actionGroupName 
$timing = -join($timing, "12. Web service created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "12. Web service created: "$stopwatch.Elapsed.TotalSeconds

#Web site
az deployment group create --resource-group $resourceGroupName --name $webSiteName --template-file "$templatesLocation\Website.json" --parameters webSiteName=$webSiteName hostingPlanName=$webhostingName storageAccountName=$storageAccountName websiteDomainName=$websiteDomainName contactEmailAddress=$contactEmailAddress letsEncryptAppServiceContributerClientSecret="$letsEncryptAppServiceContributerClientSecret"
#web site managed identity and setting keyvault access permissions
$websiteProdSlotIdentity = az webapp identity assign --resource-group $resourceGroupName --name $webSiteName 
$websiteStagingSlotIdentity = az webapp identity assign --resource-group $resourceGroupName --name $webSiteName  --slot staging
$websiteProdSlotIdentityPrincipalId = ($websiteProdSlotIdentity | ConvertFrom-Json | SELECT PrincipalId).PrincipalId
$websiteStagingSlotIdentityPrincipalId =($websiteStagingSlotIdentity | ConvertFrom-Json | SELECT PrincipalId).PrincipalId
Write-Host "prod: " $websiteProdSlotIdentityPrincipalId
Write-Host "staging: " $websiteStagingSlotIdentityPrincipalId
Write-Host "Setting access policies for key vault"
az keyvault set-policy --name $keyVaultName --object-id $websiteProdSlotIdentityPrincipalId --secret-permissions list,get
az keyvault set-policy --name $keyVaultName --object-id $websiteStagingSlotIdentityPrincipalId --secret-permissions list,get
#Set-AzKeyVaultAccessPolicy -VaultName "$keyVaultName" -ObjectId "$websiteProdSlotIdentityPrincipalId" -PermissionsToSecrets list,get -PassThru -BypassObjectIdValidation
#Set-AzKeyVaultAccessPolicy -VaultName "$keyVaultName" -ObjectId "$websiteStagingSlotIdentityPrincipalId" -PermissionsToSecrets list,get -PassThru -BypassObjectIdValidation
#Website alerts
az deployment group create --resource-group $resourceGroupName --name "webSiteAlerts" --template-file "$templatesLocation\WebAppAlerts.json" --parameters webAppName=$webSiteName actionGroupName=$actionGroupName 
$timing = -join($timing, "13. Website created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "13. Website created: "$stopwatch.Elapsed.TotalSeconds


#Frontdoor
az deployment group create --resource-group $resourceGroupName --name $frontDoorName --template-file "$templatesLocation\FrontDoor.json" --parameters frontDoorName=$frontDoorName frontDoorBackEndAddresses=$frontDoorBackEndAddresses 
$timing = -join($timing, "14. Frontdoor created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "14. Frontdoor created: "$stopwatch.Elapsed.TotalSeconds


Write-Host "storageAccountAccessKey: "$storageAccountAccessKey
Write-Host "sqlServerIPAddress: "$sqlServerIPAddress
Write-Host "servicePrincipalId: "$serviceAPIProdSlotIdentityPrincipalId
Write-Host "serviceStagingSlotPrincipalId: "$serviceAPIStagingSlotIdentityPrincipalId
Write-Host "websitePrincipalId: "$websiteProdSlotIdentityPrincipalId
Write-Host "websiteStagingSlotPrincipalId: "$websiteStagingSlotIdentityPrincipalId
Write-Host "applicationInsightsInstrumentationKey: "$applicationInsightsInstrumentationKey
$timing = -join($timing, "15. All Done: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "15. All Done: "$stopwatch.Elapsed.TotalSeconds
Write-Host "Timing: `n$timing"
Write-Host "Were there errors? (If the next line is blank, then no!) $error"
                