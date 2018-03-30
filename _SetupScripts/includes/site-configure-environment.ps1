
param(
	[String]$webConfigPath,
	[String]$environment = "Production"
)

. includes\config-transform.ps1

function SetEnvironment([string]$webConfigPath, [string]$environment)
{
    $doc = (Get-Content $webConfigPath) -as [Xml]

    Set-Environment -config $doc

    $doc.Save($webConfigPath)
}

