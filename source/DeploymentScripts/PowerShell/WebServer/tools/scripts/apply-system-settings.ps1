"Executing apply-system-settings.ps1"

# Global Variables
$currentDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$modulesDirectory = Join-Path $currentDirectory modules
$toolsDirectory = (Get-Item $currentDirectory).parent.FullName

# Imports
Import-Module WebAdministration
. (Join-Path $modulesDirectory utilities.ps1)
. (Join-Path $modulesDirectory certificate-management.ps1)
. (Join-Path $modulesDirectory local-usermanagement.ps1)
. (Join-Path $modulesDirectory iis-management.ps1)
. (Join-Path $modulesDirectory hostfile-management.ps1)
. (Join-Path $modulesDirectory scheduled-task-management.ps1)
. (Join-Path $modulesDirectory firewall-management.ps1)
. (Join-Path $modulesDirectory local-security-policy-management.ps1)
. (Join-Path $modulesDirectory topshelf-windows-service-management.ps1)
. (Join-Path $modulesDirectory dacpac-management.ps1)

# Read system settings
[xml]$systemsettings = Get-SystemSettings

# SSL Certificates
if ($systemsettings.Settings.Certificates -and $systemsettings.Settings.Certificates.Certificate)
{
	"Importing SSL Certificates"
	$certificateFolder = Join-Path $toolsDirectory certificates
	foreach($certificate in $systemsettings.Settings.Certificates.Certificate) 
	{
		if ($certificate.IsShared -eq "true" -and (Certificate-Exists -Thumbprint $certificate.Thumbprint -CertificateRootStore $certificate.CertificateRootStore -CertificateStore $certificate.CertificateStore) -eq $true)
		{
			"Skip installation of certificate $($certificate.Thumbprint) because it is shared and is already installed."
			continue
		}

		"Importing certificate $($certificate.Filename)"
		
		$filepath =  Join-Path $certificateFolder $certificate.Filename
		Import-Certificate -FilePath $filepath -RootStore $certificate.CertificateRootStore -CertStore $certificate.CertificateStore -Password $certificate.Password
	}
}

# Users
if ($systemsettings.Settings.Authorization -and $systemsettings.Settings.Authorization.UserAccounts -and $systemsettings.Settings.Authorization.UserAccounts.UserAccount)
{
	"Creating User Accounts"
	foreach($userAccount in $systemsettings.Settings.Authorization.UserAccounts.UserAccount) 
	{
		if ($userAccount.IsLocal -eq "true")
		{
			if ((LocalUser-Exists -UserName $userAccount.Username) -eq $false)
			{
				"Creating user $($userAccount.Username)"
				New-LocalUser -userName $userAccount.Username -password $userAccount.Password -fullName $userAccount.FullName -description $userAccount.Description
			}
			else
			{
				"User $($userAccount.Username) already exists"
			}
		}
	}
}

# Groups
if ($systemsettings.Settings.Authorization -and $systemsettings.Settings.Authorization.UserGroups -and $systemsettings.Settings.Authorization.UserGroups.UserGroup)
{
	"Creating Groups"
	foreach($userGroup in $systemsettings.Settings.Authorization.UserGroups.UserGroup) 
	{
		if ($userGroup.IsLocal -eq "true")
		{
			if ((LocalGroup-Exists -GroupName $userGroup.Name) -eq $false)
			{
				"Creating group $($userGroup.Name)"
				New-LocalGroup -groupName $userGroup.Name -description $userGroup.Description
			}
			else
			{
				"Group $($userGroup.Name) already exists."
			}
		}
	}
}

# Adding members to groups
if ($systemsettings.Settings.Authorization -and $systemsettings.Settings.Authorization.UserGroups -and $systemsettings.Settings.Authorization.UserGroups.UserGroup)
{
	"Adding members to groups"
	foreach($userGroup in $systemsettings.Settings.Authorization.UserGroups.UserGroup) 
	{
		if ((LocalGroup-Exists -GroupName $userGroup.Name) -eq $true)
		{
			"Adding members to group $($userGroup.Name)"
			foreach($groupMember in $userGroup.Members.Member)
			{
				"Adding member $($groupMember.Name) to group $($userGroup.Name)"
				Set-LocalGroup -groupName $userGroup.Name -userName $groupMember.Name -add
			}
		}
		else
		{
			"Group $($userGroup.Name) does not exists."
		}
	}
}

