# http://www.iis.net/configreference/system.applicationhost/applicationpools/add/processmodel
function CreateAppPool(
	[string]$iisAppPoolName,
	[string]$pipelineMode =  'Integrated',
	[string]$identity = 'ApplicationPoolIdentity',
	[string]$enable32Bit = "$false",
	[string]$iisAppPoolDotNetVersion = "",  # v4.0
	[string]$idleTimeout = "00:00:00",
	[string]$startMode = "alwaysrunning"
)
{
    Write-Host "Creating Application Pool: $iisAppPoolName" -foregroundcolor Yellow
    Write-Host "    Enable32Bit: $enable32Bit" -foregroundcolor Green
    Write-Host "    PipelineMode: $pipelineMode" -foregroundcolor Green
    Write-Host "    DotNetVersion: $iisAppPoolDotNetVersion" -foregroundcolor Green

    #check if the app pool exists
    if (!(Test-Path IIS:\AppPools\$iisAppPoolName -pathType container))
    {
        Write-Host "Creating $iisAppPoolName application pool..." -foregroundcolor Blue -backgroundColor Yellow

        #create the app pool
        $appPool = New-Item IIS:\AppPools\$iisAppPoolName
        $appPool | Set-ItemProperty -Name "managedRuntimeVersion" -Value $iisAppPoolDotNetVersion
        $appPool | Set-ItemProperty -Name "managedPipelineMode" -Value $pipelineMode

        $appPool | Set-ItemProperty -Name "startMode" -Value $startMode

        $appPool | Set-ItemProperty -Name "processModel.identityType" -Value $identity
        $appPool | Set-ItemProperty -Name "processModel.idleTimeout" -Value $idleTimeout

        $en32bit = [System.Convert]::ToBoolean($enable32Bit)
        $appPool | Set-ItemProperty -Name "enable32BitAppOnWin64" -Value $en32bit

        Write-Host "Application pool $iisAppPoolName has been created..." -foregroundcolor Yellow
    }
    else
    {
        Write-Host "Application Pool $iisAppPoolName already exists."  -foregroundcolor Green
    }
}