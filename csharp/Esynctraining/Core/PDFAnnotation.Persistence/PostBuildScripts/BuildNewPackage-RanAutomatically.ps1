﻿#==========================================================
# This script is ran automatically after every successful build.
#
# This script creates a NuGet package for the current project, and places the .nupkg file in the project's output directory (beside the .dll/.exe file).
#
# You may edit the values of the $versionNumber, $releaseNotes, $packProperties, $additionalPackProperties, and $additionPackOptions variables below to adjust 
# the settings used to create the NuGet package.
#
# If you have modified this script:
#	- if you uninstall the "Create New NuGet Package From Project After Each Build" package, this file may not be removed automatically; you may need to manually delete it.
#	- if you update the "Create New NuGet Package From Project After Each Build" package, this file may not be updated unless you specify it to, either by confirming the 
#		overwrite if prompted, or by providing the "-FileConflictAction Overwrite" parameter when installing. Also, if you do this then your custom changes will be lost. 
#		It might be easiest to backup the file, uninstall the package, delete any left-over files, reinstall the package, and then re-apply your custom changes.
#==========================================================
param ([string]$ProjectFilePath, [string]$OutputDirectory, [string]$Configuration, [string]$Platform)

# Turn on Strict Mode to help catch syntax-related errors.
# 	This must come after a script's/function's param section.
# 	Forces a function to be the first non-comment code to appear in a PowerShell Module.
Set-StrictMode -Version Latest

# Get the directory that this script is in.
$THIS_SCRIPTS_DIRECTORY = Split-Path $script:MyInvocation.MyCommand.Path

