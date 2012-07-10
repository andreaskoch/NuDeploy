# Install this package
param(
    [Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
    [string] $DeploymentType
)

"Executing Install.ps1 -DeploymentType $DeploymentType"

$toolsDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$scriptsDirectory = Join-Path $toolsDirectory scripts


$installExternalDependenciesScript = (Join-Path $scriptsDirectory install-external-dependencies.ps1)
$deployContentScript = (Join-Path $scriptsDirectory deploy-content.ps1)
$createFilesystemLinksScript = (Join-Path $scriptsDirectory create-filesystem-links.ps1)
$applySystemSettingsScript = (Join-Path $scriptsDirectory apply-system-settings.ps1)
$applyFilesystemPermissions = (Join-Path $scriptsDirectory apply-filesystem-permissions.ps1)

switch ($DeploymentType)
{
	"Full" {
		"Installing External Dependencies"
		& $installExternalDependenciesScript

		"Deploying content"
		& $deployContentScript
		
		"Creating filesystem links"
		& $createFilesystemLinksScript		

		"Applying system settings"
		& $applySystemSettingsScript

		"Applying filesystem permissions"
		& $applyFilesystemPermissions
		
		break
	}
	
	"Update" {
		"Deploying content"
		& $deployContentScript	
		break
	}
	
	default {
		"Unknown deployment type `"$DeploymentType`""
		break
	}
}