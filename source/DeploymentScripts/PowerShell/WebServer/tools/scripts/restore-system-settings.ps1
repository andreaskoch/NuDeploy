"Executing restore-system-settings.ps1"

# Global Variables
$currentDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$modulesDirectory = Join-Path $currentDirectory modules
$toolsDirectory = (Get-Item $currentDirectory).parent.FullName

# Imports
Import-Module WebAdministration
Import-Module (Join-Path $modulesDirectory utilities.ps1)
Import-Module (Join-Path $modulesDirectory iis-management.ps1)
Import-Module (Join-Path $modulesDirectory local-usermanagement.ps1)
Import-Module (Join-Path $modulesDirectory certificate-management.ps1)
Import-Module (Join-Path $modulesDirectory hostfile-management.ps1)
Import-Module (Join-Path $modulesDirectory scheduled-task-management.ps1)
Import-Module (Join-Path $modulesDirectory firewall-management.ps1)
Import-Module (Join-Path $modulesDirectory local-security-policy-management.ps1)

# Read system settings
[xml]$systemsettings = Get-SystemSettings

# Firewall Rules
if ($systemsettings.Settings.Firewall)
{
	if ($systemsettings.Settings.Firewall.PortRules -and $systemsettings.Settings.Firewall.PortRules.PortRule)
	{
		"Removing Firewall rules"
		foreach($portRule in $systemsettings.Settings.Firewall.PortRules.PortRule) 
		{
			Remove-FirewallPortRule -Name $portRule.Name -Direction $portRule.Direction -LocalPorts $portRule.LocalPorts -Protocol $portRule.Protocol
		}
	}

	if ($systemsettings.Settings.Firewall.ProgramRules -and $systemsettings.Settings.Firewall.ProgramRules.ProgramRule)
	{
		foreach($programRule in $systemsettings.Settings.Firewall.ProgramRules.ProgramRule) 
		{
			Remove-FirewallProgramRule -Name $programRule.Name -Direction $programRule.Direction -Program $programRule.Program
		}
	}
}

