# Remove
"Executing Remove.ps1"

$packageDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$toolsDirectory = Join-Path $packageDirectory tools

# Un-Install
$initScript = Get-ChildItem (Join-Path $toolsDirectory Init.ps1)
$uninstallScript = Get-ChildItem (Join-Path $toolsDirectory Uninstall.ps1)

"Executing the init script ($initScript)"
& $initScript

"Executing the un-install script ($uninstallScript)"
& $uninstallScript