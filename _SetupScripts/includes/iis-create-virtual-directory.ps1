function CreateVirtualDirectory(
	[string]$siteName,
    [string]$name,
	[string]$folderPath
)
{
	Write-Host "Creating Virtual Directory: $name" -foregroundcolor Yellow
	Write-Host "	site: $siteName" -foregroundcolor Green
	Write-Host "	folderPath: $folderPath" -foregroundcolor Green

	if (Test-Path IIS:\Sites\$siteName\$name)
	{
		Write-Host "Virtual Directory $name already exists." -foregroundcolor Green
	}
	else 
	{
		if (Test-Path $folderPath -PathType Container)
		{
			Write-Host "$folderPath folder already exists."  -foregroundcolor Yellow
		}
		else 
		{
			# CREATE FOLDER ON DISK
			Write-Host "Creating $folderPath folder." -foregroundcolor Blue -BackgroundColor Yellow
	 		New-Item $folderPath -type Directory
		}

		# CREATE VIRTUAL DIRECTORY
		Write-Host "Creating virtal directory $name with site $siteName. PhysicalPath is $folderPath" -foregroundcolor Blue -BackgroundColor Yellow

		New-Item IIS:\Sites\$siteName\$name -type VirtualDirectory -physicalPath $folderPath

		Write-Host "Virtal directory $name has been created" -foregroundcolor Green

	}
}

# EXAMPLE:

# CreateVirtualDirectory -siteName "Default Web Site" -name "DemoVirtualDir" -folderPath "c:\test\virtualDirectory1\"