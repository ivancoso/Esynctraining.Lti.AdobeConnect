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

   
# CREATE api APPLICATION    
Write-Host "EdugameCloud.$environmentAlias.Api"

CreateAppPool `
    -iisAppPoolName "EdugameCloud.$environmentAlias.Api" `
    -iisAppPoolDotNetVersion "v4.0"
   
CreateWebApplication `
    -appName "EdugameCloud.$environmentAlias\api" `
    -folderPath "C:\inetpub\EdugameCloud.$environmentAlias\api" `
    -iisAppPoolName "EdugameCloud.$environmentAlias.Api" `

SetFolderReadPermissionsForUser `
    -directoryPath "C:\inetpub\EdugameCloud.$environmentAlias\api" `
    -appPoolName "EdugameCloud.$environmentAlias.Api"

SetFolderReadPermissionsForUser `
    -directoryPath "C:\inetpub\EdugameCloud.$environmentAlias" `
    -appPoolName "EdugameCloud.$environmentAlias.Api"
    

# CREATE lti-mp4 APPLICATION
Write-Host "EdugameCloud.Mp4.$environmentAlias"

CreateAppPool `
    -iisAppPoolName  "EdugameCloud.Mp4.$environmentAlias" `
   
CreateWebApplication `
    -appName "EdugameCloud.$environmentAlias\lti-mp4" `
    -folderPath  "C:\inetpub\EdugameCloud.$environmentAlias\lti-mp4" `
    -iisAppPoolName  "EdugameCloud.Mp4.$environmentAlias" `

SetFolderReadPermissionsForUser `
    -directoryPath "C:\inetpub\EdugameCloud.$environmentAlias\lti-mp4" `
    -appPoolName "EdugameCloud.Mp4.$environmentAlias"



# CREATE EdugameCloud.$environmentAlias.CacheDependencies
$cacheDependenciesFolder = "C:\inetpub\EdugameCloud.$environmentAlias.CacheDependencies"

SetFolderWritePermissionsForUser `
    -directoryPath $cacheDependenciesFolder `
    -appPoolName "EdugameCloud.Lti.$environmentAlias"
    
