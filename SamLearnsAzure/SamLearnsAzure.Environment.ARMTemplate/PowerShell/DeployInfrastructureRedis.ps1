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
$redisCacheName = "$appPrefix-$environment-$locationShort-redis"
$timing = -join($timing, "2. Variables created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "2. Variables created: "$stopwatch.Elapsed.TotalSeconds

#Redis  
$redisOutput = az deployment group create --resource-group $resourceGroupName --name $redisCacheName --template-file "$templatesLocation\Redis.json" --parameters redisCacheName=$redisCacheName
$redisJSON = $redisOutput | ConvertFrom-Json
$redisConnectionString = $redisJSON.properties.outputs.redisConnectionStringOutput.value
$redisCacheConnectionStringName = "AppSettings--RedisCacheConnectionString$Environment"
Write-Host "Setting value $redisConnectionString for $redisCacheConnectionStringName to key vault"
az keyvault secret set --vault-name $dataKeyVaultName --name "$redisCacheConnectionStringName" --value $redisConnectionString #Upload the secret into the key vault
$timing = -join($timing, "3. Redis created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "3. Redis created: "$stopwatch.Elapsed.TotalSeconds

$timing = -join($timing, "4. All Done created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "4. All Done: "$stopwatch.Elapsed.TotalSeconds
Write-Host "Timing: `n$timing"
Write-Host "Were there errors? (If the next line is blank, then no!) $error"