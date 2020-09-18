param
(
	[string] $appPrefix,
	[string] $environment,
	[string] $resourceGroupName,
	[string] $resourceGroupLocation,
	[string] $resourceGroupLocationShort,
	[string] $dataKeyVaultName,
	[string] $templatesLocation,
	[string] $sqlDatabaseName,
	[string] $sqlAdministratorLoginUser,
	[string] $sqlAdministratorLoginPassword,
	[string] $administratorUserLogin,
	[string] $administratorUserSid,
	[string] $storageAccountAccessKey,
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
Write-Host "sqlDatabaseName: $sqlDatabaseName"
Write-Host "sqlAdministratorLoginUser: $sqlAdministratorLoginUser"
Write-Host "sqlAdministratorLoginPassword: $sqlAdministratorLoginPassword"
Write-Host "administratorUserLogin: $administratorUserLogin"
Write-Host "administratorUserSid: $administratorUserSid"
Write-Host "storageAccountAccessKey: $storageAccountAccessKey"
Write-Host "azureDevOpsPrincipalId: $azureDevOpsPrincipalId"

#Variables
$storageAccountName = "$appPrefix$environment$($resourceGroupLocationShort)storage" #Must be <= 24 lowercase letters and numbers.
$sqlServerName = "$appPrefix-$environment-$resourceGroupLocationShort-sqlserver"
if ($storageAccountName.Length -gt 24)
{
    Write-Host "Storage account name must be 3-24 characters in length"
    Break
}
$timing = -join($timing, "2. Variables created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "2. Variables created: "$stopwatch.Elapsed.TotalSeconds

#SQL
$sqlOutput = az deployment group create --resource-group $resourceGroupName --name $sqlServerName --template-file "$templatesLocation\SQL.json" --parameters sqlServerName=$sqlServerName databaseName=$sqlDatabaseName sqlAdministratorLogin=$sqlAdministratorLoginUser sqlAdministratorLoginPassword=$sqlAdministratorLoginPassword administratorUserLogin=$administratorUserLogin administratorUserSid=$administratorUserSid storageAccountName=$storageAccountName storageAccountAccessKey=$storageAccountAccessKey
$sqlJSON = $sqlOutput | ConvertFrom-Json
$sqlServerAddress = $sqlJSON.properties.outputs.sqlServerIPAddress.value
$sqlConnectionStringName = "ConnectionStrings--SamsAppConnectionString$Environment"
$sqlConnectionStringValue = "Server=tcp:$sqlServerName.database.windows.net,1433;Initial Catalog=$sqlDatabaseName;Persist Security Info=False;User ID=$sqlAdministratorLoginUser;Password=$sqlAdministratorLoginPassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
Write-Host "Setting value $sqlConnectionStringValue for $sqlConnectionStringName to key vault"
az keyvault secret set --vault-name $dataKeyVaultName --name "$sqlConnectionStringName" --value $sqlConnectionStringValue #Upload the secret into the key vault
$timing = -join($timing, "8. SQL created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "8. SQL created: "$stopwatch.Elapsed.TotalSeconds

Write-Host "sqlServerIPAddress: "$sqlServerIPAddress
$timing = -join($timing, "15. All Done: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "15. All Done: "$stopwatch.Elapsed.TotalSeconds
Write-Host "Timing: `n$timing"
Write-Host "Were there errors? (If the next line is blank, then no!) $error"