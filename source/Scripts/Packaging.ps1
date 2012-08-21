
# Global Variables
$pathOfThePackagingScript = Split-Path -Parent $MyInvocation.MyCommand.Path

# Imports
Import-Module (Join-Path $pathOfThePackagingScript "AssemblyInfo.ps1")

Function Package-Project([string] $baseDirectory, [string] $packageTargetDirectory, [string] $projectName)
{
    if ((Test-Path $baseDirectory) -eq $false)
    {
        Write-Error "The specified base directory `"$baseDirectory`" does not exist."
        return $false
    }
    
    if (($projectName -eq $null) -or $projectName.Length -eq 0)
    {
        Write-Error "The specified project name cannot be null or empty."
        return $false
    }

    if ((Test-Path $packageTargetDirectory) -eq $false)
    {
        New-Item $packageTargetDirectory -type directory
    }

	$buildOutputDirectory = Join-Path $baseDirectory "$projectName\bin\$buildConfiguration"
	$nuspecFilePath = Join-Path $buildOutputDirectory "NuDeploy.$projectName.nuspec"
	$versionFilePath = (Get-Item "$buildOutputDirectory\NuDeploy.*.dll" | Select-Object -First 1).FullName
	$packageVersion = Get-AssemblyVersion -AssemblyPath $versionFilePath

	$nugetExePath = Join-Path $baseDirectory ".nuget\nuget.exe"
	$nugetPackCommand = "$nugetExePath pack `"$nuspecFilePath`" -Version `"$packageVersion`" -OutputDirectory `"$packageTargetDirectory`""
	$packResult = Invoke-Expression $nugetPackCommand
	$packagingSucceeded = ($LASTEXITCODE -eq 0)

	if ($packagingSucceeded -eq 0) {
		return $false
	}
	
	return $true
}