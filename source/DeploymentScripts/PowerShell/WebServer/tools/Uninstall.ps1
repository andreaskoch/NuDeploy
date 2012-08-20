# Uninstall this package
"Executing Uninstall.ps1"

$toolsDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$scriptsDirectory = Join-Path $toolsDirectory scripts


$uninstallExternalDependenciesScript = (Join-Path $scriptsDirectory uninstall-external-dependencies.ps1)
$removeFilesystemLinksScript = (Join-Path $scriptsDirectory remove-filesystem-links.ps1)
$removeContentScript = (Join-Path $scriptsDirectory remove-content.ps1)
$restoreSystemSettingsScript = (Join-Path $scriptsDirectory restore-system-settings.ps1)
$removeFileSystemPermissionsScript = (Join-Path $scriptsDirectory remove-filesystem-permissions.ps1)

"Restoring system settings"
& $restoreSystemSettingsScript

"Removing External Dependencies"
& $uninstallExternalDependenciesScript

"Removing filesystem permissions"
& $removeFileSystemPermissionsScript

"Removing filesystem links"
& $removeFilesystemLinksScript

"Removing content"
& $removeContentScript