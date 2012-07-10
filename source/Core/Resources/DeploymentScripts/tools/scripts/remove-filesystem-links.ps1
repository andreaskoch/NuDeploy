"Executing remove-filesystem-links.ps1"

# Global Variables
$currentDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$modulesDirectory = Join-Path $currentDirectory modules

# Imports
Import-Module (Join-Path $modulesDirectory utilities.ps1)

# Read system settings
[xml]$systemsettings = Get-SystemSettings

if ($systemsettings.Settings.FileSystemLinks -and $systemsettings.Settings.FileSystemLinks.BaseFolders -and $systemsettings.Settings.FileSystemLinks.Links)
{	
	# Filesystem Links
	foreach ($link in $systemsettings.Settings.FileSystemLinks.Links.Link)
	{
		$linkPath = Join-Path $link.Path $link.Name
		$targetPath = $link.Target
		
		"Removing file-system link from `"$targetPath`" to `"$linkPath`""
		cmd /c rmdir "$linkPath"
	}
}