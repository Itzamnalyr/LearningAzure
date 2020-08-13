#Always start with a login: https://docs.microsoft.com/en-us/cli/azure/authenticate-azure-cli?view=azure-cli-latest
#az login
#And create a resource group
#az group create --location eastus --name SamLearnsAzurePR456  

# Instantiate and start a new stopwatch
$stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
CLS
Write-Host "1. Deployment started: "$stopwatch.Elapsed.TotalSeconds

$resourceGroupName = "SamLearnsAzurePR456"
$resourceGroupLocation = "East US"
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

$webhostingName = "$appPrefix-$environment-$locationShort-hostingplan"
$actionGroupName = "$appPrefix-$environment-$locationShort-actionGroup"
$serviceAPIName = "$appPrefix-$environment-$locationShort-service"
$webSiteName = "$appPrefix-$environment-$locationShort-web"
$websiteDomainName = "$environment.samlearnsazure.com"

$letsEncryptAppServiceContributerClientSecret="RSRf?J_z+1t6W*EPpxkVhXTs9Szirku5"

$applicationInsightsName = "$appPrefix-$environment-$locationShort-appinsights"
$applicationInsightsAvailablityTestName = "$applicationInsightsName-availability-home-page-test"

$administratorUserLogin = "c6193b13-08e7-4519-b7b4-e6b1875b15a8"
$administratorUserSid = "076f7430-ef4f-44e0-aaa7-d00c0f75b0b8"
$azureDevOpsPrincipalId = "e60b0582-1d81-4ab3-92db-fbdc53ddeb92"
$contactEmailAddress="samsmithnz@gmail.com"

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
Write-Host "2. Variables created: "$stopwatch.Elapsed.TotalSeconds

#Resource group
az group create --name SamLearnsAzurePR456 --location eastus
Write-Host "3. Resource group created: "$stopwatch.Elapsed.TotalSeconds

#key vault
az deployment group create --resource-group $resourceGroupName --name $keyVaultName --template-file "$templatesLocation\KeyVault.json" --parameters keyVaultName=$keyVaultName administratorUserPrincipalId=$administratorUserSid azureDevOpsPrincipalId=$azureDevOpsPrincipalId
Write-Host "4. Key vault created: "$stopwatch.Elapsed.TotalSeconds

#storage
$storageOutput = az deployment group create --resource-group $resourceGroupName --name $storageAccountName --template-file "$templatesLocation\Storage.json" --parameters storageAccountName=$storageAccountName
$storageJSON = $storageOutput | ConvertFrom-Json
$storageAccountAccessKey = $storageJSON.properties.outputs.storageAccountKey.value
az keyvault secret set --vault-name $keyVaultName --name "storageAccountAccessKey" --value $storageAccountAccessKey #Upload the secret into the key vault
Write-Host "5. Storage created: "$stopwatch.Elapsed.TotalSeconds

#CDN
#az deployment group create --resource-group $resourceGroupName --name $cdnName --template-file "$templatesLocation\CDN.json" --parameters cdnName=$cdnName storageAccountName=$storageAccountName
Write-Host "6. CDN created: "$stopwatch.Elapsed.TotalSeconds

#Redis  
#$redisOutput = az deployment group create --resource-group $resourceGroupName --name $redisCacheName --template-file "$templatesLocation\Redis.json" --parameters redisCacheName=$redisCacheName
#$redisJSON = $redisOutput | ConvertFrom-Json
#$redisConnectionString = $redisJSON.properties.outputs.redisConnectionStringOutput.value
Write-Host "7. Redis created: "$stopwatch.Elapsed.TotalSeconds

#SQL
#$sqlOutput = az deployment group create --resource-group $resourceGroupName --name $sqlServerName --template-file "$templatesLocation\SQL.json" --parameters sqlServerName=$sqlServerName databaseName=sqlDatabaseName sqlAdministratorLogin=$sqlAdministratorLoginUser sqlAdministratorLoginPassword=$databaseLoginPassword administratorUserLogin=$sqlAdministratorLoginPassword administratorUserSid=$administratorUserSid storageAccountName=$storageAccountName storageAccountAccessKey=$storageAccountAccessKey
#$sqlJSON = $sqlOutput | ConvertFrom-Json
#$sqlServerAddress = $sqlJSON.properties.outputs.sqlServerIPAddress.value
Write-Host "8. SQL created: "$stopwatch.Elapsed.TotalSeconds

