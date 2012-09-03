"Executing create-filesystem-links.ps1"

# Global Variables
$currentDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$modulesDirectory = Join-Path $currentDirectory modules

# Imports
Import-Module (Join-Path $modulesDirectory utilities.ps1)

# Read system settings
[xml]$systemsettings = Get-SystemSettings

if ($systemsettings.Settings.FileSystemLinks -and $systemsettings.Settings.FileSystemLinks.BaseFolders -and $systemsettings.Settings.FileSystemLinks.Links)
{
	# Base folders
	foreach ($baseFolder in $systemsettings.Settings.FileSystemLinks.BaseFolders.BaseFolder)
	{
		$baseFolderPath = $baseFolder.Path
		if ((Test-Path $baseFolderPath) -eq $false)
		{
			"Creating base folder `"$baseFolderPath`"."
			New-Item $baseFolderPath -type directory -Force
		}
	}
	
	# Filesystem Links
	foreach ($link in $systemsettings.Settings.FileSystemLinks.Links.Link)
	{
		$linkType = $link.Type
		$linkPath = Join-Path $link.Path $link.Name
		$targetPath = $link.Target
		
		if (((Test-Path $linkPath) -eq $false) -and (Test-Path $targetPath))
		{
			"Creating a file-system link from `"$targetPath`" to `"$linkPath`" (link-type: $linkType)"
			switch ($linkType)
			{
				"Folder"
				{
					cmd /c mklink /J "$linkPath" "$targetPath"
					break
				}

				"File"
				{
					cmd /c mklink /H "$linkPath" "$targetPath"
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
}