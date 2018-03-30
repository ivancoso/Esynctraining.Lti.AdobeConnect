function CreateFolder(
    [string]$folderPath
)
{
	Write-Host "Creating Directory: $folderPath" -foregroundcolor Yellow

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

	Write-Host "Folder $folderPath has been created" -foregroundcolor Green
}
# EXAMPLE:
# CreateDirectory -folderPath "c:\test\virtualDirectory1\"

function SetFolderPermissionsForUser([string]$directoryPath, [string]$userName, [string]$permissions) {

    Write-Host "Set $permissions permission(s) for $directoryPath folder for $userName user - start" -foregroundcolor green

    $colRights = [System.Security.AccessControl.FileSystemRights] $permissions 
 
    $InheritanceFlag = [System.Security.AccessControl.InheritanceFlags]"ContainerInherit,ObjectInherit"
    $PropagationFlag = [System.Security.AccessControl.PropagationFlags]::None
 
    $objType =[System.Security.AccessControl.AccessControlType]::Allow 

    $objUser = New-Object System.Security.Principal.NTAccount("$userName") 
 
    $objACE = New-Object System.Security.AccessControl.FileSystemAccessRule($objUser, $colRights, $InheritanceFlag, $PropagationFlag, $objType) 

    $UNCPath = $directoryPath

    $objACL = Get-ACL $UNCPath
    
    $objACL.AddAccessRule($objACE) 
 
    Set-ACL $UNCPath $objACL

    Write-Host "Set $permissions permission(s) for $directoryPath folder for $userName user - end" -foregroundcolor green
}

# EXAMPLE:
# SetFolderPermissionsForUser -directoryPath "c:\test\virtualDirectory1\" -$userName "esynctraining\Test_User" -permissions "FullControl, Modify, Read, ReadAndExecute, Write"