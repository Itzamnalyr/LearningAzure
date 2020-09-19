param
(
	[string] $appPrefix,
	[string] $environment,
	[string] $resourceGroupName,
	[string] $resourceGroupLocation,
	[string] $resourceGroupLocationShort,
	[string] $dataKeyVaultName,
	[string] $templatesLocation,
	[string] $customDomain
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
Write-Host "customDomain: $customDomain"

#Variables
$frontDoorName = "$appPrefix-$environment-$resourceGroupLocationShort-frontdoor"
$webSiteName = "$appPrefix-$environment-$resourceGroupLocationShort-web"
$webSite2Name = "$appPrefix-$($environment)2-$resourceGroupLocationShort-web"
$frontDoorBackEndAddresses = "['$webSiteName.azurewebsites.net','$webSite2Name.azurewebsites.net']"  #create an array of strings for each of the back end pool resources
$timing = -join($timing, "2. Variables created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "2. Variables created: "$stopwatch.Elapsed.TotalSeconds


#Frontdoor ARM template
az deployment group create --resource-group $resourceGroupName --name $frontDoorName --template-file "$templatesLocation\FrontDoor.json" --parameters frontDoorName=$frontDoorName frontDoorBackEndAddresses=$frontDoorBackEndAddresses

#Add the extension first:
az extension add --name front-door
#Add Frontdoor custom domain to frontend-endpoint
$FrontDoorFrontEndEndPointsJson = az network front-door frontend-endpoint list --front-door-name $frontDoorName --resource-group $resourceGroupName
$FrontDoorFrontEndEndPoints = $FrontDoorFrontEndEndPointsJson | ConvertFrom-Json
$FoundFrontEndPoint = $false
#We can't create the frontend point if it already exists, so check again
foreach($FrontDoorFrontEndEndPoint in $FrontDoorFrontEndEndPoints) {
    if ($FrontDoorFrontEndEndPoint.name -eq $customDomain.Replace(".","-"))
    {
        $FoundFrontEndPoint = $true
    }
}
if ($FoundFrontEndPoint -eq $false)
{
    az network front-door frontend-endpoint create --front-door-name $frontDoorName --host-name $customDomain --name $customDomain.Replace(".","-") --resource-group $resourceGroupName --session-affinity-enabled Disabled --session-affinity-ttl 0
}

#Add custom domain to routing rules
az network front-door routing-rule update --front-door-name $frontDoorName --name "$frontDoorName-routing" --resource-group $resourceGroupName --frontend-endpoints "$frontDoorName-azurefd-net".Replace(".","-") $customDomain.Replace(".","-")
#Debugging stuff for the routing
#$routingRulesJson = az network front-door routing-rule list --front-door-name $frontDoorName --resource-group $resourceGroupName
#$routingRules = $routingRulesJson  | ConvertFrom-Json 
#$routingRules[0].frontendEndpoints

$timing = -join($timing, "14. Frontdoor created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "14. Frontdoor created: "$stopwatch.Elapsed.TotalSeconds

Write-Host "sqlServerIPAddress: "$sqlServerIPAddress
$timing = -join($timing, "15. All Done: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "15. All Done: "$stopwatch.Elapsed.TotalSeconds
Write-Host "Timing: `n$timing"
Write-Host "Were there errors? (If the next line is blank, then no!) $error"