# Security Policies
if ($systemsettings.Settings.SecurityPolicies -and $systemsettings.Settings.SecurityPolicies.LocalSecurityPolicies.Areas -and $systemsettings.Settings.SecurityPolicies.LocalSecurityPolicies.LocalSecurityPolicy)
{
	"Applying security policies"

	$localSecurityPolicyAreas = $systemsettings.Settings.SecurityPolicies.LocalSecurityPolicies.Areas
	$LocalUserRightsPolicy = (Get-LocalSecurityPolicies -Areas $localSecurityPolicyAreas)
	$RevisedUserRightsPolicy = $LocalUserRightsPolicy

	if ($localSecurityPolicyAreas -and $LocalUserRightsPolicy -and $RevisedUserRightsPolicy)
	{
		foreach($localSecurityPolicy in $systemsettings.Settings.SecurityPolicies.LocalSecurityPolicies.LocalSecurityPolicy) 
		{
			$sectionName = $localSecurityPolicy.SectionName
			$key = $localSecurityPolicy.Key
			$username = $localSecurityPolicy.Value
			
			switch ($localSecurityPolicy.Action)
			{
				"Add" {
					"Adding user '$username' to security policy '$key'"
					$RevisedUserRightsPolicy = (Add-ValueToLocalSecurityPolicies -Policies $RevisedUserRightsPolicy -SectionName $sectionName -Key $key -Value $username)
				}
				"Remove" {
					"Removing user '$username' from security policy '$key'"
					$RevisedUserRightsPolicy = (Remove-ValueFromLocalSecurityPolicies -Policies $RevisedUserRightsPolicy -SectionName $sectionName -Key $key -Value $username)
				}
			}
		}

		Set-LocalSecurityPolicies -Policies $RevisedUserRightsPolicy -Areas $localSecurityPolicyAreas
	}
	else
	{
		"Could not fetch local security policies"
	}
}

# Application Pools
if ($systemsettings.Settings.IIS -and $systemsettings.Settings.IIS.ApplicationPools -and $systemsettings.Settings.IIS.ApplicationPools.ApplicationPool)
{
	"Creating Application Pools"
	foreach($applicationPool in $systemsettings.Settings.IIS.ApplicationPools.ApplicationPool) 
	{   
		"Creating application pool $($applicationPool.Name)"		
		if($applicationPool.Enable32BitAppOnWin64)
		{
			$pool = (CreateOrUpdate-AppPool -Name $applicationPool.Name -Username $applicationPool.Username -Password $applicationPool.Password -pipelineMode $applicationPool.PipelineMode -RuntimeVersion $applicationPool.RuntimeVersion -Enable32BitAppOnWin64 $applicationPool.Enable32BitAppOnWin64) 
		}
		else
		{
			$pool = (CreateOrUpdate-AppPool -Name $applicationPool.Name -Username $applicationPool.Username -Password $applicationPool.Password -pipelineMode $applicationPool.PipelineMode -RuntimeVersion $applicationPool.RuntimeVersion) 
		}		
	}
}

