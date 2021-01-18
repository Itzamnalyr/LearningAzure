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

$resourceGroupName = "SamLearnsAzureTest"
$resourceGroupLocation = "eastus"
$appPrefix = "samsapp"
$environment = "test"
$locationShort = "eu"

$keyVaultName = "$appPrefix-$environment-$locationShort-vault"
$dataKeyVaultName = $keyVaultName 
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

$applicationInsightsName = "$appPrefix-$environment-$locationShort-appinsights"
$applicationInsightsAvailablityTestName = "$applicationInsightsName-availability-home-page-test"

$azureDevOpsPrincipalId = "e60b0582-1d81-4ab3-92db-fbdc53ddeb92"
$contactEmailAddress="samsmithnz@gmail.com"

$frontDoorName = "$appPrefix-$environment-$locationShort-frontdoor"
$frontDoorBackEndAddresses = "['$webSiteName.azurewebsites.net']"  #create an array of strings for each of the back end pool resources
$frontDoorDomainName = "$($environment)fd.samlearnsazure.com"

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
$CheckWhatIfs = $true
$timing = -join($timing, "2. Variables created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "2. Variables created: "$stopwatch.Elapsed.TotalSeconds

#Resource group
$ResourceGroupExists = az group exists --name $resourceGroupName
if ($ResourceGroupExists -eq $false)
{
    Write-Host "Creating Resource Group $resourceGroupName"
    az group create --name $resourceGroupName --location $resourceGroupLocation
    $CheckWhatIfs = $true #Ignore the what ifs = run every create to save time on initial creation   
}
$timing = -join($timing, "3. Resource group created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "3. Resource group created: "$stopwatch.Elapsed.TotalSeconds


#key vault
if ($CheckWhatIfs -eq $true)
{
    $whatifResultsJson = az deployment group what-if --no-pretty-print --only-show-errors --resource-group $resourceGroupName --name $keyVaultName --template-file "$templatesLocation\KeyVault.json" --parameters keyVaultName=$keyVaultName administratorUserPrincipalId=$administratorUserSid azureDevOpsPrincipalId=$azureDevOpsPrincipalId
    $whatifResults = $whatifResultsJson | ConvertFrom-Json 
    $ChangeResults4 = $whatifResults.changes 

    #Filter out access policies from the result - as this is really data, not infrastructure (in my view)
    $ChangeResults4 = $ChangeResults4 | Where-Object { $_.delta.path -ne "properties.accessPolicies" }

    $ChangeResults4b = $ChangeResults4 | Where-Object { $_.changeType -eq "Modify" }
    $ChangeResults4b.delta.path
}
if ($CheckWhatIfs -eq $false -or $ChangeResults4.changeType -eq "Create" -or $ChangeResults4.changeType -eq "Modify")
{
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
}
else
{
    Write-Host "4. Key vault CheckWhatIf: $CheckWhatIfs and change type: $($ChangeResults4.changeType) results"
}
$timing = -join($timing, "4. Key vault created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "4. Key vault created: "$stopwatch.Elapsed.TotalSeconds


#storage
if ($CheckWhatIfs -eq $true)
{
    $whatifResultsJson = az deployment group what-if --no-pretty-print --only-show-errors --resource-group $resourceGroupName --name $storageAccountName --template-file "$templatesLocation\Storage.json" --parameters storageAccountName=$storageAccountName
    $whatifResults = $whatifResultsJson | ConvertFrom-Json 
    $ChangeResults5 = $whatifResults.changes 
}
if ($CheckWhatIfs -eq $false -or $ChangeResults5.changeType -eq "Create" -or $ChangeResults5.changeType -eq "Modify")
{
    $storageOutput = az deployment group create --resource-group $resourceGroupName --name $storageAccountName --template-file "$templatesLocation\Storage.json" --parameters storageAccountName=$storageAccountName
    $storageJSON = $storageOutput | ConvertFrom-Json
    $storageAccountAccessKey = $storageJSON.properties.outputs.storageAccountKey.value
    $storageAccountNameKV = "StorageAccountKey$Environment"
    Write-Host "Setting value $storageAccountAccessKey for $storageAccountNameKV to key vault"
    az keyvault secret set --vault-name $dataKeyVaultName --name "$storageAccountNameKV" --value $storageAccountAccessKey #Upload the secret into the key vault
}
else
{
    Write-Host "5. Storage CheckWhatIf: $CheckWhatIfs and change type: $($ChangeResults5.changeType) results"
}
$timing = -join($timing, "5. Storage created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "5. Storage created: "$stopwatch.Elapsed.TotalSeconds


#CDN
if ($CheckWhatIfs -eq $true)
{
    $whatifResultsJson = az deployment group what-if --no-pretty-print --only-show-errors --resource-group $resourceGroupName --name $cdnName --template-file "$templatesLocation\CDN.json" --parameters cdnName=$cdnName storageAccountName=$storageAccountName
    $whatifResults = $whatifResultsJson | ConvertFrom-Json 
    $ChangeResults6 = $whatifResults.changes 

    #$ChangeResults6b = $ChangeResults6 | Where-Object { $_.changeType -eq "Modify" }
    #$ChangeResults6b.delta.path
}
if ($CheckWhatIfs -eq $false -or $ChangeResults6.changeType -eq "Create" -or $ChangeResults6.changeType -eq "Modify")
{
    az deployment group create --resource-group $resourceGroupName --name $cdnName --template-file "$templatesLocation\CDN.json" --parameters cdnName=$cdnName storageAccountName=$storageAccountName
}
else
{
    Write-Host "6. CDN CheckWhatIf: $CheckWhatIfs and change type: $($ChangeResults6.changeType) results"
}
$timing = -join($timing, "6. CDN  created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "6. CDN created: "$stopwatch.Elapsed.TotalSeconds

#Redis  
if ($CheckWhatIfs -eq $true)
{
    $whatifResultsJson = az deployment group what-if --no-pretty-print --only-show-errors --resource-group $resourceGroupName --name $redisCacheName --template-file "$templatesLocation\Redis.json" --parameters redisCacheName=$redisCacheName
    $whatifResults = $whatifResultsJson | ConvertFrom-Json 
    $ChangeResults7 = $whatifResults.changes 

    #Filter out REDIS configuration, which can't be set on the Basic Redis SKU, but what-if returns as a problem
    $ChangeResults7 = $ChangeResults7 | Where-Object { $_.delta.path -ne "properties.redisConfiguration" }

    #$ChangeResults7b = $ChangeResults7 | Where-Object { $_.changeType -eq "Modify" }
    #$ChangeResults7b.delta
}
if ($CheckWhatIfs -eq $false -or $ChangeResults7.changeType -eq "Create" -or $ChangeResults7.changeType -eq "Modify")
{
    $redisOutput = az deployment group create --resource-group $resourceGroupName --name $redisCacheName --template-file "$templatesLocation\Redis.json" --parameters redisCacheName=$redisCacheName
    $redisJSON = $redisOutput | ConvertFrom-Json
    $redisConnectionString = $redisJSON.properties.outputs.redisConnectionStringOutput.value
    $redisCacheConnectionStringName = "AppSettings--RedisCacheConnectionString$Environment"
    Write-Host "Setting value $redisConnectionString for $redisCacheConnectionStringName to key vault"
    az keyvault secret set --vault-name $dataKeyVaultName --name "$redisCacheConnectionStringName" --value $redisConnectionString #Upload the secret into the key vault
}
else
{
    Write-Host "7. Redis CheckWhatIf: $CheckWhatIfs and change type: $($ChangeResults7.changeType) results"
}
$timing = -join($timing, "7. Redis created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "7. Redis created: "$stopwatch.Elapsed.TotalSeconds

#SQL
if ($CheckWhatIfs -eq $true)
{
    $whatifResultsJson = az deployment group what-if --no-pretty-print --only-show-errors --resource-group $resourceGroupName --name $sqlServerName --template-file "$templatesLocation\SQL.json" --parameters sqlServerName=$sqlServerName databaseName=$sqlDatabaseName sqlAdministratorLogin=$sqlAdministratorLoginUser sqlAdministratorLoginPassword=$sqlAdministratorLoginPassword administratorUserLogin=$administratorUserLogin administratorUserSid=$administratorUserSid storageAccountName=$storageAccountName storageAccountAccessKey=$storageAccountAccessKey
    $whatifResults = $whatifResultsJson | ConvertFrom-Json 
    $ChangeResults8 = $whatifResults.changes 

    #$ChangeResults8b = $ChangeResults8 | Where-Object { $_.changeType -eq "Modify" }
    #$ChangeResults8b.delta
}
if ($CheckWhatIfs -eq $false -or $ChangeResults8.changeType -eq "Create" -or $ChangeResults8.changeType -eq "Modify")
{
    $sqlOutput = az deployment group create --resource-group $resourceGroupName --name $sqlServerName --template-file "$templatesLocation\SQL.json" --parameters sqlServerName=$sqlServerName databaseName=$sqlDatabaseName sqlAdministratorLogin=$sqlAdministratorLoginUser sqlAdministratorLoginPassword=$sqlAdministratorLoginPassword administratorUserLogin=$administratorUserLogin administratorUserSid=$administratorUserSid storageAccountName=$storageAccountName storageAccountAccessKey=$storageAccountAccessKey
    $sqlJSON = $sqlOutput | ConvertFrom-Json
    $sqlServerAddress = $sqlJSON.properties.outputs.sqlServerIPAddress.value
    $sqlConnectionStringName = "ConnectionStrings--SamsAppConnectionString$Environment"
    $sqlConnectionStringValue = "Server=tcp:$sqlServerName.database.windows.net,1433;Initial Catalog=$sqlDatabaseName;Persist Security Info=False;User ID=$sqlAdministratorLoginUser;Password=$sqlAdministratorLoginPassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
    Write-Host "Setting value $sqlConnectionStringValue for $sqlConnectionStringName to key vault"
    az keyvault secret set --vault-name $dataKeyVaultName --name "$sqlConnectionStringName" --value $sqlConnectionStringValue #Upload the secret into the key vault
}
else
{
    Write-Host "8. SQL created CheckWhatIf: $CheckWhatIfs and change type: $($ChangeResults8.changeType) results"
}
$timing = -join($timing, "8. SQL created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "8. SQL created: "$stopwatch.Elapsed.TotalSeconds

#Action Group
if ($CheckWhatIfs -eq $true)
{
    $whatifResultsJson = az deployment group what-if --no-pretty-print --only-show-errors --resource-group $resourceGroupName --name $actionGroupName --template-file "$templatesLocation\ActionGroup.json" --parameters actionGroupName=$actionGroupName appPrefix=$appPrefix environment=$environment contactEmailAddress=$contactEmailAddress
    $whatifResults = $whatifResultsJson | ConvertFrom-Json 
    $ChangeResults9 = $whatifResults.changes 
}
if ($CheckWhatIfs -eq $false -or $ChangeResults9.changeType -eq "Create" -or $ChangeResults9.changeType -eq "Modify")
{
    az deployment group create --resource-group $resourceGroupName --name $actionGroupName --template-file "$templatesLocation\ActionGroup.json" --parameters actionGroupName=$actionGroupName appPrefix=$appPrefix environment=$environment contactEmailAddress=$contactEmailAddress
}
else
{
    Write-Host "9. Action group CheckWhatIf: $CheckWhatIfs and change type: $($ChangeResults9.changeType) results"
}
$timing = -join($timing, "9. Action group created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "9. Action group created: "$stopwatch.Elapsed.TotalSeconds

#Application Insights
if ($CheckWhatIfs -eq $true)
{
    $whatifResultsJson = az deployment group what-if --no-pretty-print --only-show-errors --resource-group $resourceGroupName --name $applicationInsightsName --template-file "$templatesLocation\ApplicationInsights.json" --parameters applicationInsightsName=$applicationInsightsName applicationInsightsAvailablityTestName="$applicationInsightsAvailablityTestName" websiteDomainName=$websiteDomainName 
    $whatifResults = $whatifResultsJson | ConvertFrom-Json 
    $ChangeResults10 = $whatifResults.changes 

    #$ChangeResults10b = $ChangeResults10 | Where-Object { $_.changeType -eq "Modify" }
    #$ChangeResults10b.delta
}
if ($CheckWhatIfs -eq $false -or $ChangeResults10.changeType -eq "Create" -or $ChangeResults10.changeType -eq "Modify")
{
    $applicationInsightsOutput = az deployment group create --resource-group $resourceGroupName --name $applicationInsightsName --template-file "$templatesLocation\ApplicationInsights.json" --parameters applicationInsightsName=$applicationInsightsName applicationInsightsAvailablityTestName="$applicationInsightsAvailablityTestName" websiteDomainName=$websiteDomainName 
    $applicationInsightsJSON = $applicationInsightsOutput | ConvertFrom-Json
    $applicationInsightsInstrumentationKey = $applicationInsightsJSON.properties.outputs.applicationInsightsInstrumentationKeyOutput.value
    $applicationInsightsInstrumentationKeyName = "ApplicationInsights--InstrumentationKey$Environment"
    Write-Host "Setting value $ApplicationInsightsInstrumentationKey for $applicationInsightsInstrumentationKeyName to key vault"
    az keyvault secret set --vault-name $dataKeyVaultName --name "$applicationInsightsInstrumentationKeyName" --value $ApplicationInsightsInstrumentationKey #Upload the secret into the key vault
}
else
{
    Write-Host "10. Application insights CheckWhatIf: $CheckWhatIfs and change type: $($ChangeResults10.changeType) results"
}
$timing = -join($timing, "10. Application insights created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "10. Application insights created: "$stopwatch.Elapsed.TotalSeconds

#Web hosting
if ($CheckWhatIfs -eq $true)
{
    $whatifResultsJson = az deployment group what-if --no-pretty-print --only-show-errors --resource-group $resourceGroupName --name $webhostingName --template-file "$templatesLocation\WebHosting.json" --parameters hostingPlanName=$webhostingName actionGroupName=$actionGroupName 
    $whatifResults = $whatifResultsJson | ConvertFrom-Json 
    $ChangeResults11 = $whatifResults.changes 

    #Filter out metric alert
    for ($i=0; $i -le $ChangeResults11.Count; $i++) 
    {
        if ($ChangeResults11[$i].changeType -eq "Modify")
        {
            #$ChangeResults11[$i].delta.path | Where-Object { $_.path –eq "properties.targetResourceType" }
            $ChangeResults11[$i].delta = ($ChangeResults11[$i].delta | Where-Object { $_.path –ne "properties.targetResourceType" })
            $ChangeResults11[$i].delta = ($ChangeResults11[$i].delta | Where-Object { $_.path –ne "properties.criteria.allOf" })
            $ChangeResults11[$i].delta = ($ChangeResults11[$i].delta | Where-Object { $_.path –ne "properties.profiles" })
            if ($ChangeResults11[$i].delta.Count -eq 0)
            {
                $ChangeResults11[$i].changeType = "Ignore"    
            }
        }       
    }
    
    #$ChangeResults11b = $ChangeResults11 | Where-Object { $_.changeType -eq "Modify" }
    #$ChangeResults11b.delta

}
if ($CheckWhatIfs -eq $false -or $ChangeResults11.changeType -eq "Create" -or $ChangeResults11.changeType -eq "Modify")
{
    az deployment group create --resource-group $resourceGroupName --name $webhostingName --template-file "$templatesLocation\WebHosting.json" --parameters hostingPlanName=$webhostingName actionGroupName=$actionGroupName 
}
else
{
    Write-Host "11. Web hosting CheckWhatIf: $CheckWhatIfs and change type: $($ChangeResults11.changeType) results"
}
$timing = -join($timing, "11. Web hosting created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "11. Web hosting created: "$stopwatch.Elapsed.TotalSeconds


#Web service
if ($CheckWhatIfs -eq $true)
{
    $whatifResultsJson = az deployment group what-if --no-pretty-print --only-show-errors --resource-group $resourceGroupName --name $serviceAPIName --template-file "$templatesLocation\WebService.json" --parameters serviceAPIName=$serviceAPIName hostingPlanName=$webhostingName sqlServerName=$sqlServerName sqlServerAddress=$sqlServerAddress sqlDatabaseName=$sqlDatabaseName sqlDatabaseLoginName=$sqlAdministratorLoginUser sqlDatabaseLoginPassword=$sqlAdministratorLoginPassword
    $whatifResults = $whatifResultsJson | ConvertFrom-Json 
    $ChangeResults12 = $whatifResults.changes 

    #Filter out identity and properties
    for ($i=0; $i -le $ChangeResults12.Count; $i++) 
    {
        if ($ChangeResults12[$i].changeType -eq "Modify")
        {
            $ChangeResults12[$i].delta = $ChangeResults12[$i].delta | Where-Object { $_.path –notlike "properties.*" }
            $ChangeResults12[$i].delta = ($ChangeResults12[$i].delta | Where-Object { $_.path –ne "identity" })
            if ($ChangeResults12[$i].delta.Count -eq 0)
            {
                $ChangeResults12[$i].changeType = "Ignore"    
            }
        }       
    }
    
    #$ChangeResults12b = $ChangeResults12 | Where-Object { $_.changeType -eq "Modify" }
    #$ChangeResults12b.delta

}
if ($CheckWhatIfs -eq $false -or $ChangeResults12.changeType -eq "Create" -or $ChangeResults12.changeType -eq "Modify")
{
    az deployment group create --resource-group $resourceGroupName --name $serviceAPIName --template-file "$templatesLocation\WebService.json" --parameters serviceAPIName=$serviceAPIName hostingPlanName=$webhostingName sqlServerName=$sqlServerName sqlServerAddress=$sqlServerAddress sqlDatabaseName=$sqlDatabaseName sqlDatabaseLoginName=$sqlAdministratorLoginUser sqlDatabaseLoginPassword=$sqlAdministratorLoginPassword
    #web service managed identity and setting keyvault access permissions
    $serviceAPIProdSlotIdentity = az webapp identity assign --resource-group $resourceGroupName --name $serviceAPIName 
    $serviceAPIStagingSlotIdentity = az webapp identity assign --resource-group $resourceGroupName --name $serviceAPIName  --slot staging
    $serviceAPIProdSlotIdentityPrincipalId = ($serviceAPIProdSlotIdentity | ConvertFrom-Json | SELECT PrincipalId).PrincipalId
    $serviceAPIStagingSlotIdentityPrincipalId = ($serviceAPIStagingSlotIdentity | ConvertFrom-Json | SELECT PrincipalId).PrincipalId
    Write-Host "Setting access policies for key vault"
    $policy1 = az keyvault set-policy --name $keyVaultName --object-id $serviceAPIProdSlotIdentityPrincipalId --secret-permissions list get
    $policy2 = az keyvault set-policy --name $keyVaultName --object-id $serviceAPIStagingSlotIdentityPrincipalId --secret-permissions list get
}
else
{
    Write-Host "12. Web service CheckWhatIf: $CheckWhatIfs and change type: $($ChangeResults12.changeType) results"
}
$timing = -join($timing, "12. Web service created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "12. Web service created: "$stopwatch.Elapsed.TotalSeconds

#Web service alerts
if ($CheckWhatIfs -eq $true)
{
    $whatifResultsJson = az deployment group what-if --no-pretty-print --only-show-errors --resource-group $resourceGroupName --name "webSiteAlerts" --template-file "$templatesLocation\WebAppAlerts.json" --parameters webAppName=$serviceAPIName actionGroupName=$actionGroupName 
    $whatifResults = $whatifResultsJson | ConvertFrom-Json 
    $ChangeResults12a = $whatifResults.changes 

    #Filter out properties
    for ($i=0; $i -le $ChangeResults12a.Count; $i++) 
    {
        if ($ChangeResults12a[$i].changeType -eq "Modify")
        {
            $ChangeResults12a[$i].delta = $ChangeResults12a[$i].delta | Where-Object { $_.path –notlike "properties.*" }
            if ($ChangeResults12a[$i].delta.Count -eq 0)
            {
                $ChangeResults12a[$i].changeType = "Ignore"    
            }
        }       
    }

    #$ChangeResults12ab = $ChangeResults12a | Where-Object { $_.changeType -eq "Modify" }
    #$ChangeResults12ab.delta
}
if ($CheckWhatIfs -eq $false -or $ChangeResults12a.changeType -eq "Create" -or $ChangeResults12a.changeType -eq "Modify")
{
    az deployment group create --resource-group $resourceGroupName --name "webServiceAlerts" --template-file "$templatesLocation\WebAppAlerts.json" --parameters webAppName=$serviceAPIName actionGroupName=$actionGroupName 
}
else
{
    Write-Host "12a. webservice Alerts CheckWhatIf: $CheckWhatIfs and change type: $($ChangeResults12a.changeType) results"
}
$timing = -join($timing, "12a. webservice Alerts created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "12a. webservice Alerts created: "$stopwatch.Elapsed.TotalSeconds

#Web site
if ($CheckWhatIfs -eq $true)
{
    $whatifResultsJson = az deployment group what-if --no-pretty-print --only-show-errors --resource-group $resourceGroupName --name $webSiteName --template-file "$templatesLocation\Website.json" --parameters webSiteName=$webSiteName hostingPlanName=$webhostingName storageAccountName=$storageAccountName websiteDomainName=$websiteDomainName 
    $whatifResults = $whatifResultsJson | ConvertFrom-Json 
    $ChangeResults13 = $whatifResults.changes 

    #Filter out identity and properties
    for ($i=0; $i -le $ChangeResults13.Count; $i++) 
    {
        if ($ChangeResults13[$i].changeType -eq "Modify")
        {
            $ChangeResults13[$i].delta = $ChangeResults13[$i].delta | Where-Object { $_.path –notlike "properties*" }
            $ChangeResults13[$i].delta = ($ChangeResults13[$i].delta | Where-Object { $_.path –ne "identity" })
            $ChangeResults13[$i].delta = ($ChangeResults13[$i].delta | Where-Object { $_.path –ne "location" })
            if ($ChangeResults13[$i].delta.Count -eq 0)
            {
                $ChangeResults13[$i].changeType = "Ignore"    
            }
        }
        if ($ChangeResults13[$i].changeType -eq "Create")  
        {
            if ($ChangeResults13[$i].resourceId -like "*roleAssignments/6e4cff57-e63a-403e-822c-e98e5ba02145*")
            {
                $ChangeResults13[$i].changeType = "Ignore"  
            }
        }  
    }
    
    #$ChangeResults13b = $ChangeResults13 | Where-Object { $_.changeType -eq "Modify" }
    #$ChangeResults13b.delta
}
if ($CheckWhatIfs -eq $false -or $ChangeResults13.changeType -eq "Create" -or $ChangeResults13.changeType -eq "Modify")
{
    az deployment group create --resource-group $resourceGroupName --name $webSiteName --template-file "$templatesLocation\Website.json" --parameters webSiteName=$webSiteName hostingPlanName=$webhostingName storageAccountName=$storageAccountName websiteDomainName=$websiteDomainName 
    #web site managed identity and setting keyvault access permissions
    $websiteProdSlotIdentity = az webapp identity assign --resource-group $resourceGroupName --name $webSiteName 
    $websiteStagingSlotIdentity = az webapp identity assign --resource-group $resourceGroupName --name $webSiteName  --slot staging
    $websiteProdSlotIdentityPrincipalId = ($websiteProdSlotIdentity | ConvertFrom-Json | SELECT PrincipalId).PrincipalId
    $websiteStagingSlotIdentityPrincipalId =($websiteStagingSlotIdentity | ConvertFrom-Json | SELECT PrincipalId).PrincipalId
    Write-Host "prod: " $websiteProdSlotIdentityPrincipalId
    Write-Host "staging: " $websiteStagingSlotIdentityPrincipalId
    Write-Host "Setting access policies for key vault"
    $policy3 = az keyvault set-policy --name $keyVaultName --object-id $websiteProdSlotIdentityPrincipalId --secret-permissions list get
    $policy4 = az keyvault set-policy --name $keyVaultName --object-id $websiteStagingSlotIdentityPrincipalId --secret-permissions list get
}
else
{
    Write-Host "13. Website CheckWhatIf: $CheckWhatIfs and change type: $($ChangeResults13.changeType) results"
}
#Generate the certificate
$newCert = az webapp config ssl create --hostname $websiteDomainName --name $webSiteName --resource-group $resourceGroupName --only-show-errors
$thumbprint = ($newCert | ConvertFrom-Json).thumbprint
#Bind the certificate to the web app
az webapp config ssl bind --certificate-thumbprint $thumbprint --ssl-type SNI --name $webSiteName --resource-group $resourceGroupName
$timing = -join($timing, "13. Website created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "13. Website created: "$stopwatch.Elapsed.TotalSeconds

#Website alerts
if ($CheckWhatIfs -eq $true)
{
    $whatifResultsJson = az deployment group what-if --no-pretty-print --only-show-errors --resource-group $resourceGroupName --name "webSiteAlerts" --template-file "$templatesLocation\WebAppAlerts.json" --parameters webAppName=$webSiteName actionGroupName=$actionGroupName 
    $whatifResults = $whatifResultsJson | ConvertFrom-Json 
    $ChangeResults13a = $whatifResults.changes 

    #Filter out properties
    for ($i=0; $i -le $ChangeResults13a.Count; $i++) 
    {
        if ($ChangeResults13a[$i].changeType -eq "Modify")
        {
            $ChangeResults13a[$i].delta = $ChangeResults13a[$i].delta | Where-Object { $_.path –notlike "properties.*" }
            if ($ChangeResults13a[$i].delta.Count -eq 0)
            {
                $ChangeResults13a[$i].changeType = "Ignore"    
            }
        }       
    }

    #$ChangeResults13ab = $ChangeResults13a | Where-Object { $_.changeType -eq "Modify" }
    #$ChangeResults13ab.delta
}
if ($CheckWhatIfs -eq $false -or $ChangeResults13a.changeType -eq "Create" -or $ChangeResults13a.changeType -eq "Modify")
{
    az deployment group create --resource-group $resourceGroupName --name "webSiteAlerts" --template-file "$templatesLocation\WebAppAlerts.json" --parameters webAppName=$webSiteName actionGroupName=$actionGroupName 
}
else
{
    Write-Host "13a. webSite Alerts CheckWhatIf: $CheckWhatIfs and change type: $($ChangeResults13a.changeType) results"
}
$timing = -join($timing, "13a. webSite Alerts created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "13a. webSite Alerts created: "$stopwatch.Elapsed.TotalSeconds

#Frontdoor
if ($CheckWhatIfs -eq $true)
{
    $whatifResultsJson = az deployment group what-if --no-pretty-print --only-show-errors --resource-group $resourceGroupName --name $frontDoorName --template-file "$templatesLocation\FrontDoor.json" --parameters frontDoorName=$frontDoorName frontDoorBackEndAddresses=$frontDoorBackEndAddresses
    $whatifResults = $whatifResultsJson | ConvertFrom-Json 
    $ChangeResults14 = $whatifResults.changes 

    #Filter out properties
    for ($i=0; $i -le $ChangeResults14.Count; $i++) 
    {
        if ($ChangeResults14[$i].changeType -eq "Modify")
        {
            $ChangeResults14[$i].delta = $ChangeResults14[$i].delta | Where-Object { $_.path –notlike "properties.*" }
            if ($ChangeResults14[$i].delta.Count -eq 0)
            {
                $ChangeResults14[$i].changeType = "Ignore"    
            }
        }       
    }

    #$ChangeResults14b = $ChangeResults14 | Where-Object { $_.changeType -eq "Modify" }
    #$ChangeResults14.delta
}
if ($CheckWhatIfs -eq $false -or $ChangeResults14.changeType -eq "Create" -or $ChangeResults14.changeType -eq "Modify")
{
    az deployment group create --resource-group $resourceGroupName --name $frontDoorName --template-file "$templatesLocation\FrontDoor.json" --parameters frontDoorName=$frontDoorName frontDoorBackEndAddresses=$frontDoorBackEndAddresses
    #Add extension to use Azure CLI for front door:
    #az extension add --name front-door
    #Add Frontdoor custom domain to frontend-endpoint, checking to see if it exists first
    $FrontDoorFrontEndEndPointsJson = az network front-door frontend-endpoint list --front-door-name $frontDoorName --resource-group $resourceGroupName
    $FrontDoorFrontEndEndPoints = $FrontDoorFrontEndEndPointsJson | ConvertFrom-Json
    $FoundFrontEndPoint = $false
    #We can't create the frontend point if it already exists, so check again
    foreach($FrontDoorFrontEndEndPoint in $FrontDoorFrontEndEndPoints) {
        if ($FrontDoorFrontEndEndPoint -ne $null)
        {
            if ($FrontDoorFrontEndEndPoint.name -eq $frontDoorDomainName.Replace(".","-"))
            {
                $FoundFrontEndPoint = $true
            }
        }
    }
    if ($FoundFrontEndPoint -eq $false)
    {
        az network front-door frontend-endpoint create --front-door-name $frontDoorName --host-name $frontDoorDomainName --name $frontDoorDomainName.Replace(".","-") --resource-group $resourceGroupName --session-affinity-enabled Disabled --session-affinity-ttl 0
    }

    #Add custom domain to routing rules
    az network front-door routing-rule update --front-door-name $frontDoorName --name "$frontDoorName-routing" --resource-group $resourceGroupName --frontend-endpoints "$frontDoorName-azurefd-net".Replace(".","-") $frontDoorDomainName.Replace(".","-")
    #Debugging stuff for the routing
    #$routingRulesJson = az network front-door routing-rule list --front-door-name $frontDoorName --resource-group $resourceGroupName
    #$routingRules = $routingRulesJson  | ConvertFrom-Json 
    #$routingRules[0].frontendEndpoints$timing = -join($timing, "14. Frontdoor created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
}
else
{
    Write-Host "14. Frontdoor CheckWhatIf: $CheckWhatIfs and change type: $($ChangeResults14.changeType) results"
}
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