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

Write-Host "looking..."
foreach($secret in $secrets2){
    if ($secret.name  -like '*PR4*')
    #if ($secret.name  -like '*PR4*' -and $secret.name -notlike "*PR456")
    {
        Write-Host "Deleting key $($secret.name)"
        az keyvault secret delete --name $secret.name --vault-name samsapp-data-eu-keyvault 
    }
    else
    {
       # Write-Host "Found nothing"
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
