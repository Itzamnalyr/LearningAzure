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
$keyVaultName = "samsapp-data-eu-keyvault"

Write-Host "Key vault cleanup"
#Get all access policies from the message queue

$messageJson = az storage message get --queue-name $queueName --account-key $storageAccountKey --account-name $storageAccountName

do {
    $message = $messageJson | ConvertFrom-Json

    if ($message.Count -gt 0) {
        
        #Remove the access policy
        Write-Host "Removing key vault policy" $message.content 
        $result = az keyvault delete-policy --name $keyVaultName --object-id $message.content --subscription $subscriptionId --resource-group $resourceGroupName
        
        #Now delete the message from the queue
        az storage message delete --id $message.id --pop-receipt $message.popReceipt --queue-name $queueName --account-key $storageAccountKey --account-name $storageAccountName
    }

    $messageJson = az storage message get --queue-name $queueName --account-key $storageAccountKey --account-name $storageAccountName

} while ($messageJson -ne $null)

Write-Host "key vault cleanup complete"