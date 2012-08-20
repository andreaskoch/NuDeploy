"Executing install-external-dependencies.ps1"

# Global Variables
$currentDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$toolsDirectory = (Get-Item $currentDirectory).parent.FullName
$externalDependenciesDirectory = Join-Path $toolsDirectory external-dependencies

if (Test-Path $externalDependenciesDirectory)
{
	$installScripts = (Get-ChildItem "$externalDependenciesDirectory\*\install.ps1")
	foreach($installScript in $installScripts)
	{
		$installScriptPath = $installScript.FullName
		$installScriptDirectoryName = (Get-item $installScriptPath).Directory.Name
		
		"Installing external dependency '$installScriptDirectoryName'"
		& $installScriptPath
	}
}