# Batch Jobs
if ($systemsettings.Settings.BatchJobs -and $systemsettings.Settings.BatchJobs.Job)
{
	"Removing Batch Jobs"
	foreach($batchJob in $systemsettings.Settings.BatchJobs.Job) 
	{
		"Removing batch job $($batchJob.Name)"
		Remove-ScheduledTask -ComputerName localhost -TaskName $batchJob.Name -TaskLocation $batchJob.Location
	}

	"Removing Batch Jobs Folders"
	$batchJobFolders = @()
	foreach($batchJob in $systemsettings.Settings.BatchJobs.Job) 
	{
		if ($batchJob.Location.Contains("\"))
		{
			$rootFolder = ($batchJob.Location.Split("\") | Select -First 1)
			if ($rootFolder -and $rootFolder.Length -gt 0)
			{
				$batchJobFolders += $rootFolder
			}
		}
		
		$batchJobFolders += $batchJob.Location
	}

	$uniqueBatchJobFolders = $batchJobFolders | Select -unique
	$sortedBatchJobFolders = $uniqueBatchJobFolders | Sort-Object -property Length -descending

	foreach ($batchJobFolderName in $sortedBatchJobFolders)
	{
		"Removing batch job folder $batchJobFolderName"
		Remove-ScheduledTaskFolder -ComputerName localhost -TaskLocation $batchJobFolderName
	}
}

# Hosts file
if ($systemsettings.Settings.HostFileAdditions -and $systemsettings.Settings.HostFileAdditions.Enabled -eq "true")
{
    "Removing Hosts file extensions"
    $hostfileAdditionsFolder = Join-Path $toolsDirectory hosts-files
    $hostfileAdditionsFilePath = Join-Path $hostfileAdditionsFolder $systemsettings.Settings.HostFileAdditions.Filename
    if ((Test-Path $hostfileAdditionsFilePath) -eq $true)
    {
        Remove-HostfileExtension -ExtensionFilePath $hostfileAdditionsFilePath
    }
}

# FTP Sites
if ($systemsettings.Settings.IIS -and $systemsettings.Settings.IIS.FtpSites -and $systemsettings.Settings.IIS.FtpSites.FtpSite)
{
	"Removing FTP Sites"
	foreach($site in $systemsettings.Settings.IIS.FtpSites.FtpSite) 
	{		
		"Removing Authorization Rules"
		if ($site.AuthorizationRules -and $site.AuthorizationRules.Groups)
		{
			"Removing group authorization rules"
			foreach($group in $site.AuthorizationRules.Groups.Group)
			{
				"Removing group $($group.Name) from site $($site.Name)"
				Remove-FtpSiteGroupAuthorizationRole -SiteName $site.Name -GroupName $group.Name
			}
		}		
	
		"Removing ftp site $($site.Name)"
		Remove-Website -Name $site.Name
	}
}

# Websites
if ($systemsettings.Settings.IIS -and $systemsettings.Settings.IIS.Sites -and $systemsettings.Settings.IIS.Sites.Site)
{
	"Removing Websites"
	foreach($site in $systemsettings.Settings.IIS.Sites.Site) 
	{
		"Stopping website $($site.Name)"
		Stop-Website -Name $site.Name
		
		if ($site.Bindings -and $site.Bindings.Binding)
		{		
			"Remove all http bindings from website $($site.Name)"
			Clear-WebsiteBindings -Name $site.Name
		}
		
		if ($site.SslBindings -and $site.SslBindings.SslBinding)
		{		
			"Remove all ssl bindings from website $($site.Name)"
			foreach($binding in $site.SslBindings.SslBinding)
			{
				"Removing ssl binding for ip address $($binding.IPAddress)"
				Remove-SslBinding -IPAddress $binding.IPAddress -Port $binding.Port
			}
		}

		"Removing website $($site.Name)"
		Remove-Website -Name $site.Name
	}
}

# Application Pools
if ($systemsettings.Settings.IIS -and $systemsettings.Settings.IIS.ApplicationPools -and $systemsettings.Settings.IIS.ApplicationPools.ApplicationPool)
{
	"Removing Application Pools"
	foreach($applicationPool in $systemsettings.Settings.IIS.ApplicationPools.ApplicationPool) 
	{
		"Stopping application pool $($applicationPool.Name)"
		Stop-WebAppPool -Name $applicationPool.Name

		"Removing application pool $($applicationPool.Name)"
		Remove-WebAppPool -Name $applicationPool.Name
	}
}

# Security Policies
if ($systemsettings.Settings.SecurityPolicies -and $systemsettings.Settings.SecurityPolicies.LocalSecurityPolicies.Areas -and $systemsettings.Settings.SecurityPolicies.LocalSecurityPolicies.LocalSecurityPolicy)
{
	"Rolling back security policy changes"

	$localSecurityPolicyAreas = $systemsettings.Settings.SecurityPolicies.LocalSecurityPolicies.Areas
	$LocalUserRightsPolicy = (Get-LocalSecurityPolicies -Areas $localSecurityPolicyAreas)
	$RevisedUserRightsPolicy = $LocalUserRightsPolicy

	foreach($localSecurityPolicy in $systemsettings.Settings.SecurityPolicies.LocalSecurityPolicies.LocalSecurityPolicy) 
	{   
		$sectionName = $localSecurityPolicy.SectionName
		$key = $localSecurityPolicy.Key
		$username = $localSecurityPolicy.Value
		
		switch ($localSecurityPolicy.Action)
		{
			"Add" {
				"Removing user '$username' from security policy '$key'"
				$RevisedUserRightsPolicy = (Remove-ValueFromLocalSecurityPolicies -Policies $RevisedUserRightsPolicy -SectionName $sectionName -Key $key -Value $username)
			}
			"Remove" {
				"Adding user '$username' to security policy '$key'"
				$RevisedUserRightsPolicy = (Add-ValueToLocalSecurityPolicies -Policies $RevisedUserRightsPolicy -SectionName $sectionName -Key $key -Value $username)
			}
		}
	}

	Set-LocalSecurityPolicies -Policies $RevisedUserRightsPolicy -Areas $localSecurityPolicyAreas
}

# Remove members from groups
if ($systemsettings.Settings.Authorization -and $systemsettings.Settings.Authorization.UserGroups -and $userGroup.Members -and $systemsettings.Settings.Authorization.UserGroups.UserGroup -and $userGroup.Members.Member)
{
	"Removing members from groups"
	foreach($userGroup in $systemsettings.Settings.Authorization.UserGroups.UserGroup) 
	{
		if ($userGroup.IsLocal -eq "true")
		{	
			if ((LocalGroup-Exists -GroupName $userGroup.Name) -eq $true)
			{
				"Removing members from group $($userGroup.Name)"
				foreach($groupMember in $userGroup.Members.Member)
				{
					"Removing member $groupMember.Name from group $($userGroup.Name)"
					Set-LocalGroup -groupName $userGroup.Name -userName $groupMember.Name -remove
				}
			}
			else
			{
				"Group $($userGroup.Name) does not exist."
			}
		}
	}
}

# Groups
if ($systemsettings.Settings.Authorization -and $systemsettings.Settings.Authorization.UserGroups -and $systemsettings.Settings.Authorization.UserGroups.UserGroup)
{
	foreach($userGroup in $systemsettings.Settings.Authorization.UserGroups.UserGroup) 
	{
		if ($userGroup.IsLocal -eq "true")
		{
			if ((LocalGroup-Exists -GroupName $userGroup.Name) -eq $true)
			{
				"Removing group $($userGroup.Name)"
				Remove-LocalGroup -groupName $userGroup.Name
			}
			else
			{
				"Group $($userGroup.Name) does not exist."
			}
		}
	}
}

# Users
if ($systemsettings.Settings.Authorization -and $systemsettings.Settings.Authorization.UserAccounts -and $systemsettings.Settings.Authorization.UserAccounts.UserAccount)
{
	"Removing User Accounts"
	foreach($userAccount in $systemsettings.Settings.Authorization.UserAccounts.UserAccount) 
	{
		if ($userAccount.IsLocal -eq "true")
		{
			if ((LocalUser-Exists -UserName $userAccount.Username) -eq $true)
			{
				"Removing user $($userAccount.Username)"
				Remove-LocalUser -userName $userAccount.Username
			}
			else
			{
				"User $($userAccount.Username) does not exist."
			}
		}
	}
}

# SSL Certificates
if ($systemsettings.Settings.Certificates -and $systemsettings.Settings.Certificates.Certificate)
{
	"Removing SSL Certificates"

	$certificateFolder = Join-Path $toolsDirectory certificates
	foreach($certificate in $systemsettings.Settings.Certificates.Certificate) 
	{
		if ($certificate.IsShared -eq "true")
		{
			"Leaving certificate $($certificate.Thumbprint) installed because it is shared."
			continue
		}

		$certificatePath = $certificate.CertificateRootStore + "\" + $certificate.CertificateStore
		
		"Removing certificate $($certificate.Thumbprint) from $certificatePath"
		Remove-Certificate -Thumbprint $certificate.Thumbprint -CertificateRootStore $certificate.CertificateRootStore -CertificateStore $certificate.CertificateStore
	}
}