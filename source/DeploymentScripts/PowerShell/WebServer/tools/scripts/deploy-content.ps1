"Executing deploy-content.ps1"

# Global Variables
$currentDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$modulesDirectory = Join-Path $currentDirectory modules
$toolsDirectory = (Get-Item $currentDirectory).parent.FullName
$packageDirectory = (Get-Item $toolsDirectory).Parent.FullName

# Imports
Import-Module (Join-Path $modulesDirectory utilities.ps1)

# Read system settings
[xml]$systemsettings = Get-SystemSettings

$targetDirectory = $systemsettings.Settings.DeploymentSettings.TargetFolder

# make sure the target directory exists
if((Test-Path $targetDirectory) -eq $false) {
	"Creating the target directory $targetDirectory"
	New-Item $targetDirectory -type directory
}

"Deploying content"
$contentDirectories = (Get-ChildItem "$packageDirectory\content\*") | Where-Object {$_.Mode -match "d"}
foreach ($directory in $contentDirectories)
{
	$sourceFolderPath = $directory.FullName
	$targetFolderPath = Join-Path $targetDirectory $directory.Name
	
	if((Test-Path $targetFolderPath))
	{
		"Removing existing folder $targetFolderPath"
		Remove-Item $targetFolderPath -Recurse -Force
	}
	
	"Copying $sourceFolderPath to $targetFolderPath"
    Copy-Item $sourceFolderPath -Destination $targetFolderPath -Recurse -Force
}