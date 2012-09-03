
# Global Variables
$currentDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$libraryDirectory = Join-Path $currentDirectory "Scripts"
$projectDirectory = Split-Path -Parent $currentDirectory
$nugetPackageDirectoryBase = Join-Path $projectDirectory "packages"

# Imports
Import-Module (Join-Path $libraryDirectory "Packaging.ps1")

# Build Solution
$solutionPath = Join-Path $currentDirectory "NuDeploy.sln"
$buildConfiguration = "Release"
$targetPlatform = "Any CPU"

$buildCommand = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild `"$solutionPath`" /p:Configuration=`"$buildConfiguration`" /p:Platform=`"$targetPlatform`" /t:Rebuild"
Invoke-Expression $buildCommand
$buildSucceeded = ($LASTEXITCODE -eq 0)

if ($buildSucceeded -eq 0) {
	"Building solution `"$solutionPath`" failed."
	exit
}

# Package
$projectsToPackage = "CommandLine", "Core", "DeploymentScripts"
foreach ($project in $projectsToPackage)
{
    $targetDirectory = Join-Path $nugetPackageDirectoryBase "NuDeploy.$project"
	$packagingResult = Package-Project -baseDirectory $currentDirectory -packageTargetDirectory $targetDirectory -projectName $project
	
	if ($packagingResult -eq $true)
	{
		Write-Host "Packaging of NuDeploy.$project succeeded."
	}
	else
	{
		Write-Error "Packaging of NuDeploy.$project failed."
	}
}