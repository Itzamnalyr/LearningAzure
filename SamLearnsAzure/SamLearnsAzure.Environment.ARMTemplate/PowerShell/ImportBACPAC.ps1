##################################
#Import from storage to database 
##################################
param
(
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
	[string] $ServiceObjectiveName,
	[bool] $RunImportAlways
)

# Check if there is data in the database
if ($RunImportAlways -eq $false)
{
	#Query the database to see if there are records in it
	$connection = New-Object System.Data.SqlClient.SqlConnection
	$connection.ConnectionString = "Server=tcp:$DBServerName.database.windows.net,1433;Database=$DatabaseName;User ID=$ServerAdmin;Password=$ServerPassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
	$connection.Open()
	$command = $connection.CreateCommand()
	$command.CommandText = "SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'dbo' AND t.name = 'Colors'"
	$result = $command.ExecuteReader()
	$table = new-object “System.Data.DataTable”
	$table.Load($result)
	$connection.Close()

	#If no records were found, then run the import, otherwise we can skip it
	$rows = $table.Rows.Count
	if ($rows -eq 0)
	{
		$RunImportAlways = $true
	}
	else
	{
		Write-Host "Import not required, $rows rows found"
	}
}

#Run the import if the override $RunImportAlways is set to true, or no rows were found in the database
if ($RunImportAlways -eq $true)
{
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
	Write-Host "Found file to import" $BacpacFilename.Name
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
	else
	{
		Write-Host "Database not present, deletion not required"
	}

	# Create the new database
    az sql db create --resource-group $ResourceGroupName --server $DBServerName --name "$DatabaseName" --edition $Edition --service-objective $ServiceObjectiveName

	# double checking the database restore is successful
	$DatabaseRestoreSuccessful = "false"
	$Counter = 0;
	do
	{
		try
		{
			# Restore the database from storage to target environment
			Write-Host "Starting database import..." + (Get-Date).ToString("HH:mm:ss.ff")
            Write-Host "$importRequest = az sql db import --resource-group $ResourceGroupName --server $DBServerName --name $DatabaseName --admin-user $ServerAdmin --admin-password $ServerPassword --storage-key $StorageAccountKey --storage-key-type StorageAccessKey --storage-uri $BacpacUri"  
			
            az sql db import --resource-group $ResourceGroupName --server $DBServerName --name $DatabaseName --admin-user $ServerAdmin --admin-password $ServerPassword --storage-key "$StorageAccountKey" --storage-key-type StorageAccessKey --storage-uri "$BacpacUri"  
			
            Write-Host "Finishing database import..." + (Get-Date).ToString("HH:mm:ss.ff")
            #$importRequest = New-AzSqlDatabaseImport -ResourceGroupName $ResourceGroupName -ServerName $DBServerName -DatabaseName $DatabaseName -StorageKeytype "StorageAccessKey" -StorageKey $StorageAccountKey -StorageUri $BacpacUri -AdministratorLogin $Creds.UserName -AdministratorLoginPassword $Creds.Password -Edition $Edition -ServiceObjectiveName $ServiceObjectiveName -DatabaseMaxSizeBytes 5000000 -ErrorAction SilentlyContinue
			#$importRequest

			# Monitor import status, showing progress every 1 second
			#$importStatus = Get-AzSqlDatabaseImportExportStatus -OperationStatusLink $importRequest.OperationStatusLink 
			#while ($importStatus.Status -eq "InProgress")
			#{
			#	Write-Host "Import in progress..." + (Get-Date).ToString("HH:mm:ss.ff")
			#	Start-Sleep -s 1
			#	$importStatus = Get-AzSqlDatabaseImportExportStatus -OperationStatusLink $importRequest.OperationStatusLink
			#}
			#$importStatus
			$DatabaseRestoreSuccessful = "true"
		}
		catch 
		{
            Write-Host "Error: $_"
			$Counter = $Counter + 1
			if ($Counter -lt 100)
			{
				$DatabaseRestoreSuccessful = "false"
				Write-Host "Import went awry. Waiting 10 seconds and then trying again!" (Get-Date).ToString("HH:mm:ss.ff")
				Start-Sleep -s 10 # wait 10 seconds and try again
			}
			else
			{
				Write-Host "Import went awry. Waited 1000 seconds and it still failed. Sad!" (Get-Date).ToString("HH:mm:ss.ff")
				$DatabaseRestoreSuccessful = "true"
                $dateString = $(Get-Date).ToString("HH:mm:ss.ff")
				throw [System.Exception] "Import went awry. Waited 1000 seconds and it still failed. Sad! $dateString" 			}
		}
	}
	while ($DatabaseRestoreSuccessful -eq "false")
}

Write-Host "Import complete!" (Get-Date).ToString("HH:mm:ss.ff")
