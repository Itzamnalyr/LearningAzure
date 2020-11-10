param
(
	[string] $appPrefix,
	[string] $environment,
	[string] $resourceGroupName,
	[string] $resourceGroupLocation,
	[string] $resourceGroupLocationShort,
	[string] $dataKeyVaultName,
	[string] $templatesLocation,
	[string] $administratorUserSid,
	[string] $azureDevOpsPrincipalId
)

$stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
$timing = ""
$timing = -join($timing, "1. Deployment started: ", $stopwatch.Elapsed.TotalSeconds, "`n")
Write-Host "1. Deployment started: "$stopwatch.Elapsed.TotalSeconds
Write-Host "Parameters:"
Write-Host "appPrefix: $appPrefix"
Write-Host "environment: $environment"
Write-Host "resourceGroupName: $resourceGroupName"
Write-Host "resourceGroupLocation: $resourceGroupLocation"
Write-Host "resourceGroupLocationShort: $resourceGroupLocationShort"
Write-Host "dataKeyVaultName: $dataKeyVaultName"
Write-Host "templatesLocation: $templatesLocation"
Write-Host "administratorUserSid: $administratorUserSid"
Write-Host "azureDevOpsPrincipalId: $azureDevOpsPrincipalId"

#Variables
$keyVaultName = "$appPrefix-$environment-$resourceGroupLocationShort-vault" #Must be <= 23 characters
$storageAccountName = "$appPrefix$environment$($resourceGroupLocationShort)storage" #Must be <= 24 lowercase letters and numbers.          
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
az group create --location $resourceGroupLocation --name $resourceGroupName
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
    #Get all deleted key vault names. If it matches, purge it
    $results = az keyvault list-deleted --subscription 07db7d0b-a6cb-4e58-b07e-e1d541c39f5b
    $results = $results | ConvertFrom-Json
    if ($results -ne $null -and $results.Length -gt 0)
    {
        #if we have purged keyvaults, search to see if we've used this name before and purge it
        foreach($purgedKV in $results) {
            if ($purgedKV.name -eq $keyVaultName)
            {
                Write-Host "Purging existing keyvault" $purgedKV.name         
                az keyvault purge --name $keyVaultName                     
            }
        }
    }
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
$timing = -join($timing, "4. Key vault created:: ", $stopwatch.Elapsed.TotalSeconds, "`n");
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

Write-Host "storageAccountAccessKey: "$storageAccountAccessKey
$timing = -join($timing, "6. All Done: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "6. All Done: "$stopwatch.Elapsed.TotalSeconds
Write-Host "Timing: `n$timing"
Write-Host "Were there errors? (If the next line is blank, then no!) $error"