# Websites
if ($systemsettings.Settings.IIS -and $systemsettings.Settings.IIS.Sites -and $systemsettings.Settings.IIS.Sites.Site)
{
	"Creating Websites"
	foreach($site in $systemsettings.Settings.IIS.Sites.Site) 
	{
		"Creating website $($site.Name)"
		CreateOrUpdate-Website -Name $site.Name -PhysicalPath $site.PhysicalPath -ApplicationPoolName $site.ApplicationPoolName -LogfileDirectory $site.LogfileDirectory -LogFieldFlags $site.LogFieldFlags
		
		"Remove existing bindings from the newly create website"
		Clear-WebsiteBindings -Name $site.Name	
		
		if ($site.Bindings -and $site.Bindings.Binding)
		{
			"Creating Bindings"
			foreach($binding in $site.Bindings.Binding)
			{
				"Adding binding to site $($site.Name)"
				CreateOrUpdate-WebsiteBinding -Name $site.Name -Port $binding.Port -Protocol $binding.Protocol -IPAddress $binding.IPAddress -HostHeader $binding.HostHeader
			}
		}
		
		if ($site.SslBindings -and $site.SslBindings.SslBinding)
		{
			"Creating SSL Bindings"
			foreach($binding in $site.SslBindings.SslBinding)
			{
				"Adding SSL binding to site $($site.Name)"
				CreateOrUpdate-SslBinding -IPAddress $binding.IPAddress -Port $binding.Port -SslCertificateStore $binding.SslCertificateStore -SslCertificateThumbprint $binding.SslCertificateThumbprint
			}
		}

		if ($site.VirtualDirectories -and $site.VirtualDirectories.VirtualDirectory)
		{
			"Creating Virtual Directories (Website Level)"
			foreach($virtualDirectory in $site.VirtualDirectories.VirtualDirectory)
			{
				"Adding virtual directory $($virtualDirectory.Name) to site $($site.Name)"
				CreateOrUpdate-VirtualDirectory -WebsiteName $site.Name -VirtualDirectoryName $virtualDirectory.Name -Path $virtualDirectory.Path -Username $virtualDirectory.Username -Password $virtualDirectory.Password
			}
		}
		
		if ($site.WebApplications -and $site.WebApplications.WebApplication)
		{		
			"Creating Web Applications"
			foreach($webApplication in $site.WebApplications.WebApplication)
			{
				"Adding web application $($webApplication.Name) to site $($site.Name)"
				CreateOrUpdate-WebApplication -WebsiteName $site.Name -ApplicationName $webApplication.Name -ApplicationPath $webApplication.ApplicationPath -ApplicationPoolName $webApplication.ApplicationPoolName

				if ($webApplication.VirtualDirectories -and $webApplication.VirtualDirectories.VirtualDirectory)
				{
					"Creating Virtual Directories (Web Application Level)"
					foreach($virtualDirectory in $webApplication.VirtualDirectories.VirtualDirectory)
					{
						"Adding virtual directory $($virtualDirectory.Name) to site $($site.Name)"
						CreateOrUpdate-VirtualDirectory -WebsiteName $site.Name -ApplicationName $webApplication.Name -VirtualDirectoryName $virtualDirectory.Name -Path $virtualDirectory.Path -Username $virtualDirectory.Username -Password $virtualDirectory.Password
					}
				}
			}
		}
		
		"Starting Web Site $($site.Name)"
		Start-Website -Name $site.Name
	}
}

# FTP Sites
if ($systemsettings.Settings.IIS -and $systemsettings.Settings.IIS.FtpSites -and $systemsettings.Settings.IIS.FtpSites.FtpSite)
{
	"Creating FTP Sites"
	foreach($site in $systemsettings.Settings.IIS.FtpSites.FtpSite) 
	{
		"Creating ftp site $($site.Name)"
		Create-FtpSite -Name $site.Name -PhysicalPath $site.PhysicalPath -LogFileDirectory $site.LogFileDirectory -IPAddress $site.IPAddress -HostHeader $site.HostHeader -Port $site.Port -SslCertificateThumbprint $site.SslCertificateThumbprint
		
		"Applying Authorization Rules"
		if ($site.AuthorizationRules -and $site.AuthorizationRules.Groups)
		{
			"Adding group authorization rules"
			foreach($group in $site.AuthorizationRules.Groups.Group)
			{
				"Adding group $($group.Name) to site $($site.Name)"
				Add-FtpSiteGroupAuthorizationRole -SiteName $site.Name -GroupName $group.Name -Permissions $group.Permissions
			}
		}		
	}
}

