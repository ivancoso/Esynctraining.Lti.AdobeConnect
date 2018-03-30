function InstallAspNetCoreWindowsHosting()
{
    Write-Host "Installing ASP.NET Core Windows Hosting Bundle" -foregroundcolor Yellow
    $downloadUrl = 'https://download.microsoft.com/download/1/1/0/11046135-4207-40D3-A795-13ECEA741B32/DotNetCore.2.0.5-WindowsHosting.exe';
    $dotNETCoreUpdatesPath = "Registry::HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Updates\.NET Core" 
    $notInstalled = $true

    $microsoftUpdatesPath = "Registry::HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Updates";
    $notExistDotNetCoreBranch = $true;


    $updates = Get-ChildItem $microsoftUpdatesPath;
    $updates
    $updates | where {$_.Name -Match ".NET Core"} | ForEach-Object {
        Write-Host ".Net core runtime is installed." -foregroundcolor Green
        $notExistDotNetCoreBranch = $false;
    }

    if ($notExistDotNetCoreBranch -eq $false)
    {
        $dotNetCoreItems = Get-Item -ErrorAction Stop -Path $dotNETCoreUpdatesPath 
        $dotNetCoreItems.GetSubKeyNames() | Where { $_ -Match "Microsoft .NET Core 2.0.*Windows Server Hosting" } | ForEach-Object { 
            $notInstalled = $false 
            Write-Host "The host has installed $_" -foregroundcolor Green
        }
    }

    if ($notInstalled) {
        Write-Host ".Net core hosting module is not installed. Installing..." -foregroundcolor Green
        
        $file = (Get-Item -Path ".\" -Verbose).FullName + '\DotNetCore-WindowsHosting.exe';
        Write-Host "Downloading installer to: " + $file -foregroundcolor Blue -backgroundColor Yellow

        $downloader = New-Object System.Net.WebClient
        $downloader.DownloadFile($downloadUrl, $file);
        Write-Host $downloadUrl "is downloaded. Executing with '/quiet /install' args..." -foregroundcolor Blue -backgroundColor Yellow

        $parms = "OPT_INSTALL_LTS_REDIST=1 OPT_INSTALL_FTS_REDIST=1 OPT_NO_X86=1 /quiet /install"
        $prms = $parms.Split(" ")
        & "$file" $prms
        
        Write-Host "Installed. Restarting W3SVC..." -foregroundcolor Blue -backgroundColor Yellow
    }
}