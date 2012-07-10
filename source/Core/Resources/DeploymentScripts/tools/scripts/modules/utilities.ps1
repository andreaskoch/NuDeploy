$currentDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$scriptsDirectory = (Get-Item $currentDirectory).parent.FullName
$toolsDirectory = (Get-Item $scriptsDirectory).parent.FullName

function Get-SystemSettings {

	$systemSettingsFilePath = (Join-Path $toolsDirectory systemsettings.xml)
	$transformedSystemSettingsFilePath = (Join-Path $toolsDirectory systemsettings.xml.transformed)
	
	if (Test-Path $transformedSystemSettingsFilePath)
	{
		Write-Host "Get-SystemSettings: Using the transformed system settings."
		[xml]$systemsettings = Get-Content $transformedSystemSettingsFilePath
		return $systemsettings
	}
	else
	{
		Write-Host "Get-SystemSettings: Using the default system settings."
		[xml]$systemsettings = Get-Content $systemSettingsFilePath
		return $systemsettings		
	}
}