. includes\iis-install-iis.ps1
. includes\iis-install-asp-net-core-bundles.ps1
. includes\iis-create-web-site.ps1
. includes\iis-create-app-pool.ps1
. includes\iis-create-web-application.ps1
. includes\site-configure-environment.ps1
. includes\iis-create-virtual-directory.ps1
. includes\file-system.ps1

InstallIIS
Import-Module WebAdministration

InstallAspNetCoreWindowsHosting

# CREATE EdugameCloud.Stage SITE    
CreateAppPool `
    -iisAppPoolName EdugameCloud.Stage `
    -iisAppPoolDotNetVersion "v4.0"

CreateWebSite `
    -siteName EdugameCloud.Stage `
    -siteFolderPath c:\inetpub\EdugameCloud.Stage `
    -iisAppPoolName EdugameCloud.Stage `
    -port 80

SetFolderPermissionsForUser `
    -directoryPath c:\inetpub\EdugameCloud.Stage `
    -userName "ESYNCTRAINING\Bamboo_LTI_stage" `
    -permissions "FullControl"

SetFolderReadPermissionsForUser `
    -directoryPath c:\inetpub\EdugameCloud.Stage `
    -appPoolName "EdugameCloud.Stage"
   
# CREATE api APPLICATION    
CreateAppPool `
    -iisAppPoolName EdugameCloud.Stage.Api `
    -iisAppPoolDotNetVersion "v4.0"
   
CreateWebApplication `
    -appName "EdugameCloud.Stage\api" `
    -folderPath C:\inetpub\EdugameCloud.Stage\api `
    -iisAppPoolName EdugameCloud.Stage.Api `

SetFolderReadPermissionsForUser `
    -directoryPath C:\inetpub\EdugameCloud.Stage\api `
    -appPoolName "EdugameCloud.Stage.Api"

SetFolderReadPermissionsForUser `
    -directoryPath C:\inetpub\EdugameCloud.Stage `
    -appPoolName "EdugameCloud.Stage.Api"
    

# CREATE lti APPLICATION    
CreateAppPool `
    -iisAppPoolName EdugameCloud.Lti.Stage `
    -iisAppPoolDotNetVersion "v4.0"
   
CreateWebApplication `
    -appName "EdugameCloud.Stage\lti" `
    -folderPath C:\inetpub\EdugameCloud.Stage\lti `
    -iisAppPoolName EdugameCloud.Lti.Stage `

SetFolderReadPermissionsForUser `
    -directoryPath C:\inetpub\EdugameCloud.Stage\lti `
    -appPoolName "EdugameCloud.Lti.Stage"

SetFolderReadPermissionsForUser `
    -directoryPath C:\inetpub\EdugameCloud.Stage `
    -appPoolName "EdugameCloud.Lti.Stage"


# CREATE lti-api APPLICATION
CreateAppPool `
    -iisAppPoolName EdugameCloud.LtiApi.Stage `
   
CreateWebApplication `
    -appName "EdugameCloud.Stage\lti-api" `
    -folderPath C:\inetpub\EdugameCloud.Stage\lti-api `
    -iisAppPoolName EdugameCloud.LtiApi.Stage `

SetFolderReadPermissionsForUser `
    -directoryPath C:\inetpub\EdugameCloud.Stage\lti-api `
    -appPoolName "EdugameCloud.LtiApi.Stage"


# CREATE lti-mp4 APPLICATION
CreateAppPool `
    -iisAppPoolName  EdugameCloud.Mp4.Stage `
   
CreateWebApplication `
    -appName "EdugameCloud.Stage\lti-mp4" `
    -folderPath  C:\inetpub\EdugameCloud.Stage\lti-mp4 `
    -iisAppPoolName  EdugameCloud.Mp4.Stage `

SetFolderReadPermissionsForUser `
    -directoryPath C:\inetpub\EdugameCloud.Stage\lti-mp4 `
    -appPoolName "EdugameCloud.Mp4.Stage"

# CREATE SERVICES APPLICATION
CreateAppPool `
    -iisAppPoolName  EdugameCloud.Stage.Wcf `
    -iisAppPoolDotNetVersion "v4.0"
   
CreateWebApplication `
    -appName "EdugameCloud.Stage\services" `
    -folderPath  C:\inetpub\EdugameCloud.Stage\services `
    -iisAppPoolName  EdugameCloud.Stage.Wcf `

SetFolderReadPermissionsForUser `
    -directoryPath C:\inetpub\EdugameCloud.Stage\services `
    -appPoolName "EdugameCloud.Stage.Wcf"

SetFolderReadPermissionsForUser `
    -directoryPath C:\inetpub\EdugameCloud.Stage `
    -appPoolName "EdugameCloud.Stage.Wcf"


# CREATE RESOURCES VIRTUAL DIRECTORY
CreateVirtualDirectory `
    -siteName EdugameCloud.Stage `
    -name resources `
    -folderPath C:\inetpub\EdugameCloud.Stage\Content\swf\admin\resources\


# CREATE EdugameCloud.Stage.CacheDependencies
$cacheDependenciesFolder = "C:\inetpub\EdugameCloud.Stage.CacheDependencies"

CreateFolder -folderPath C:\inetpub\EdugameCloud.Stage.CacheDependencies

SetFolderWritePermissionsForUser `
    -directoryPath $cacheDependenciesFolder `
    -appPoolName "EdugameCloud.Lti.Stage"

SetFolderWritePermissionsForUser `
    -directoryPath $cacheDependenciesFolder `
    -appPoolName "EdugameCloud.Stage.Wcf"

SetFolderWritePermissionsForUser `
    -directoryPath $cacheDependenciesFolder `
    -appPoolName "EdugameCloud.Stage"
    
SetFolderWritePermissionsForUser `
    -directoryPath $cacheDependenciesFolder `
    -appPoolName "EdugameCloud.LtiApi.Stage"


# CREATE EdugameCloud.Stage.FileStorage
$fileStorageFolderPath = "C:\inetpub\EdugameCloud.Stage.FileStorage"

CreateFolder -folderPath $fileStorageFolderPath

SetFolderWritePermissionsForUser `
    -directoryPath $fileStorageFolderPath `
    -appPoolName "EdugameCloud.Stage"

SetFolderWritePermissionsForUser `
    -directoryPath $fileStorageFolderPath `
    -appPoolName "EdugameCloud.Stage.Wcf"

# CREATE c:\inetpub\EdugameCloud.Stage\Content\swf\pub
CreateFolder -folderPath c:\inetpub\EdugameCloud.Stage\Content\swf\pub

SetFolderWritePermissionsForUser `
    -directoryPath c:\inetpub\EdugameCloud.Stage\Content\swf\pub `
    -appPoolName "EdugameCloud.Stage"


# SetEnvironment -webConfigPath "C:\inetpub\EdugameCloud.Stage\lti-api\web.config" -environment "Staging"
# SetEnvironment -webConfigPath "C:\inetpub\EdugameCloud.Stage\lti-mp4\web.config" -environment "Staging"

