﻿##################################
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
	[string] $StorageContainerName,
	[string] $Edition,
	[string] $ServiceObjectiveName
)

# Check if the Storage Context exists. The storage container is required to backup the database
Write-Host "Checking storage for container blob $StorageContainerName"
$StorageContext = New-AzStorageContext -StorageAccountName $StorageAccountName -StorageAccountKey $StorageAccountKey
$StorageContainer = Get-AzStorageContainer -Name $StorageContainerName -Context $StorageContext

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
$Blobs = Get-AzStorageBlob -Container $StorageContainerName -Context $StorageContext -Prefix $DatabaseName
$BacpacFilename = $Blobs | Sort-Object -Property @{Expression={$_.LastModified}} -Descending | Select-Object Name, LastModified -First 1
if (!$BacpacFilename)
{
	Write-Error "BACPAC does not exist in container $StorageContainerName. Import aborted"
}
Write-Host "Found file to import" + $BacpacFilename.Name
$BacpacUri = $StorageUri + $StorageContainerName + "/" + $BacpacFilename.Name

# Remove the current database, if it exists
Write-Host "Checking if database already exists..."
$ExistingDatabases = Get-AzSqlDatabase -ResourceGroupName $ResourceGroupName -ServerName $DBServerName 
$ExistingDatabase = $ExistingDatabases | Where-Object {$_.DatabaseName -eq $DatabaseName}
if ($ExistingDatabase)
{
	Write-Host "Removing existing database..."
	Remove-AzSqlDatabase -ResourceGroupName $ResourceGroupName -ServerName $DBServerName -DatabaseName $DatabaseName -Force
	do
	{
		Write-Host "Deleting in progress..." (Get-Date).ToString("HH:mm:ss.ff")
		Start-Sleep -s 1
		$DeletingExistingDatabase = Get-AzSqlDatabase -ResourceGroupName $ResourceGroupName -ServerName $DBServerName | Where-Object {$_.DatabaseName -eq $DatabaseName} 
	}
	while ($DeletingExistingDatabase -ne $null)
	Write-Host "Delete done! " (Get-Date).ToString("HH:mm:ss.ff")
}

# double checking the database restore is successful
$DatabaseRestoreSuccessful = "false"
$Counter = 0;
do
{
	try
	{
		# Restore the database from storage to target environment
		Write-Host "Starting database import"
		$importRequest = New-AzSqlDatabaseImport -ResourceGroupName $ResourceGroupName -ServerName $DBServerName -DatabaseName $DatabaseName -StorageKeytype "StorageAccessKey" -StorageKey $StorageAccountKey -StorageUri $BacpacUri -AdministratorLogin $Creds.UserName -AdministratorLoginPassword $Creds.Password -Edition $Edition -ServiceObjectiveName $ServiceObjectiveName -DatabaseMaxSizeBytes 50000 -ErrorAction SilentlyContinue
		$importRequest

		# Monitor import status, showing progress every 1 second
		$importStatus = Get-AzSqlDatabaseImportExportStatus -OperationStatusLink $importRequest.OperationStatusLink 
		while ($importStatus.Status -eq "InProgress")
		{
			Write-Host "Import in progress..." + (Get-Date).ToString("HH:mm:ss.ff")
			Start-Sleep -s 1
			$importStatus = Get-AzSqlDatabaseImportExportStatus -OperationStatusLink $importRequest.OperationStatusLink
		}
		$importStatus
		$DatabaseRestoreSuccessful = "true"
	}
	catch 
	{
		$Counter = $Counter + 1
		if ($Counter < 100)
		{
			$DatabaseRestoreSuccessful = "false"
			Write-Host "Import went awry. Waiting 10 seconds and then trying again!" (Get-Date).ToString("HH:mm:ss.ff")
			Start-Sleep -s 10 # wait 10 seconds and try again
		}
		else
		{
			Write-Host "Import went awry. Waited 1000 seconds and it still failed. Sad!" (Get-Date).ToString("HH:mm:ss.ff")
			$DatabaseRestoreSuccessful = "true"
		}
	}
}
while ($DatabaseRestoreSuccessful -eq "false")

Write-Host "Import complete!" (Get-Date).ToString("HH:mm:ss.ff")
