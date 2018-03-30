function SetFolderReadPermissionsForUser([string]$directoryPath, [string]$appPoolName) {

    Write-Host "SetFolderWritePermissionsForUser - $directoryPath - start" -foregroundcolor green

    $colRights = [System.Security.AccessControl.FileSystemRights]"ReadAndExecute" 
 
    $InheritanceFlag = [System.Security.AccessControl.InheritanceFlags]"ContainerInherit,ObjectInherit"
    $PropagationFlag = [System.Security.AccessControl.PropagationFlags]::None
 
    $objType =[System.Security.AccessControl.AccessControlType]::Allow 

    $objUser = New-Object System.Security.Principal.NTAccount("IIS APPPOOL\$appPoolName") 
 
    $objACE = New-Object System.Security.AccessControl.FileSystemAccessRule($objUser, $colRights, $InheritanceFlag, $PropagationFlag, $objType) 

    $UNCPath = $directoryPath

    $objACL = Get-ACL $UNCPath
    
    $objACL.AddAccessRule($objACE) 
 
    Set-ACL $UNCPath $objACL

    Write-Host "SetFolderReadPermissionsForUser - $directoryPath - end" -foregroundcolor green
}

function Set-RemoteReadWritePermissionsForUser($computerName, $userName, $password, [string]$directoryPath, [string]$appPoolName)
{
    $secpasswd = ConvertTo-SecureString $password -AsPlainText -Force
    $creds = New-Object System.Management.Automation.PSCredential ($userName, $secpasswd)

    Invoke-Command -ComputerName $computerName -Credential $creds -ScriptBlock ${function:SetFolderReadPermissionsForUser} -ArgumentList $directoryPath,$appPoolName
}