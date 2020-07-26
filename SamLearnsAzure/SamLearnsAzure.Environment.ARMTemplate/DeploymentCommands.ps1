$resourceGroupName = "network-eastus2-rg"
$resourceGroupLocation = "East US"
$appPrefix = "samsapp"
$environment = "pre"
$locationShort = "eu"


$keyVaultName = "$appPrefix-$environment-$locationShort-keyvault"
$serviceAPIName = "$appPrefix-$environment-$locationShort-service"
$webSiteName = "$appPrefix-$environment-$locationShort-web"
$sqlserverName = "$appPrefix-$environment-$locationShort-sqlserver"
$hostingPlanName = "$appPrefix-$environment-$locationShort-hostingplan"
$storageAccountName = "$appPrefix$environment$($locationShort)storage"
$actionGroupName = "$appPrefix-$environment-$locationShort-actionGroup"
$actionGroupShortName = "$environment-actgrp"
$applicationInsightsName = "$appPrefix-$environment-$locationShort-appinsights"
$applicationInsightsAvailablityTestName = "Availability home page test-$applicationInsightsName"
$redisCacheName = "$appPrefix-$environment-$locationShort-redis"
$cdnName = "$appPrefix-$environment-$locationShort-cdn"
write-host $serviceAPIName
write-host $storageAccountName
write-host $actionGroupName
write-host $applicationInsightsAvailablityTestName

az deployment group create --resource-group $resourceGroupName --name $storageAccountName --template-file templates/AzureStorage.json --parameters storageAccountName="$storageAccountName" 

az deployment group create --resource-group $resourceGroupName --name $keyVaultName --template-file templates/AzureKeyVault.json --parameters keyVaultName=$keyVaultName 


#Order
#1. Storage
#2. Key Vault
#3a. SQL
#3b. web hosting
#4b. web app must be after web hosting
#5a Redis must be after SQL and web app
#5b CDN must be after storage
#5c Application Insights must be after web app
#5d Front door must be after web apps