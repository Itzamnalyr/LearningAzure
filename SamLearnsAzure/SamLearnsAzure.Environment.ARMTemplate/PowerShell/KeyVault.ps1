#######################################################################################
#Extract important information from arm template variables and store in key vault
#######################################################################################
param
(
	[string] $KeyVaultName,
	[string] $Environment,
	[string] $ServicePrincipalId,
	[string] $ServiceStagingSlotPrincipalId,
	[string] $WebsitePrincipalId,
	[string] $WebsiteStagingSlotPrincipalId,
	[string] $ApplicationInsightsInstrumentationKey,
	[string] $StorageAccountKey,
	[string] $DatabaseServerName,
	[string] $DatabaseLoginName,
	[string] $DatabaseLoginPassword,
	[string] $RedisConnectionString
)

Write-Host "Key vault clean up"
$secrets = az keyvault secret list --vault-name $KeyVaultName
$secrets2 = $secrets | ConvertFrom-Json
$secrets2 | select name | ft

$i = 0
foreach($secret in $secrets2){
    $i++
    Write-Host "looking... $i"
    if ($secret.name -like '*PR5*')
    {
        Write-Host "Deleting key $($secret.name)"
        az keyvault secret delete --name $secret.name --vault-name $KeyVaultName
    }
}
    
Write-Host "purging old secrets"
$secretsDeleted = az keyvault secret list-deleted --vault-name $KeyVaultName
$secretsDeleted2 = $secretsDeleted | ConvertFrom-Json
$secretsDeleted2 | select name | ft

$i = 0
foreach($secret in $secretsDeleted2){
    $i++
    Write-Host "looking... $i"
    if ($secret.name -like '*PR4*')
    {
        Write-Host "Purging key $($secret.name)"
        az keyvault secret purge --name $secret.name --vault-name samsapp-data-eu-keyvault 
    }
}

Write-Host "Setting key vault secrets"
#Get the application insights instrumentation key from the ARM Template outputs
#$applicationInsightsInstrumentationKeyName = "ApplicationInsights--InstrumentationKey$Environment"
#$applicationInsightsInstrumentationKeySecretvalue = ConvertTo-SecureString "$ApplicationInsightsInstrumentationKey" -AsPlainText -Force
#$applicationInsightsInstrumentationKeyExistingSecretValue = (Get-AzKeyVaultSecret -vaultName "$KeyVaultName" -name "$applicationInsightsInstrumentationKeyName").SecretValueText
#Write-Host "Setting value $ApplicationInsightsInstrumentationKey for $applicationInsightsInstrumentationKeyName to key vault"
#Set-AzKeyVaultSecret -VaultName "$KeyVaultName" -Name "$applicationInsightsInstrumentationKeyName" -SecretValue $applicationInsightsInstrumentationKeySecretvalue

#Get the storage account key from the ARM Template outputs
#$storageAccountName = "StorageAccountKey$Environment"
#$storageAccountSecretvalue = ConvertTo-SecureString "$StorageAccountKey" -AsPlainText -Force
#$storageAccountExistingSecretValue = (Get-AzKeyVaultSecret -vaultName "$KeyVaultName" -name "$storageAccountName").SecretValueText
#Write-Host "Setting value for $storageAccountName to key vault"
#Set-AzKeyVaultSecret -VaultName "$KeyVaultName" -Name "$storageAccountName" -SecretValue $storageAccountSecretvalue

#Get the sql server connection string from the ARM Template outputs
#$sqlConnectionStringName = "ConnectionStrings--SamsAppConnectionString$Environment"
#$sqlConnectionStringSecretvalue = ConvertTo-SecureString "Server=tcp:$DatabaseServerName.database.windows.net,1433;Initial Catalog=samsdb;Persist Security Info=False;User ID=$DatabaseLoginName;Password=$DatabaseLoginPassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" -AsPlainText -Force
#$sqlConnectionStringExistingSecretValue = (Get-AzKeyVaultSecret -vaultName "$KeyVaultName" -name "$sqlConnectionStringName").SecretValueText
#Write-Host "Setting value for $sqlConnectionStringName to key vault"
#Set-AzKeyVaultSecret -VaultName "$KeyVaultName" -Name "$sqlConnectionStringName" -SecretValue $sqlConnectionStringSecretvalue

#Get the redis connection string from the ARM Template outputs
#$redisCacheConnectionStringName = "AppSettings--RedisCacheConnectionString$Environment"
#$redisCacheConnectionStringSecretvalue = ConvertTo-SecureString "$RedisConnectionString" -AsPlainText -Force
#$redisCacheConnectionStringExistingSecretValue = (Get-AzKeyVaultSecret -vaultName "$KeyVaultName" -name "$redisCacheConnectionStringName").SecretValueText
#Write-Host "Setting value for $redisCacheConnectionStringName to key vault"
#Set-AzKeyVaultSecret -VaultName "$KeyVaultName" -Name "$redisCacheConnectionStringName" -SecretValue $redisCacheConnectionStringSecretvalue

Write-Host "key vault updates complete"



##############################
## POC
##############################

#cls
$keyvaultJson = az keyvault show --name "samsapp-data-eu-keyvault"
$keyvault = $keyvaultJson | ConvertFrom-Json
$accesspolicies = $keyvault.properties.accessPolicies | Select objectId | Sort-Object -property objectId

#Get the resources with identities in each resource group in a sub
$resourcegroupJson = az group list --subscription 07db7d0b-a6cb-4e58-b07e-e1d541c39f5b
$resourcegroups = $resourcegroupJson | ConvertFrom-Json

$items = @()
foreach($resourcegroup in $resourcegroups) {
    $resourceJson = az resource list --subscription 07db7d0b-a6cb-4e58-b07e-e1d541c39f5b --resource-group $resourcegroup.name
    $resource = $resourceJson | ConvertFrom-Json
    $resources = $resource | Select name, identity | Where-Object identity -ne $null
    foreach($item in $resources) {
        if ($resource.identity -ne $null) {
            $obj = New-Object System.Object
            $obj | Add-Member -type NoteProperty -name ResourceGroup -Value $resourcegroup.name
            $obj | Add-Member -type NoteProperty -name ResourceName -Value $item.name
            $obj | Add-Member -type NoteProperty -name IdentityId -Value $item.identity.principalId
            $obj | Add-Member -type NoteProperty -name IdentityType -Value $item.identity.type
            $items += $obj
        }
    }
}
$items

#now match up the resource id's with the access policies
foreach($resource in $items) {
    #Write-Host "Identity id: " $resource.IdentityId
    foreach($accesspolicy in $accesspolicies) {
        #$accesspolicy
        #Write-Host "Identity id: " + $resource.IdentityId + ", access policy: " + $accesspolicy.objectId
        if ($resource.IdentityId -eq $accesspolicy.objectId) {               
            #Write-Host "$Found resource " $resource.ResourceName "Identity id: " $resource.IdentityId ", access policy: " $accesspolicy.objectId
        }
    }
}