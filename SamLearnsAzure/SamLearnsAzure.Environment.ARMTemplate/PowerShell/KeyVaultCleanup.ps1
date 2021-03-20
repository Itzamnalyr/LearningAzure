#######################################################################################
#Clean unused secrets and access policies from key vault 
#######################################################################################
param
(
#	[string] $subscriptionId,
#	[string] $resourceGroupName,
#	[string] $storageAccountName,
	[string] $storageAccountKey#,
#	[string] $queueName,
#	[string] $keyVaultName
)

$subscriptionId = "07db7d0b-a6cb-4e58-b07e-e1d541c39f5b"
$resourceGroupName = "SamLearnsAzureData"
$storageAccountName = "samsappdataeustorage"
$queueName = "keyvaultcleanupqueue"
$queue2Name = "keyvaultsecretscleanupqueue"
$keyVaultName = "samsapp-data-eu-keyvault"

Write-Host "Key vault cleanup"

#Remove old access policies from the key vault
$messageJson = az storage message get --queue-name $queueName --account-key $storageAccountKey --account-name $storageAccountName

do {
    $message = $messageJson | ConvertFrom-Json

    if ($message.Count -gt 0) {
        
        #Remove the access policy
        Write-Host "Removing key vault policy $(message.content)"
        $result = az keyvault delete-policy --name $keyVaultName --object-id $message.content --subscription $subscriptionId --resource-group $resourceGroupName
        
        #Now delete the message from the queue
        az storage message delete --id $message.id --pop-receipt $message.popReceipt --queue-name $queueName --account-key $storageAccountKey --account-name $storageAccountName
    }

    $messageJson = az storage message get --queue-name $queueName --account-key $storageAccountKey --account-name $storageAccountName

} while ($messageJson -ne $null)

#Remove old secrets from the key vault
$secretsMessageJson = az storage message get --queue-name $queue2Name --account-key $storageAccountKey --account-name $storageAccountName
$secretsMessage = $secretsMessageJson | ConvertFrom-Json

if ($secretsMessage.Count -gt 0) {
        
    $secrets = az keyvault secret list --vault-name $KeyVaultName
    $secrets2 = $secrets | ConvertFrom-Json
    $secrets2 | select name | ft

    $i = 0
    Write-Host "looking for secrets for $(secretsMessage.Content)"
    foreach($secret in $secrets2){
        $i++
        Write-Host "looking for secret... $i"
        if ($secret.name -like "*$(secretsMessage.Content)*")
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
    Write-Host "looking for purged secrets for $(secretsMessage.Content)"
    foreach($secret in $secretsDeleted2){
        $i++
        Write-Host "looking for purge... $i"
        if ($secret.name -like "*$(secretsMessage.Content)*")
        {
            Write-Host "Purging key $($secret.name)"
            az keyvault secret purge --name $secret.name --vault-name $KeyVaultName
        }
    }

    #Now delete the message from the queue
    az storage message delete --id $secretsMessage.id --pop-receipt $secretsMessage.popReceipt --queue-name $queue2Name --account-key $storageAccountKey --account-name $storageAccountName

}


Write-Host "key vault cleanup complete"