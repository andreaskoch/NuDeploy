"Executing remove-filesystem-permissions.ps1"

# Global Variables
$currentDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$modulesDirectory = Join-Path $currentDirectory modules

# Imports
Import-Module (Join-Path $modulesDirectory utilities.ps1)
Import-Module (Join-Path $modulesDirectory filesystem-permission-management.ps1)

# Read system settings
[xml]$systemsettings = Get-SystemSettings

if ($systemsettings.Settings.FileSystemPermissions)
{
	if ($systemsettings.Settings.FileSystemPermissions.FolderPermissions -and $systemsettings.Settings.FileSystemPermissions.FolderPermissions.FolderPermission)
	{
		"Removing folder permissions"
		foreach($folderPermission in $systemsettings.Settings.FileSystemPermissions.FolderPermissions.FolderPermission)
		{
			"Removing permissions for user $($folderPermission.User) from folder $($folderPermission.Path)"
			Remove-FolderAccess -Path $folderPermission.Path -Access $folderPermission.User
		}
	}
}		