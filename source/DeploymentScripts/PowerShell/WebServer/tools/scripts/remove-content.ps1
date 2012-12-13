"Executing remove-content.ps1"

# Global Variables
$currentDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$modulesDirectory = Join-Path $currentDirectory modules
$toolsDirectory = (Get-Item $currentDirectory).parent.FullName
$packageDirectory = (Get-Item $toolsDirectory).Parent.FullName

# Imports
. (Join-Path $modulesDirectory utilities.ps1)

# Read system settings
[xml]$systemsettings = Get-SystemSettings

$targetDirectory = $systemsettings.Settings.DeploymentSettings.TargetFolder

"Removing content"
if((Test-Path $targetDirectory) -eq $true)
{
	$contentDirectories = (Get-ChildItem "$packageDirectory\content\*") | Where-Object {$_.Mode -match "d"}
	foreach ($directory in $contentDirectories)
	{
		$sourceFolderPath = $directory.FullName
		$targetFolderPath = Join-Path $targetDirectory $directory.Name
		
		if((Test-Path $targetFolderPath))
		{
			"Removing folder $targetFolderPath"
			Remove-Item -Recurse -Force $targetFolderPath
		}
	}
	
	# Remove target directory
	"Removing the target directory $targetDirectory. Started."
	Remove-Item $targetDirectory -Recurse -Force
	"Removing the target directory $targetDirectory. Finished."	
}
else
{
	"Cannot remove target directory $targetDirectory because it does not exist."
}