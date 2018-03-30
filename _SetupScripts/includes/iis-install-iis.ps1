# https://www.iis.net/learn/install/installing-iis-85/installing-iis-85-on-windows-server-2012-r2

function InstallIIS()
{
    Write-Host "Installing IIS" -foregroundcolor Yellow
    $listFetures = Get-WindowsOptionalFeature -Online | where {$_.FeatureName -eq "IIS-ASPNET45"  -and $_.State -eq "Enabled"}

    if($listFetures.Count -eq 0) {
        Import-Module servermanager

        Write-Host "IIS is not installed. Installing..."   -foregroundcolor Blue -backgroundColor Yellow
        add-windowsfeature Web-Server, Web-WebServer, Web-Asp-Net45, Web-Mgmt-Tools, Web-Mgmt-Console, Web-Http-Errors, Web-Http-Redirect, Web-Default-Doc, Web-Http-Logging, Web-Stat-Compression, Web-Dyn-Compression, Web-Filtering, Web-ISAPI-Ext, Web-ISAPI-Filter, Web-Net-Ext45
    }
    else {
        Write-Host "IIS is installed already."   -foregroundcolor green
    }
}