# Hosts file
if ($systemsettings.Settings.HostFileAdditions -and $systemsettings.Settings.HostFileAdditions.Enabled -eq "true")
{
    "Adding Hosts file extensions"
    $hostfileAdditionsFolder = Join-Path $toolsDirectory hosts-files
    $hostfileAdditionsFilePath = Join-Path $hostfileAdditionsFolder $systemsettings.Settings.HostFileAdditions.Filename
    if ((Test-Path $hostfileAdditionsFilePath) -eq $true)
    {
        Add-HostfileExtension -ExtensionFilePath $hostfileAdditionsFilePath
    }
}

# Batch Jobs
if ($systemsettings.Settings.BatchJobs -and $systemsettings.Settings.BatchJobs.Job)
{
	"Creating Batch Jobs"
	foreach($batchJob in $systemsettings.Settings.BatchJobs.Job) 
	{
		"Creating batch job $($batchJob.Name)"
		if($batchJob.Schedule.StartDate)
		{
			CreateOrUpdate-ScheduledTask -ComputerName localhost -TaskName $batchJob.Name -TaskLocation $batchJob.Location -TaskRun $batchJob.ProgramPath -RunAsUser $batchJob.Username -RunAsUserPassword $batchJob.Password -Schedule $batchJob.Schedule.Type -Interval $batchJob.Schedule.RecurInterval -StartTime $batchJob.Schedule.StartTime -EndTime $batchJob.Schedule.EndTime -StartDate $batchJob.Schedule.StartDate -Parameters $batchJob.Parameters -HighestRunLevel $batchJob.HighestRunLevel
		}
		else
		{
			CreateOrUpdate-ScheduledTask -ComputerName localhost -TaskName $batchJob.Name -TaskLocation $batchJob.Location -TaskRun $batchJob.ProgramPath -RunAsUser $batchJob.Username -RunAsUserPassword $batchJob.Password -Schedule $batchJob.Schedule.Type -Interval $batchJob.Schedule.RecurInterval -StartTime $batchJob.Schedule.StartTime -EndTime $batchJob.Schedule.EndTime -Parameters $batchJob.Parameters -HighestRunLevel $batchJob.HighestRunLevel
		}
	}
}

# Xml based Batch Jobs
if ($systemsettings.Settings.BatchJobsXml -and $systemsettings.Settings.BatchJobsXml.Job)
{
	"Creating XML based Batch Jobs"
	foreach($batchJob in $systemsettings.Settings.BatchJobsXml.Job) 
	{
		"Creating batch job $($batchJob.Name)"
		CreateOrUpdate-XmlBasedScheduledTask -ComputerName localhost -TaskName $batchJob.Name -TaskLocation $batchJob.Location -RunAsUser $batchJob.Username -RunAsUserPassword $batchJob.Password -XmlConfigFilePath $batchJob.XmlConfigFilePath
	}
}

# Firewall Rules
if ($systemsettings.Settings.Firewall)
{
	if ($systemsettings.Settings.Firewall.PortRules -and $systemsettings.Settings.Firewall.PortRules.PortRule)
	{
		"Creating Firewall rules"
		foreach($portRule in $systemsettings.Settings.Firewall.PortRules.PortRule) 
		{
			CreateOrUpdate-FirewallPortRule -Name $portRule.Name -Direction $portRule.Direction -LocalPorts $portRule.LocalPorts -Protocol $portRule.Protocol -Action $portRule.Action
		}
	}

	if ($systemsettings.Settings.Firewall.ProgramRules -and $systemsettings.Settings.Firewall.ProgramRules.ProgramRule)
	{
		foreach($programRule in $systemsettings.Settings.Firewall.ProgramRules.ProgramRule) 
		{
			CreateOrUpdate-FirewallProgramRule -Name $programRule.Name -Direction $programRule.Direction -Program $programRule.Program -Action $portRule.Action
		}
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

# Install DacPacs
if ($systemsettings.Settings.DacPacs)
{
    foreach ($dac in $systemsettings.Settings.DacPacs.DacPac) {
                
        $sqlPackageExePath = $systemsettings.Settings.DacPacs.SqlPackageExePath
        
        Publish-DacPac -sqlPackageExePath $sqlPackageExePath -packagePath $dac.PackagePath -publishProfilePath $dac.PublishProfilePath
        
    }
}