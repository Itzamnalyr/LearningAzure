#######################################################################################
#Clean up deployments older than 30 days to ensure we don't hit the limit of 800 deployments: 
#######################################################################################
param
(
	[string] $ResourceGroupName
)

Get-AzResourceGroup -Name $ResourceGroupName -ErrorVariable notPresent -ErrorAction SilentlyContinue
if ($notPresent)
{
    # ResourceGroup doesn't exist, do nothing
    Write-Host "Resource group $ResourceGroupName does not (yet) exist"
}
else
{
    # ResourceGroup exist
    Write-Host "Getting a list of all deployments older than 30 days"
    $daysOlderToDelete = 30
    $deployments = Get-AzResourceGroupDeployment -ResourceGroupName $ResourceGroupName | Where-Object Timestamp -lt ((Get-Date).AddDays(-$daysOlderToDelete))


    Write-Host "removing deployments " $deployments.Length
    $deploymentsDone = 0
    foreach ($deployment in $deployments) {
        Write-Host ($deployments.Length - $deploymentsDone) " deployments left!"
        $deploymentsDone++
        Remove-AzResourceGroupDeployment -ResourceGroupName $ResourceGroupName -Name $deployment.DeploymentName
    }

    Write-Host "Deployment cleanup completed " $deployments.Length
}

