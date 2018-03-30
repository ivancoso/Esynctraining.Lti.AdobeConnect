. "includes\set-folder-read-permissions.ps1"
. "includes\set-folder-write-permissions.ps1"

function CreateWebSite(
	[string]$siteName = '',
	[string]$siteFolderPath = "",
	[string]$iisAppPoolName = '',
	[Int32]$port=80
)
{
	Write-Host "Creating WebSiteName: $siteName" -foregroundcolor Yellow
	Write-Host "	WebSiteName: $siteName" -foregroundcolor Green
	Write-Host "	SiteFolderPhysicalPath: $siteFolderPath" -foregroundcolor Green
	Write-Host "	AppPoolName: $iisAppPoolName" -foregroundcolor Green

	if ((Test-Path $siteFolderPath -pathType container))
	{
		Write-Host "$siteFolderPath folder already exists."  -foregroundcolor Yellow
	}
	else
	{
		# CREATE FOLDER ON DISK
		Write-Host "Creating $siteFolderPath folder." -foregroundcolor Blue	-backgroundColor Yellow
		New-Item $siteFolderPath -type Directory
	}

	if (Test-Path IIS:\Sites\$siteName)
	{
		Write-Host "Website $siteName already exists." -foregroundcolor Yellow
	}
	else
	{
		# CREATE WEB SITE
		Write-Host "Creating web site $siteName with app-pool $Get-ExecutionPolicy. PhysicalPath is $siteFolderPath" -foregroundcolor Blue -backgroundColor Yellow

		New-Item IIS:\Sites\$siteName -physicalPath $siteFolderPath -bindings @{protocol="http";bindingInformation=":" + $port.ToString() +  ":"}
		Set-ItemProperty IIS:\Sites\$siteName -name applicationPool -value $iisAppPoolName	

		Write-Host "Web site $siteName has been created" -foregroundcolor Yellow
	}

    #Set read permission for site folder.
    SetFolderReadPermissionsForUser -directoryPath $siteFolderPath -appPoolName $iisAppPoolName

    Write-Host "Set read permission for $siteFolderPath folder" -foregroundcolor Blue -backgroundColor Yellow

    #Set write permission for Logs
    Write-Host "Set write permission for $siteFolderPath\Logs folder" -foregroundcolor Blue -backgroundColor Yellow
    $logsPath = $siteFolderPath + "\Logs"
    if (!(Test-Path $logsPath))
    {
        New-Item $logsPath -type Directory
    }

    SetFolderWritePermissionsForUser -directoryPath $logsPath -appPoolName $iisAppPoolName 
}