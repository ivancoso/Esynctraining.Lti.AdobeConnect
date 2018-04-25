param([string]$environmentAlias = "Stage")

. includes\iis-install-iis.ps1
. includes\iis-install-asp-net-core-bundles.ps1
. includes\iis-create-web-site.ps1
. includes\iis-create-app-pool.ps1
. includes\iis-create-web-application.ps1
. includes\site-configure-environment.ps1
. includes\iis-create-virtual-directory.ps1
. includes\file-system.ps1
. includes\wcf-install-wcf.ps1

InstallIIS
Import-Module WebAdministration

InstallAspNetCoreWindowsHosting
InstallWCF

# CREATE EdugameCloud.$environmentAlias SITE   
Write-Host "EdugameCloud.$environmentAlias"

CreateAppPool `
    -iisAppPoolName "EdugameCloud.$environmentAlias" `
    -iisAppPoolDotNetVersion "v4.0"

CreateWebSite `
    -siteName "EdugameCloud.$environmentAlias" `
    -siteFolderPath "c:\inetpub\EdugameCloud.$environmentAlias" `
    -iisAppPoolName "EdugameCloud.$environmentAlias" `
    -port 80

SetFolderReadPermissionsForUser `
    -directoryPath "c:\inetpub\EdugameCloud.$environmentAlias" `
    -appPoolName "EdugameCloud.$environmentAlias"
      

# CREATE lti APPLICATION    
Write-Host "EdugameCloud.Lti.$environmentAlias"

CreateAppPool `
    -iisAppPoolName "EdugameCloud.Lti.$environmentAlias" `
    -iisAppPoolDotNetVersion "v4.0"
   
CreateWebApplication `
    -appName "EdugameCloud.$environmentAlias\lti" `
    -folderPath "C:\inetpub\EdugameCloud.$environmentAlias\lti" `
    -iisAppPoolName "EdugameCloud.Lti.$environmentAlias" `
    -siteName "EdugameCloud.$environmentAlias" `

SetFolderReadPermissionsForUser `
    -directoryPath "C:\inetpub\EdugameCloud.$environmentAlias\lti" `
    -appPoolName "EdugameCloud.Lti.$environmentAlias"

SetFolderReadPermissionsForUser `
    -directoryPath "C:\inetpub\EdugameCloud.$environmentAlias" `
    -appPoolName "EdugameCloud.Lti.$environmentAlias"


# CREATE lti-api APPLICATION
Write-Host "EdugameCloud.LtiApi.$environmentAlias"

CreateAppPool `
    -iisAppPoolName "EdugameCloud.LtiApi.$environmentAlias" `
   
CreateWebApplication `
    -appName "EdugameCloud.$environmentAlias\lti-api" `
    -folderPath "C:\inetpub\EdugameCloud.$environmentAlias\lti-api" `
    -iisAppPoolName "EdugameCloud.LtiApi.$environmentAlias" `
    -siteName "EdugameCloud.$environmentAlias" `

SetFolderReadPermissionsForUser `
    -directoryPath "C:\inetpub\EdugameCloud.$environmentAlias\lti-api" `
    -appPoolName "EdugameCloud.LtiApi.$environmentAlias"


# CREATE SERVICES APPLICATION
Write-Host "EdugameCloud.$environmentAlias.Wcf"

CreateAppPool `
    -iisAppPoolName  "EdugameCloud.$environmentAlias.Wcf" `
    -iisAppPoolDotNetVersion "v4.0"
   
CreateWebApplication `
    -appName "EdugameCloud.$environmentAlias\services" `
    -folderPath  "C:\inetpub\EdugameCloud.$environmentAlias\services" `
    -iisAppPoolName  "EdugameCloud.$environmentAlias.Wcf" `
    -siteName "EdugameCloud.$environmentAlias" `

SetFolderReadPermissionsForUser `
    -directoryPath "C:\inetpub\EdugameCloud.$environmentAlias\services" `
    -appPoolName "EdugameCloud.$environmentAlias.Wcf"

SetFolderReadPermissionsForUser `
    -directoryPath "C:\inetpub\EdugameCloud.$environmentAlias" `
    -appPoolName "EdugameCloud.$environmentAlias.Wcf"


# CREATE RESOURCES VIRTUAL DIRECTORY
CreateVirtualDirectory `
    -siteName "EdugameCloud.$environmentAlias" `
    -name resources `
    -folderPath "C:\inetpub\EdugameCloud.$environmentAlias\Content\swf\admin\resources\"


# CREATE EdugameCloud.$environmentAlias.CacheDependencies
$cacheDependenciesFolder = "C:\inetpub\EdugameCloud.$environmentAlias.CacheDependencies"

CreateFolder -folderPath "C:\inetpub\EdugameCloud.$environmentAlias.CacheDependencies"

SetFolderWritePermissionsForUser `
    -directoryPath $cacheDependenciesFolder `
    -appPoolName "EdugameCloud.Lti.$environmentAlias"

SetFolderWritePermissionsForUser `
    -directoryPath $cacheDependenciesFolder `
    -appPoolName "EdugameCloud.$environmentAlias.Wcf"

SetFolderWritePermissionsForUser `
    -directoryPath $cacheDependenciesFolder `
    -appPoolName "EdugameCloud.$environmentAlias"
    
SetFolderWritePermissionsForUser `
    -directoryPath $cacheDependenciesFolder `
    -appPoolName "EdugameCloud.LtiApi.$environmentAlias"


# CREATE EdugameCloud.$environmentAlias.FileStorage
$fileStorageFolderPath = "C:\inetpub\EdugameCloud.$environmentAlias.FileStorage"

CreateFolder -folderPath $fileStorageFolderPath

SetFolderPermissionsForUser -directoryPath $fileStorageFolderPath `
    -$userName "IIS APPPOOL\EdugameCloud.$environmentAlias" `
    -permissions "Modify, Read, ReadAndExecute, Write"

SetFolderWritePermissionsForUser `
    -directoryPath $fileStorageFolderPath `
    -appPoolName "EdugameCloud.$environmentAlias.Wcf"

# CREATE c:\inetpub\EdugameCloud.$environmentAlias\Content\swf\pub
CreateFolder -folderPath "c:\inetpub\EdugameCloud.$environmentAlias\Content\swf\pub"

SetFolderWritePermissionsForUser `
    -directoryPath "c:\inetpub\EdugameCloud.$environmentAlias\Content\swf\pub" `
    -appPoolName "EdugameCloud.$environmentAlias"


# SET ENVIRONMENT FOR WEB.CONFOG IN ASP CORE PROJECT
$environmentName = ""

switch ($environmentAlias) {
    "Stage" { $environmentName = "Staging" }
    "Dev"   { $environmentName = "Development" }
    "Prod"  { $environmentName = "Production" }
    Default {
        Write-Host "$environmentName Environment alias is not correct. Please check input variable environmentName. It should have only values: [Stage, Dev, Prod]"   -foregroundcolor red
    }
}

# COPY BINARIES
if ((Test-Path "..\Binaries"))
{
    Copy-Item -Force "..\Binaries\LTI\*" -Destination "c:\inetpub\EdugameCloud.$environmentAlias" -Recurse

    SetEnvironment -webConfigPath "C:\inetpub\EdugameCloud.$environmentAlias\lti-api\web.config" -environment $environmentName
    SetEnvironment -webConfigPath "C:\inetpub\EdugameCloud.$environmentAlias\lti-mp4\web.config" -environment $environmentName    
}
else 
{
    Write-Host "Binaries folder is missed." -foregroundcolor red
}


