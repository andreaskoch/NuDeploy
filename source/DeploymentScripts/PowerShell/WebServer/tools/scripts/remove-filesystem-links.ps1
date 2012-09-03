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
		$linkType = $link.Type
		$linkPath = Join-Path $link.Path $link.Name
		$targetPath = $link.Target
		
		"Removing file-system link from `"$targetPath`" to `"$linkPath`" (link-type: $linkType)"
		switch ($linkType)
		{
			"Folder"
			{
				cmd /c rmdir "$linkPath"
				break
			}

			"File"
			{
				cmd /c del "$linkPath"
				break
			}

			default
			{
				"No link type specified."
				break
			}
		}
		
	}
}