#Action Group
az deployment group create --resource-group $resourceGroupName --name $actionGroupName --template-file "$templatesLocation\ActionGroup.json" --parameters actionGroupName=$actionGroupName appPrefix=$appPrefix environment=$environment contactEmailAddress=$contactEmailAddress
Write-Host "9. Action group created: "$stopwatch.Elapsed.TotalSeconds

#Application Insights
$applicationInsightsOutput = az deployment group create --resource-group $resourceGroupName --name $applicationInsightsName --template-file "$templatesLocation\ApplicationInsights.json" --parameters applicationInsightsName=$applicationInsightsName applicationInsightsAvailablityTestName=$applicationInsightsAvailablityTestName websiteDomainName=$websiteDomainName 
$applicationInsightsJSON = $applicationInsightsOutput | ConvertFrom-Json
$applicationInsightsInstrumentationKey = $applicationInsightsJSON.properties.outputs.applicationInsightsInstrumentationKeyOutput.value
Write-Host "10. Application insights created: "$stopwatch.Elapsed.TotalSeconds

#Web hosting
az deployment group create --resource-group $resourceGroupName --name $webhostingName --template-file "$templatesLocation\WebHosting.json" --parameters hostingPlanName=$webhostingName actionGroupName=$actionGroupName 
Write-Host "11. Web hosting created: "$stopwatch.Elapsed.TotalSeconds

#Web service
#$webServiceOutput = az deployment group create --resource-group $resourceGroupName --name $serviceAPIName --template-file "$templatesLocation\WebService.json" --parameters serviceAPIName=$serviceAPIName hostingPlanName=$webhostingName actionGroupName=$actionGroupName sqlServerName=$sqlServerName sqlServerAddress=$sqlServerAddress sqlDatabaseName=$sqlDatabaseName sqlDatabaseLoginName=$sqlAdministratorLoginUser sqlDatabaseLoginPassword=$sqlAdministratorLoginPassword
#$webServiceJSON = $webServiceOutput | ConvertFrom-Json
#$servicePrincipalId = $webServiceJSON.properties.outputs.servicePrincipalId.value
#$serviceStagingSlotPrincipalId = $webServiceJSON.properties.outputs.serviceStagingSlotPrincipalId.value
#Web service alerts
#az deployment group create --resource-group $resourceGroupName --name "serviceAlerts" --template-file "$templatesLocation\WebAppAlerts.json" --parameters webAppName=$serviceAPIName actionGroupName=$actionGroupName 
#Write-Host "12. Web service created: "$stopwatch.Elapsed.TotalSeconds

#Web site
$webAppOutput = az deployment group create --resource-group $resourceGroupName --name $webSiteName --template-file "$templatesLocation\Website.json" --parameters webSiteName=$webSiteName hostingPlanName=$webhostingName actionGroupName=$actionGroupName storageAccountName=$storageAccountName websiteDomainName=$websiteDomainName contactEmailAddress=$contactEmailAddress letsEncryptAppServiceContributerClientSecret="$letsEncryptAppServiceContributerClientSecret"
$webAppJSON = $webAppOutput | ConvertFrom-Json
$websitePrincipalId = $webAppJSON.properties.outputs.websitePrincipalId.value
$websiteStagingSlotPrincipalId = $webAppJSON.properties.outputs.websiteStagingSlotPrincipalId.value
#Website alerts
az deployment group create --resource-group $resourceGroupName --name "webSiteAlerts" --template-file "$templatesLocation\WebAppAlerts.json" --parameters webAppName=$webSiteName actionGroupName=$actionGroupName 
Write-Host "13. Website created: "$stopwatch.Elapsed.TotalSeconds


Write-Host "storageAccountAccessKey: "$storageAccountAccessKey
Write-Host "sqlServerIPAddress: "$sqlServerIPAddress
Write-Host "servicePrincipalId: "$servicePrincipalId
Write-Host "serviceStagingSlotPrincipalId: "$serviceStagingSlotPrincipalId
Write-Host "websitePrincipalId: "$websitePrincipalId
Write-Host "websiteStagingSlotPrincipalId: "$websiteStagingSlotPrincipalId
Write-Host "applicationInsightsInstrumentationKey: "$applicationInsightsInstrumentationKey
Write-Host "14. All Done: "$stopwatch.Elapsed.TotalSeconds

                
#Deployment Order
#1. Key Vault
#2. Storage must be after key vault, as it stores the storage access key in the key vault
#3a CDN must be after storage, as it connects to the storage for the CDN cache
#3b Redis must be after key vault, as it stores the redis connection string in the key vault 

#4a. Azure SQL service & database (lumped together, as we just have one database)
#4b. web hosting
#5a. web app must be after web hosting
#6 Application Insights must be after web app

#7 Front door must be after web apps