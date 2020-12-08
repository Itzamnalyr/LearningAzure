param
(
	[string] $appPrefix,
	[string] $environment,
	[string] $webAppEnvironment,
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
Write-Host "webAppEnvironment: $webAppEnvironment"
Write-Host "resourceGroupName: $resourceGroupName"
Write-Host "resourceGroupLocation: $resourceGroupLocation"
Write-Host "resourceGroupLocationShort: $resourceGroupLocationShort"
Write-Host "dataKeyVaultName: $dataKeyVaultName"
Write-Host "templatesLocation: $templatesLocation"

#Variables
$webSiteName = "$appPrefix-$webAppEnvironment-$resourceGroupLocationShort-web"
$webhostingName = "$appPrefix-$environment-$resourceGroupLocationShort-hostingplan"
$actionGroupName = "$appPrefix-$environment-$resourceGroupLocationShort-actionGroup"
$storageAccountName = "$appPrefix$environment$($resourceGroupLocationShort)storage" #Must be <= 24 lowercase letters and numbers.
$websiteDomainName = "$($webAppEnvironment.ToString().ToLower()).samlearnsazure.com"
Write-Host "websiteDomainName: $websiteDomainName"
if ($storageAccountName.Length -gt 24)
{
    Write-Host "Storage account name must be 3-24 characters in length"
    Break
}

$CheckWhatIfs = $true
$timing = -join($timing, "2. Variables created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "2. Variables created: "$stopwatch.Elapsed.TotalSeconds

#Web site
if ($CheckWhatIfs -eq $true)
{
    $whatifResultsJson = az deployment group what-if --no-pretty-print --only-show-errors --resource-group $resourceGroupName --name $webSiteName --template-file "$templatesLocation\Website.json" --parameters webSiteName=$webSiteName hostingPlanName=$webhostingName storageAccountName=$storageAccountName websiteDomainName=$websiteDomainName
    $whatifResults = $whatifResultsJson | ConvertFrom-Json 
    $ChangeResults13 = $whatifResults.changes 

    #Filter out identity and properties noise
    for ($i=0; $i -le $ChangeResults13.Count; $i++) 
    {
        if ($ChangeResults13[$i].changeType -eq "Modify")
        {
            $ChangeResults13[$i].delta = $ChangeResults13[$i].delta | Where-Object { $_.path –notlike "properties*" }
            $ChangeResults13[$i].delta = ($ChangeResults13[$i].delta | Where-Object { $_.path –ne "identity" })
            $ChangeResults13[$i].delta = ($ChangeResults13[$i].delta | Where-Object { $_.path –ne "location" })
            if ($ChangeResults13[$i].delta.Count -eq 0)
            {
                $ChangeResults13[$i].changeType = "Ignore"    
            }
        }
        if ($ChangeResults13[$i].changeType -eq "Create")  
        {
            if ($ChangeResults13[$i].resourceId -like "*roleAssignments/6e4cff57-e63a-403e-822c-e98e5ba02145*")
            {
                $ChangeResults13[$i].changeType = "Ignore"  
            }
        }  
    }
    
    #$ChangeResults13b = $ChangeResults13 | Where-Object { $_.changeType -eq "Modify" }
    #$ChangeResults13b.delta
}
if ($CheckWhatIfs -eq $false -or $ChangeResults13.changeType -eq "Create" -or $ChangeResults13.changeType -eq "Modify")
{
    az deployment group create --resource-group $resourceGroupName --name $webSiteName --template-file "$templatesLocation\Website.json" --parameters webSiteName=$webSiteName hostingPlanName=$webhostingName storageAccountName=$storageAccountName websiteDomainName=$websiteDomainName
    
    #Setup web site managed identity and setting keyvault access permissions
    $websiteProdSlotIdentity = az webapp identity assign --resource-group $resourceGroupName --name $webSiteName 
    $websiteStagingSlotIdentity = az webapp identity assign --resource-group $resourceGroupName --name $webSiteName --slot staging
    $websiteProdSlotIdentityPrincipalId = ($websiteProdSlotIdentity | ConvertFrom-Json | SELECT PrincipalId).PrincipalId
    $websiteStagingSlotIdentityPrincipalId =($websiteStagingSlotIdentity | ConvertFrom-Json | SELECT PrincipalId).PrincipalId
    Write-Host "Prod PrincipalId: " $websiteProdSlotIdentityPrincipalId
    Write-Host "Staging PrincipalId: " $websiteStagingSlotIdentityPrincipalId
    Write-Host "Started access policy 1 for key vault"
    $policy1 = az keyvault set-policy --name $dataKeyVaultName --object-id $websiteProdSlotIdentityPrincipalId --secret-permissions list get
    Write-Host "Finished access policy 1 for key vault"
    Write-Host "Started access policy 2 for key vault"
    $policy2 = az keyvault set-policy --name $dataKeyVaultName --object-id $websiteStagingSlotIdentityPrincipalId --secret-permissions list get
    Write-Host "Finished access policy 2 for key vault"
}
else
{
    Write-Host "13. Website CheckWhatIf: $CheckWhatIfs and change type: $($ChangeResults13.changeType) results"
}

#Generate the certificate
$newCert = az webapp config ssl create --hostname $websiteDomainName --name $webSiteName --resource-group $resourceGroupName --only-show-errors
$thumbprint = ($newCert | ConvertFrom-Json).thumbprint
Write-Host "Thumbprint id: $thumbprint"
Write-Host "Cmd: az webapp config ssl create --hostname $websiteDomainName --name $webSiteName --resource-group $resourceGroupName"
Write-Host $newCert
#Bind the certificate to the web app
az webapp config ssl bind --certificate-thumbprint $thumbprint --ssl-type SNI --name $webSiteName --resource-group $resourceGroupName

$timing = -join($timing, "13. Website created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "13. Website created: "$stopwatch.Elapsed.TotalSeconds


#Website alerts
if ($CheckWhatIfs -eq $true)
{
    $whatifResultsJson = az deployment group what-if --no-pretty-print --only-show-errors --resource-group $resourceGroupName --name "webSiteAlerts" --template-file "$templatesLocation\WebAppAlerts.json" --parameters webAppName=$webSiteName actionGroupName=$actionGroupName 
    $whatifResults = $whatifResultsJson | ConvertFrom-Json 
    $ChangeResults13a = $whatifResults.changes 

    #Filter out properties
    for ($i=0; $i -le $ChangeResults13a.Count; $i++) 
    {
        if ($ChangeResults13a[$i].changeType -eq "Modify")
        {
            $ChangeResults13a[$i].delta = $ChangeResults13a[$i].delta | Where-Object { $_.path –notlike "properties.*" }
            if ($ChangeResults13a[$i].delta.Count -eq 0)
            {
                $ChangeResults13a[$i].changeType = "Ignore"    
            }
        }       
    }

    #$ChangeResults13ab = $ChangeResults13a | Where-Object { $_.changeType -eq "Modify" }
    #$ChangeResults13ab.delta
}
if ($CheckWhatIfs -eq $false -or $ChangeResults13a.changeType -eq "Create" -or $ChangeResults13a.changeType -eq "Modify")
{
    Write-Host "Webalerts deployment name: $($webSiteName)Alerts"
    Write-Host "webAppName: $webSiteName"
    Write-Host "actionGroupName: $actionGroupName"
    az deployment group create --resource-group $resourceGroupName --name "$($webSiteName)Alerts" --template-file "$templatesLocation\WebAppAlerts.json" --parameters webAppName=$webSiteName actionGroupName=$actionGroupName 
}
else
{
    Write-Host "13a. webSite Alerts CheckWhatIf: $CheckWhatIfs and change type: $($ChangeResults13a.changeType) results"
}
$timing = -join($timing, "13a. webSite Alerts created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "13a. webSite Alerts created: "$stopwatch.Elapsed.TotalSeconds

Write-Host "websitePrincipalId: "$websiteProdSlotIdentityPrincipalId
Write-Host "websiteStagingSlotPrincipalId: "$websiteStagingSlotIdentityPrincipalId
$timing = -join($timing, "15. All Done: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "15. All Done: "$stopwatch.Elapsed.TotalSeconds
Write-Host "Timing: `n$timing"
Write-Host "Were there errors? (If the next line is blank, then no!) $error"