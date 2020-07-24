$resourceGroupName = "network-eastus2-rg"
$resourceGroupLocation = "East US"
$appPrefix = "samsapp"
$environment = "pre"
$locationShort = "eu"


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

az deployment group create --resource-group $resourceGroupName --name samsapparmeustorage --template-file templates/AzureStorage.json --parameters storageAccountName="$storageAccountName" resourceGroupLocation="$resourceGroupLocation"

az deployment group create --resource-group $resourceGroupName --name firewall-eastus2 --template-file templates/azuredeploy.json --parameters  networkResourceGroup=network-eastus2-rg vnetName=hub-vnet subnetName=AzureFirewallSubnet


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