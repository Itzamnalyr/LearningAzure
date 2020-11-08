param
(
	[string] $appPrefix,
	[string] $environment,
	[string] $resourceGroupName,
	[string] $resourceGroupLocation,
	[string] $resourceGroupLocationShort,
	[string] $dataKeyVaultName,
	[string] $templatesLocation,
	[string] $contactEmailAddress
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
Write-Host "contactEmailAddress: $contactEmailAddress"

#Variables
$webhostingName = "$appPrefix-$environment-$resourceGroupLocationShort-hostingplan"
$actionGroupName = "$appPrefix-$environment-$resourceGroupLocationShort-actionGroup"
$actionGroupShortName = "$environment-actgrp"
$applicationInsightsName = "$appPrefix-$environment-$resourceGroupLocationShort-appinsights"
$applicationInsightsAvailablityTestName = "Availability home page test-$applicationInsightsName"
if ($environment -ne "Prod")
{
	$websiteDomainName = "$environment.samlearnsazure.com"
}
else
{
	$websiteDomainName = "samlearnsazure.com"
}
$timing = -join($timing, "2. Variables created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "2. Variables created: "$stopwatch.Elapsed.TotalSeconds

#Action Group
if ($CheckWhatIfs -eq $true)
{
    $whatifResultsJson = az deployment group what-if --no-pretty-print --only-show-errors --resource-group $resourceGroupName --name $actionGroupName --template-file "$templatesLocation\ActionGroup.json" --parameters actionGroupName=$actionGroupName appPrefix=$appPrefix environment=$environment contactEmailAddress=$contactEmailAddress
    $whatifResults = $whatifResultsJson | ConvertFrom-Json 
    $ChangeResults9 = $whatifResults.changes 
}
if ($CheckWhatIfs -eq $false -or $ChangeResults9.changeType -eq "Create" -or $ChangeResults9.changeType -eq "Modify")
{
	az deployment group create --resource-group $resourceGroupName --name $actionGroupName --template-file "$templatesLocation\ActionGroup.json" --parameters actionGroupName=$actionGroupName appPrefix=$appPrefix environment=$environment contactEmailAddress=$contactEmailAddress
}
else
{
    Write-Host "9. Action group CheckWhatIf: $CheckWhatIfs and change type: $($ChangeResults9.changeType) results"
}
$timing = -join($timing, "9. Action group created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "9. Action group created: "$stopwatch.Elapsed.TotalSeconds

#Application Insights
if ($CheckWhatIfs -eq $true)
{
    $whatifResultsJson = az deployment group what-if --no-pretty-print --only-show-errors --resource-group $resourceGroupName --name $applicationInsightsName --template-file "$templatesLocation\ApplicationInsights.json" --parameters applicationInsightsName=$applicationInsightsName applicationInsightsAvailablityTestName="$applicationInsightsAvailablityTestName" websiteDomainName=$websiteDomainName 
    $whatifResults = $whatifResultsJson | ConvertFrom-Json 
    $ChangeResults10 = $whatifResults.changes 

    #$ChangeResults10b = $ChangeResults10 | Where-Object { $_.changeType -eq "Modify" }
    #$ChangeResults10b.delta
}
if ($CheckWhatIfs -eq $false -or $ChangeResults10.changeType -eq "Create" -or $ChangeResults10.changeType -eq "Modify")
{
	$applicationInsightsOutput = az deployment group create --resource-group $resourceGroupName --name $applicationInsightsName --template-file "$templatesLocation\ApplicationInsights.json" --parameters applicationInsightsName=$applicationInsightsName applicationInsightsAvailablityTestName="$applicationInsightsAvailablityTestName" websiteDomainName=$websiteDomainName 
	$applicationInsightsJSON = $applicationInsightsOutput | ConvertFrom-Json
	$applicationInsightsInstrumentationKey = $applicationInsightsJSON.properties.outputs.applicationInsightsInstrumentationKeyOutput.value
	$applicationInsightsInstrumentationKeyName = "ApplicationInsights--InstrumentationKey$Environment"
	Write-Host "Setting value $ApplicationInsightsInstrumentationKey for $applicationInsightsInstrumentationKeyName to key vault"
	az keyvault secret set --vault-name $dataKeyVaultName --name "$applicationInsightsInstrumentationKeyName" --value $ApplicationInsightsInstrumentationKey #Upload the secret into the key vault
}
else
{
    Write-Host "10. Application insights CheckWhatIf: $CheckWhatIfs and change type: $($ChangeResults10.changeType) results"
}
$timing = -join($timing, "10. Application created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "10. Application insights created: "$stopwatch.Elapsed.TotalSeconds

#Web hosting
if ($CheckWhatIfs -eq $true)
{
    $whatifResultsJson = az deployment group what-if --no-pretty-print --only-show-errors --resource-group $resourceGroupName --name $webhostingName --template-file "$templatesLocation\WebHosting.json" --parameters hostingPlanName=$webhostingName actionGroupName=$actionGroupName 
    $whatifResults = $whatifResultsJson | ConvertFrom-Json 
    $ChangeResults11 = $whatifResults.changes 

    #Filter out metric alert
    for ($i=0; $i -le $ChangeResults11.Count; $i++) 
    {
        if ($ChangeResults11[$i].changeType -eq "Modify")
        {
            #$ChangeResults11[$i].delta.path | Where-Object { $_.path –eq "properties.targetResourceType" }
            $ChangeResults11[$i].delta = ($ChangeResults11[$i].delta | Where-Object { $_.path –ne "properties.targetResourceType" })
            $ChangeResults11[$i].delta = ($ChangeResults11[$i].delta | Where-Object { $_.path –ne "properties.criteria.allOf" })
            $ChangeResults11[$i].delta = ($ChangeResults11[$i].delta | Where-Object { $_.path –ne "properties.profiles" })
            if ($ChangeResults11[$i].delta.Count -eq 0)
            {
                $ChangeResults11[$i].changeType = "Ignore"    
            }
        }       
    }
    
    #$ChangeResults11b = $ChangeResults11 | Where-Object { $_.changeType -eq "Modify" }
    #$ChangeResults11b.delta

}
if ($CheckWhatIfs -eq $false -or $ChangeResults11.changeType -eq "Create" -or $ChangeResults11.changeType -eq "Modify")
{
    az deployment group create --resource-group $resourceGroupName --name $webhostingName --template-file "$templatesLocation\WebHosting.json" --parameters hostingPlanName=$webhostingName actionGroupName=$actionGroupName 
}
else
{
    Write-Host "11. Web hosting CheckWhatIf: $CheckWhatIfs and change type: $($ChangeResults11.changeType) results"
}
$timing = -join($timing, "11. Web hosting created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "11. Web hosting created: "$stopwatch.Elapsed.TotalSeconds

Write-Host "applicationInsightsInstrumentationKey: "$applicationInsightsInstrumentationKey
$timing = -join($timing, "15. All Done: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "15. All Done: "$stopwatch.Elapsed.TotalSeconds
Write-Host "Timing: `n$timing"
Write-Host "Were there errors? (If the next line is blank, then no!) $error"