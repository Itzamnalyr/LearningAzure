##################################
#Import from storage to database 
##################################
param
(
	[string] $SubscriptionId,
	[string] $ResourceGroupName,
	[string] $DBServerName,
	[string] $ServerAdmin,
	[string] $ServerPassword,
	[string] $DatabaseName,
	[string] $StorageAccountName,
	[string] $StorageUri,
	[string] $StorageAccountKey,
	[string] $StorageContainerName
)

# Check if the Storage Context exists. The storage container is required to backup the database
Write-Host "Checking storage for container blob $StorageContainerName"
$StorageContext = New-AzureStorageContext -StorageAccountName $StorageAccountName -StorageAccountKey $StorageAccountKey
$StorageContainer = Get-AzureStorageContainer -Name $StorageContainerName -Context $StorageContext

# Create the storage if it doesn't exist
if (!$StorageContainer)
{
	Write-Error "Container $StorageContainerName does not exist. Import aborted"
}

# Generate the credentials required to connect to the SQL database
Write-Host "Generating credentials" 
$SecurePassword = ConvertTo-SecureString -String $ServerPassword -AsPlainText -Force
$Creds = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $ServerAdmin, $SecurePassword

# Generate a connection to the file
$Blobs = Get-AzureStorageBlob -Container $StorageContainerName -Context $StorageContext -Prefix $DatabaseName
$BacpacFilename = $Blobs | Sort-Object -Property @{Expression={$_.LastModified}} -Descending | Select-Object Name, LastModified -First 1
if (!$BacpacFilename)
{
	Write-Error "BACPAC does not exist in container $StorageContainerName. Import aborted"
}
$BacpacUri = $StorageUri + $StorageContainerName + "/" + $BacpacFilename.Name

# Remove the current database, if it exists
$ExistingDatabases = Get-AzureRmSqlDatabase -ResourceGroupName $ResourceGroupName -ServerName $DBServerName 
$ExistingDatabase = $ExistingDatabases | Where-Object {$_.DatabaseName -eq $DatabaseName}
if ($ExistingDatabase)
{
	Remove-AzureRmSqlDatabase -ResourceGroupName $ResourceGroupName -ServerName $DBServerName -DatabaseName $DatabaseName -Force
}

# Restore the database from storage to target environment
Write-Host "Importing database"
$importRequest = New-AzureRmSqlDatabaseImport -ResourceGroupName $ResourceGroupName -ServerName $DBServerName -DatabaseName $DatabaseName -StorageKeytype "StorageAccessKey" -StorageKey $StorageAccountKey -StorageUri $BacpacUri -AdministratorLogin $Creds.UserName -AdministratorLoginPassword $Creds.Password -Edition Standard -ServiceObjectiveName S0 -DatabaseMaxSizeBytes 50000
$importRequest

# Monitor import status, showing progress every 1 second
$importStatus = Get-AzureRmSqlDatabaseImportExportStatus -OperationStatusLink $importRequest.OperationStatusLink
while ($importStatus.Status -eq "InProgress")
{
    Write-Host "Import in progress..." + (Get-Date).ToString("HH:mm:ss.ff")
	$importStatus = Get-AzureRmSqlDatabaseImportExportStatus -OperationStatusLink $importRequest.OperationStatusLink
    Start-Sleep -s 1
}
$importStatus

Write-Host "Import complete"
