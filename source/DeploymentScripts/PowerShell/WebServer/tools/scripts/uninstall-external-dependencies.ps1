"Executing uninstall-external-dependencies.ps1"

# Global Variables
$currentDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$toolsDirectory = (Get-Item $currentDirectory).parent.FullName

$externalDependenciesDirectory = Join-Path $toolsDirectory external-dependencies

if (Test-Path $externalDependenciesDirectory)
{
	$uninstallScripts = (Get-ChildItem "$externalDependenciesDirectory\*\uninstall.ps1")
	foreach($uninstallScript in $uninstallScripts) {
		
		$uninstallScriptPath = $uninstallScript.FullName
		$uninstallScriptDirectoryName = (Get-item $uninstallScriptPath).Directory.Name
		
		"Un-Installing external dependency '$uninstallScriptDirectoryName'"
		& $uninstallScriptPath
	}
}