"Executing update-content.ps1"

function Free-Handles([string]$lockedFolder, [string]$pathToHandle)
{
	"Atempting to close all file handles in $lockedFolder"
	#Get the console output of HANDLE.EXE from Sysinternals into a variable
	$data=(& $pathToHandle $lockedFolder)
	$lenData=$data.Length-1

	#check whether there are open handles for the file/folder
	# Remember, the good stuff starts at line 5
	for ($index=5;$index -le $lenData; $index++) 
	{
		#split current "line" ignoring blanks
		$splitData=$data[$index].ToString().split(": ") | where {$_ -ne ""}
			
		# Get the Process Id for each file
		$procID=$splitData[2]
		# Get the Hexadecimal ID for each open file
			
		$hexID=$splitData[5]
		# Close that file!
		Write-Host "Attempting to close file handle with hexid = $hexID and procID = $procID was closed"
		(& $pathToHandle -c $hexID.toString() -p $procID.tostring() -y) #|out-null
	}	
}

# Global Variables
$currentDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$modulesDirectory = Join-Path $currentDirectory modules
$toolsDirectory = (Get-Item $currentDirectory).parent.FullName
$packageDirectory = (Get-Item $toolsDirectory).Parent.FullName
$pathToHandle = Join-Path $currentDirectory 'handle\handle.exe'
$tempDeployFile = Join-Path $currentDirectory tempDeploymentFile

# Imports
Import-Module WebAdministration
. (Join-Path $modulesDirectory utilities.ps1)
. (Join-Path $modulesDirectory iis-management.ps1)
. (Join-Path $modulesDirectory local-usermanagement.ps1)
. (Join-Path $modulesDirectory certificate-management.ps1)
. (Join-Path $modulesDirectory hostfile-management.ps1)
. (Join-Path $modulesDirectory scheduled-task-management.ps1)
. (Join-Path $modulesDirectory firewall-management.ps1)
. (Join-Path $modulesDirectory local-security-policy-management.ps1)
. (Join-Path $modulesDirectory topshelf-windows-service-management.ps1)

# Read system settings
[xml]$systemsettings = Get-SystemSettings

$targetDirectory = $systemsettings.Settings.DeploymentSettings.TargetFolder

# make sure the target directory exists
if((Test-Path $targetDirectory) -eq $false) {
	"Creating the target directory $targetDirectory"
	New-Item $targetDirectory -type directory
}

# Uninstall TopShelf Service
if ($systemsettings.Settings.TopshelfServices)
{
    foreach ($service in $systemsettings.Settings.TopshelfServices.Service) {
        Uninstall-TopShelfService -exePath $service.Executable -displayName $service.DisplayName -instance $service.Instance
    }
}

# Websites
if ($systemsettings.Settings.IIS -and $systemsettings.Settings.IIS.Sites -and $systemsettings.Settings.IIS.Sites.Site)
{
	"Setting Websites to a different folder"
	foreach($site in $systemsettings.Settings.IIS.Sites.Site) 
	{
		"Setting website $($site.Name) to path $tempDeployFile"
		$websitePath = Join-Path IIS:\Sites $site.Name
		Set-ItemProperty $websitePath PhysicalPath $tempDeployFile.toString()	
	}
}

# Application Pools
if ($systemsettings.Settings.IIS -and $systemsettings.Settings.IIS.ApplicationPools -and $systemsettings.Settings.IIS.ApplicationPools.ApplicationPool)
{
	"Restarting Application Pools"
	foreach($applicationPool in $systemsettings.Settings.IIS.ApplicationPools.ApplicationPool) 
	{
		"Stopping application pool $($applicationPool.Name)"
		Restart-WebAppPool -Name $applicationPool.Name
	}
}


"Deploying content"
$contentDirectories = (Get-ChildItem "$packageDirectory\content\*") | Where-Object {$_.Mode -match "d"}
foreach ($directory in $contentDirectories)
{
	$sourceFolderPath = $directory.FullName
	$targetFolderPath = Join-Path $targetDirectory $directory.Name
	
	if((Test-Path $targetFolderPath))
	{
		Free-Handles $targetFolderPath $pathToHandle		
		
		"Removing existing folder $targetFolderPath"
		Remove-Item $targetFolderPath -Recurse -Force
	}
	
	"Copying $sourceFolderPath to $targetFolderPath"
    Copy-Item $sourceFolderPath -Destination $targetFolderPath -Recurse -Force
}


if ($systemsettings.Settings.IIS -and $systemsettings.Settings.IIS.Sites -and $systemsettings.Settings.IIS.Sites.Site)
{
	"Setting Websites to a different folder"
	foreach($site in $systemsettings.Settings.IIS.Sites.Site) 
	{
		"Setting website $($site.Name) to path $site.PhysicalPath"
		$websitePath = Join-Path IIS:\Sites $site.Name
		Set-ItemProperty $websitePath PhysicalPath $site.PhysicalPath		
	}
}

# Application Pools
if ($systemsettings.Settings.IIS -and $systemsettings.Settings.IIS.ApplicationPools -and $systemsettings.Settings.IIS.ApplicationPools.ApplicationPool)
{
	"Restarting Application Pools"
	foreach($applicationPool in $systemsettings.Settings.IIS.ApplicationPools.ApplicationPool) 
	{
		"Restart application pool $($applicationPool.Name)"
		Restart-WebAppPool -Name $applicationPool.Name
	}
}

# Install TopShelf Service
if ($systemsettings.Settings.TopshelfServices)
{
    foreach ($service in $systemsettings.Settings.TopshelfServices.Service) {
                
        $startAfterInstallation = $False
        if ($service.startAfterInstallation -eq "true") {
            $startAfterInstallation = $True
        }
        
        Install-TopShelfService -exePath $service.Executable -displayName $service.DisplayName -instance $service.Instance -username $service.Username -password $service.Password -description $service.Description -start $service.Start -startAfterInstallation:$startAfterInstallation
    }
}