# Make sure the OutputDirectory does not end in a trailing backslash, as it will mess up nuget.exe's parameter parsing.
$OutputDirectory = $OutputDirectory.TrimEnd('\')

#################################################
# Users May Edit The Following Variables.
#################################################

# Specify the version number to use for the NuGet package. If not specified the version number of the assembly being packed will be used.
$versionNumber = ""

# Specify any release notes for this package. 
# These will only be included in the package if you have a .nuspec file for the project in the same directory as the project file.
$releaseNotes = ""

# Make sure we pack the assemblies of the currently selected Configuration (e.g. Debug, Release) and Platform (e.g. x86, x64, Any CPU).
# If you want to force your NuGet package to always be packed with a specific Configuration and Platform, change "$Configuration" and "$Platform" below to the desired values.
#	e.g. $packProperties = "Configuration=""Release"";Platform=""Any CPU"";"
# However, be sure that you have built your project with the hard-coded Configuration and Platform, as we cannot pass -Build into the Pack Options.
$packProperties = "Configuration=""$Configuration"";Platform=""$Platform"";"

# Specify any additional NuGet Pack Properties to pass to MsBuild. e.g. "TargetFrameworkVersion=v3.5;Optimize=true"
# Do not specify the "Configuration" or "Platform" here (use $packProperties above).
# MsBuild Properties that can be specified: http://msdn.microsoft.com/en-us/library/vstudio/bb629394.aspx
$additionalPackProperties = ""

# Specify any additional NuGet Pack options to pass to nuget.exe.
# Do not specify a "-Version" (use $versionNumber above), "-OutputDirectory", or "-NonInteractive", as these are already provided.
# Do not specify any "-Properties" here; instead use the $additionalPackProperties variable above.
# Do not specify "-Build", as this may result in an infinite build loop.
$additionalPackOptions = ""

#################################################
# Do Not Edit Anything Past This Point.
#################################################

# Join the user's custom pack Properties with our default ones.
$packProperties = ($packProperties + $additionalPackProperties).TrimEnd(';')

#-----
# Make sure the assembly still exists in the folder specified by the project file, since if an Output Directory different than the one specified in 
# the project file was used (e.g. passed in using msbuild.exe's outdir property parameter, NuGet.exe won't be able to find the assembly file and pack it.

# Define helper functions.
function Get-XmlNamespaceManager([xml]$XmlDocument, [string]$NamespaceURI = "")
{
	# If a Namespace URI was not given, use the Xml document's default namespace.
	if ([string]::IsNullOrEmpty($NamespaceURI)) { $NamespaceURI = $XmlDocument.DocumentElement.NamespaceURI }	
	
	# In order for SelectSingleNode() to actually work, we need to use the fully qualified node path along with an Xml Namespace Manager, so set them up.
	[System.Xml.XmlNamespaceManager]$xmlNsManager = New-Object System.Xml.XmlNamespaceManager($XmlDocument.NameTable)
	$xmlNsManager.AddNamespace("ns", $NamespaceURI)
	return ,$xmlNsManager		# Need to put the comma before the variable name so that PowerShell doesn't convert it into an Object[].
}

function Get-FullyQualifiedXmlNodePath([string]$NodePath, [string]$NodeSeparatorCharacter = '.')
{
	return "/ns:$($NodePath.Replace($($NodeSeparatorCharacter), '/ns:'))"
}

function Get-XmlNode([xml]$XmlDocument, [string]$NodePath, [string]$NamespaceURI = "", [string]$NodeSeparatorCharacter = '.')
{
	$xmlNsManager = Get-XmlNamespaceManager -XmlDocument $XmlDocument -NamespaceURI $NamespaceURI
	[string]$fullyQualifiedNodePath = Get-FullyQualifiedXmlNodePath -NodePath $NodePath -NodeSeparatorCharacter $NodeSeparatorCharacter
	
	# Try and get the node, then return it. Returns $null if the node was not found.
	$node = $XmlDocument.SelectSingleNode($fullyQualifiedNodePath, $xmlNsManager)
	return $node
}

function Get-XmlNodes([xml]$XmlDocument, [string]$NodePath, [string]$NamespaceURI = "", [string]$NodeSeparatorCharacter = '.')
{
	$xmlNsManager = Get-XmlNamespaceManager -XmlDocument $XmlDocument -NamespaceURI $NamespaceURI
	[string]$fullyQualifiedNodePath = Get-FullyQualifiedXmlNodePath -NodePath $NodePath -NodeSeparatorCharacter $NodeSeparatorCharacter

	# Try and get the nodes, then return them. Returns $null if no nodes were found.
	$nodes = $XmlDocument.SelectNodes($fullyQualifiedNodePath, $xmlNsManager)
	return $nodes
}

function Get-XmlElementsTextValue([xml]$XmlDocument, [string]$ElementPath, [string]$NamespaceURI = "", [string]$NodeSeparatorCharacter = '.')
{
	# Try and get the node.	
	$node = Get-XmlNode -XmlDocument $XmlDocument -NodePath $ElementPath -NamespaceURI $NamespaceURI -NodeSeparatorCharacter $NodeSeparatorCharacter
	
	# If the node already exists, return its value, otherwise return null.
	if ($node) { return $node.InnerText } else { return $null }
}

# Makes sure the assembly exists in the directory defined by the Project File (this is where NuGet.exe will expect it to be in order to pack it).
# This is required in case the user is building with MsBuild and has provided an alternative output directory (e.g. /p:OutDir="Some\Other\Path").
function Ensure-AssemblyFileExistsWhereNuGetExpectsItToBe([string]$ProjectFilePath, [string]$OutputDirectory, [string]$Configuration, [string]$Platform)
{
	# Display the time that the pre-processing started running.
	$scriptStartTime = Get-Date
	Write-Output "Pre-processing the project file to make sure the assembly exists where NuGet.exe will expect it to be, started at $($scriptStartTime.TimeOfDay.ToString())."

	# If the Project File Path does not exist, display an error message and return.
	if (!(Test-Path $ProjectFilePath)) { Write-Output "Project file does not exist at '$ProjectFilePath', so cannot pre-process it."; return }
	
	# Get the contents of the Project File as Xml.
	$projectFileXml = New-Object System.Xml.XmlDocument
	$projectFileXml.Load($ProjectFilePath)

	# Get the Property Group for the current Configuration and Platform from the Project File.
	$projectFilePropertyGroups = Get-XmlNodes -XmlDocument $projectFileXml -NodePath "Project.PropertyGroup"
	if (!$projectFilePropertyGroups) { "No PropertyGroup elements could be found in the project file, so cannot pre-process it."; return }
	$projectFilePropertyGroupForCurrentConfigurationAndPlatform = $projectFilePropertyGroups | Where { $_.Attributes.GetNamedItem('Condition') -and $_.Attributes.GetNamedItem('Condition').Value -match ".*$Configuration\|$Platform.*" }
	if (!$projectFilePropertyGroupForCurrentConfigurationAndPlatform) { "Could not find the PropertyGroup element in the project file that corresponds to Configuration '$Configuration' and Platform '$Platform', so cannot pre-process it."; return }

	# Get the Directory where NuGet.exe will expect to find the assembly to pack.
	$projectFileOutputDirectory = $projectFilePropertyGroupForCurrentConfigurationAndPlatform.OutputPath

	# If we were not able to get the Output Directory where NuGet.exe will expect to find the assembly from the Project File, display an error message and return.
	if ([string]::IsNullOrEmpty($projectFileOutputDirectory)) { Write-Output "Could not find the OutputPath element in the project file that corresponds to Configuration '$Configuration' and Platform '$Platform', so cannot pre-process it."; return }
	
	# Get the full path of the directory where NuGet.exe will expect to find the assembly.
	$nuGetExpectedOutputDirectoryPath = Join-Path (Split-Path -Path $ProjectFilePath -Parent) $projectFileOutputDirectory
	$nuGetExpectedOutputDirectoryPath = $nuGetExpectedOutputDirectoryPath.TrimEnd('\')	# We trimmed the OutputDirectory, so trim this one too so we can compare them to see if they match.

	# If the actual Output Directory is different than the one specified in the Project File.
	if (!$OutputDirectory.Equals($nuGetExpectedOutputDirectoryPath, [System.StringComparison]::InvariantCultureIgnoreCase))
	{
		# Get the name of the assembly.
		$assemblyName = Get-XmlElementsTextValue -XmlDocument $projectFileXml -ElementPath "Project.PropertyGroup.AssemblyName"
		if (!$assemblyName) { $assemblyName = [string]::Empty }
		
		# Get the type of project being built, so we can determine what file extension it should have.
		$assemblyType = Get-XmlElementsTextValue -XmlDocument $projectFileXml -ElementPath "Project.PropertyGroup.OutputType"
		if (!$assemblyType) { $assemblyType = [string]::Empty }
		
		# Attach the file extension to the assembly name based on the type of project this is. Either a Library or Executable.
		if ($assemblyType.Equals("Library", [System.StringComparison]::InvariantCultureIgnoreCase)) { $assemblyName = "$assemblyName.dll" }
		else { $assemblyName = "$assemblyName.exe" }

		# Get the full paths of the assembly.
		$assemblyPath = Join-Path $OutputDirectory $assemblyName
		$nuGetExpectedAssemblyPath = Join-Path $nuGetExpectedOutputDirectoryPath $assemblyName

		# If the assembly is not in the Output Directory (which is should be), display an error message and return.
		if (!(Test-Path $assemblyPath -PathType Leaf)) { Write-Output "Could not find the assembly at the expected path '$assemblyPath', so cannot continue pre-processing."; return }
		
		# Make sure the assembly exists in the Project File's Output Path, since that is where NuGet.exe expects to find it.
		# If the assembly does not exist where the Project File defines it should be (i.e. where NuGet expects it to be), copy it there, 
		# OR the assembly exists in both places, but If the one in the Output Directory is newer, overwrite the one in the Project Output Directory.
		if (!(Test-Path $nuGetExpectedAssemblyPath -PathType Leaf) -or 
			((Get-Item -Path $assemblyPath).LastWriteTime -lt (Get-Item -Path $nuGetExpectedAssemblyPath).LastWriteTime))
		{
			# If the directory to hold the assembly file does not exist, create it.
			$nuGetExpectedAssemblyDirectoryPath = Split-Path $nuGetExpectedAssemblyPath -Parent
			if (!(Test-Path $nuGetExpectedAssemblyDirectoryPath)) { New-Item -Path $nuGetExpectedAssemblyDirectoryPath -ItemType Container -Force > $null }
		
			# Copy the assembly file.
			Write-Output "Copying assembly file from '$assemblyPath' to '$nuGetExpectedAssemblyPath'." 
			Copy-Item -Path $assemblyPath -Destination $nuGetExpectedAssemblyPath -Force
			
			# Copy the Pdb file, if it exists.
			$assemblyPdbPath = [System.IO.Path]::ChangeExtension($assemblyPath, "pdb")
			$nuGetExpectedAssemblyPdbPath = [System.IO.Path]::ChangeExtension($nuGetExpectedAssemblyPath, "pdb")
			if (Test-Path $assemblyPdbPath -PathType Leaf)
			{ 
				Write-Output "Copying symbols file from '$assemblyPdbPath' to '$nuGetExpectedAssemblyPdbPath'." 
				Copy-Item -Path $assemblyPdbPath -Destination $nuGetExpectedAssemblyPdbPath -Force
			}
			else { Write-Output "No symbols file found at '$assemblyPdbPath', so it was not copied to '$nuGetExpectedAssemblyPdbPath'." }
			
			# Copy the Xml file, if it exists.
			# The Xml file location and name are specified in the Project File, but NuGet.exe seems to ignore that and expects it to be in the same directory as the assembly,
			# with the same name as the assembly, so just put it where NuGet.exe expects it to be.
			$assemblyXmlPath = [System.IO.Path]::ChangeExtension($assemblyPath, "xml")
			$nuGetExpectedAssemblyXmlPath = [System.IO.Path]::ChangeExtension($nuGetExpectedAssemblyPath, "xml")
			if (Test-Path $assemblyXmlPath -PathType Leaf)
			{ 
				Write-Output "Copying documentation file from '$assemblyXmlPath' to '$nuGetExpectedAssemblyXmlPath'." 
				Copy-Item -Path $assemblyXmlPath -Destination $nuGetExpectedAssemblyXmlPath -Force
			}
			else { Write-Output "No documentation file found at '$assemblyXmlPath', so it was not copied to '$nuGetExpectedAssemblyXmlPath'." }
		}
		else { Write-Output "The proper assembly already exists where NuGet.exe will expect it to be, so no pre-processing actions were required." }
	}
	else { Write-Output "The Output Directory is the same as defined in the project file, so no pre-processing actions were required." }

	# Display the time that the pre-processing finished running, and how long it took to run.
	$scriptFinishTime = Get-Date
	$scriptElapsedTimeInSeconds = ($scriptFinishTime - $scriptStartTime).TotalSeconds.ToString()
	Write-Output "Pre-processing the project file finished running at $($scriptFinishTime.TimeOfDay.ToString()). Completed in $scriptElapsedTimeInSeconds seconds."
}

# Process the Project File and make sure the assembly exists where NuGet.exe will expect it to be.
Ensure-AssemblyFileExistsWhereNuGetExpectsItToBe -ProjectFilePath $ProjectFilePath -OutputDirectory $OutputDirectory -Configuration $Configuration -Platform $Platform
#-----

# Create the new NuGet package.
& "$THIS_SCRIPTS_DIRECTORY\New-NuGetPackage.ps1" -ProjectFilePath "$ProjectFilePath" -VersionNumber $versionNumber -ReleaseNotes $releaseNotes -PackOptions "-OutputDirectory ""$OutputDirectory"" -Properties $packProperties -NonInteractive $additionalPackOptions" -DoNotUpdateNuSpecFile -NoPrompt -Verbose