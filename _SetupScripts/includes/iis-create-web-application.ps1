function CreateWebApplication(
	[string]$appName,
	[string]$folderPath,
	[string]$iisAppPoolName
)
{
	Write-Host "Creating Application: $appName" -foregroundcolor Yellow
	Write-Host "	Folder: $folderPath" -foregroundcolor Green
	Write-Host "	AppPool: $iisAppPoolName" -foregroundcolor Green
	
	if (Test-Path IIS:\Sites\$appName)
	{
		Write-Host "Application $appName already exists." -foregroundcolor Green
	}
	else
	{
		if ((Test-Path $folderPath -pathType container))
		{
			Write-Host "$folderPath folder already exists."  -foregroundcolor Yellow
		}
		else
		{
			# CREATE FOLDER ON DISK
			Write-Host "Creating $folderPath folder." -foregroundcolor Blue -BackgroundColor Yellow
			New-Item $folderPath -type Directory
		}
		# CREATE WEB SITE
		Write-Host "Creating application $appName with app-pool $iisAppPoolName. PhysicalPath is $folderPath" -foregroundcolor Blue -BackgroundColor Yellow
		
		New-Item IIS:\Sites\$appName -physicalPath $folderPath -type Application
		Set-ItemProperty IIS:\Sites\$appName -name applicationPool -value $iisAppPoolName

        #Set read permission for site folder.
        SetFolderReadPermissionsForUser -directoryPath $folderPath -appPoolName $iisAppPoolName
        Write-Host "Set read permission for $folderPath folder" -foregroundcolor Blue -backgroundColor Yellow

        #Set write permission for Logs
        $logsPath = $folderPath + "\Logs"
        if (!(Test-Path $logsPath))
        {
            New-Item $logsPath -type Directory
        }

        SetFolderWritePermissionsForUser -directoryPath $logsPath -appPoolName $iisAppPoolName
        Write-Host "Set write permission for $folderPath\Logs folder" -foregroundcolor Blue -backgroundColor Yellow

		Write-Host "Application $appName has been created" -foregroundcolor Green
	}
}

# EXAMPLE:

# CreateWebApplication -appName "ce\api" -folderPath "d:\Development\esync\clients\CEC\csharp\trunk\Powershell\" -iisAppPoolName "ce"