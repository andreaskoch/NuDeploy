
# Global Variables
$pathOfTheAssemblyInfoScript = Split-Path -Parent $MyInvocation.MyCommand.Path

Function Get-AssemblyVersion([string] $assemblyPath)
{
	if ((Test-Path $assemblyPath) -eq $false) {
		return $null
	}
	
	$assemblyVersionGetCommand = "$pathOfTheAssemblyInfoScript\GetAssemblyVersion.exe `"$assemblyPath`""
	$assemblyVersion = Invoke-Expression $assemblyVersionGetCommand
	$getVersionSucceeded = ($LASTEXITCODE -eq 0)
	
	if ($buildSucceeded -eq 0 -or $assemblyVersion -eq "") {
		return "1.0.0.0"
	}	
	
	return $assemblyVersion
}