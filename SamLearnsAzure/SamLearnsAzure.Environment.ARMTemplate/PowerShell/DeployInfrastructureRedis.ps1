param
(
	[string] $appPrefix,
	[string] $environment,
	[string] $resourceGroupName,
	[string] $resourceGroupLocation,
	[string] $resourceGroupLocationShort,
	[string] $dataKeyVaultName,
	[string] $templatesLocation
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

#Variables
$redisCacheName = "$appPrefix-$environment-$resourceGroupLocationShort-redis"
$CheckWhatIfs = $true
$timing = -join($timing, "2. Variables created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "2. Variables created: "$stopwatch.Elapsed.TotalSeconds

#Redis
if ($CheckWhatIfs -eq $true)
{
    $whatifResultsJson = az deployment group what-if --no-pretty-print --only-show-errors --resource-group $resourceGroupName --name $redisCacheName --template-file "$templatesLocation\Redis.json" --parameters redisCacheName=$redisCacheName
    $whatifResults = $whatifResultsJson | ConvertFrom-Json 
    $ChangeResults7 = $whatifResults.changes 

    #Filter out REDIS configuration, which can't be set on the Basic Redis SKU, but what-if returns as a problem
    $ChangeResults7 = $ChangeResults7 | Where-Object { $_.delta.path -ne "properties.redisConfiguration" }

    #$ChangeResults7b = $ChangeResults7 | Where-Object { $_.changeType -eq "Modify" }
    #$ChangeResults7b.delta
}
if ($CheckWhatIfs -eq $false -or $ChangeResults7.changeType -eq "Create" -or $ChangeResults7.changeType -eq "Modify")
{
    $redisOutput = az deployment group create --resource-group $resourceGroupName --name $redisCacheName --template-file "$templatesLocation\Redis.json" --parameters redisCacheName=$redisCacheName
    $redisJSON = $redisOutput | ConvertFrom-Json
    $redisConnectionString = $redisJSON.properties.outputs.redisConnectionStringOutput.value
    $redisCacheConnectionStringName = "AppSettings--RedisCacheConnectionString$Environment"
    Write-Host "Setting value $redisConnectionString for $redisCacheConnectionStringName to key vault"
    az keyvault secret set --vault-name $dataKeyVaultName --name "$redisCacheConnectionStringName" --value $redisConnectionString #Upload the secret into the key vault
}
else
{
    Write-Host "7. Redis CheckWhatIf: $CheckWhatIfs and change type: $($ChangeResults7.changeType) results"
}
$timing = -join($timing, "7. Redis created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "7. Redis created: "$stopwatch.Elapsed.TotalSeconds

$timing = -join($timing, "15. All Done: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "15. All Done: "$stopwatch.Elapsed.TotalSeconds
Write-Host "Timing: `n$timing"
Write-Host "Were there errors? (If the next line is blank, then no!) $error"