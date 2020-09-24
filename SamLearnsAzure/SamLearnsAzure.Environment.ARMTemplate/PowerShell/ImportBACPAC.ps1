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

Write-Host "ResourceGroupName: $ResourceGroupName"
Write-Host "DBServerName: $DBServerName"
Write-Host "ServerAdmin: $ServerAdmin"
Write-Host "DatabaseName: $DatabaseName"
Write-Host "StorageAccountName: $StorageAccountName"
Write-Host "StorageUri: $StorageUri"
Write-Host "StorageContainerName: $StorageContainerName"
Write-Host "Edition: $Edition"
Write-Host "ServiceObjectiveName: $ServiceObjectiveName"


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
	# Get the last file in the blob
    $Blobs = az storage blob list --container-name $StorageContainerName --prefix $DatabaseName --account-name $StorageAccountName --account-key $StorageAccountKey
	$Blobs = $Blobs | ConvertFrom-Json 
	if ($Blobs.Length -ne 0)
	{
		$BacpacFilename = $Blobs[-1].name #Get the last item in the array
	}
	if (!$BacpacFilename)
	{
		Write-Error "BACPAC does not exist in container $StorageContainerName. Import aborted"
	}
	Write-Host "Found file to import" $BacpacFilename
	$BacpacUri = $StorageUri + $StorageContainerName + "/" + $BacpacFilename

	# Remove the current database, if it exists
	Write-Host "Checking if database already exists..."
    $ExistingDatabases = az sql db list --resource-group $ResourceGroupName --server $DBServerName 
	$ExistingDatabases = $ExistingDatabases | ConvertFrom-Json
	$ExistingDatabase = $ExistingDatabases | Select name | Where-Object {$_.name -eq $DatabaseName} 
	if ($ExistingDatabase)
	{
		Write-Host "Removing existing database..."
        az sql db delete --name $DatabaseName --resource-group $ResourceGroupName --server $DBServerName --yes #--no-wait
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