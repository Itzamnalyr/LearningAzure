#############################
#Export database to storage
#############################
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
	Write-Host "Creating new storage container blob $StorageContainerName"
	# Create a Blob Container in the Storage Account
	New-AzureStorageContainer -Name $StorageContainerName -Context $StorageContext 
}

# Generate the credentials required to connect to the SQL database
Write-Host "Generating credentials" 
$SecurePassword = ConvertTo-SecureString -String $ServerPassword -AsPlainText -Force
$Creds = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $ServerAdmin, $SecurePassword

# Generate a unique filename for the BACPAC, and then a connection to the file
$BacpacFilename = $DatabaseName + "_" + (Get-Date).ToString("yyyyMMddHHmm") + ".bacpac"
$BacpacUri = $StorageUri + $StorageContainerName + "/" + $BacpacFilename

# Export the database to storage
Write-Host "Exporting database"
$ExportRequest = New-AzureRmSqlDatabaseExport -ResourceGroupName $ResourceGroupName -ServerName $DBServerName -DatabaseName $DatabaseName -StorageKeytype "StorageAccessKey" -StorageKey $StorageAccountKey -StorageUri $BacpacUri -AdministratorLogin $Creds.UserName -AdministratorLoginPassword $Creds.Password
$ExportRequest

# Monitor export status, showing progress every 1 second
$exportStatus = Get-AzureRmSqlDatabaseImportExportStatus -OperationStatusLink $exportRequest.OperationStatusLink
while ($exportStatus.Status -eq "InProgress")
{
    Write-Host "Export in progress..." + (Get-Date).ToString("HH:mm:ss.ff")
	$exportStatus = Get-AzureRmSqlDatabaseImportExportStatus -OperationStatusLink $exportRequest.OperationStatusLink
    Start-Sleep -s 1
}
$exportStatus

Write-Host "Export complete"
