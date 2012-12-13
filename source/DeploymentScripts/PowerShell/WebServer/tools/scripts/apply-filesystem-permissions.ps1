"Executing apply-filesystem-permissions.ps1"

# Global Variables
$currentDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$modulesDirectory = Join-Path $currentDirectory modules

# Imports
. (Join-Path $modulesDirectory utilities.ps1)
. (Join-Path $modulesDirectory filesystem-permission-management.ps1)

# Read system settings
[xml]$systemsettings = Get-SystemSettings

if ($systemsettings.Settings.FileSystemPermissions)
{
	if ($systemsettings.Settings.FileSystemPermissions.FolderPermissions -and $systemsettings.Settings.FileSystemPermissions.FolderPermissions.FolderPermission)
	{
		"Assigning folder permissions"
		foreach($folderPermission in $systemsettings.Settings.FileSystemPermissions.FolderPermissions.FolderPermission)
		{
			"Assigning permissions to folder $($folderPermission.Path)"
			if ((Test-Path $folderPermission.Path) -eq $false)
			{
				"The folder $($folderPermission.Path) does not exist. Creating it now."
				New-Item $folderPermission.Path -Type directory -Force
			}
			
			Add-FolderAccess -Path $folderPermission.Path -Access $folderPermission.User -Permission $folderPermission.Permissions
		}
	}
}