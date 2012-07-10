﻿Import-Module WebAdministration

Function AppPool-Exists
{ 
	[CmdletBinding()]
	Param(
		[Parameter(Position=0,Mandatory=$True,ValueFromPipeline=$True)]
		[string]$name
	)

	try {
		$appPools = Get-ChildItem IIS:\AppPools
		foreach($appPool in $appPools)
		{
			if ($appPool.Name -eq $name)
			{
				return $true
			}
		}
		
		return $false
	} catch {
		return $false
	}
}

Function Website-Exists
{ 
	[CmdletBinding()]
	Param(
		[Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$name
	)

	try {
		$sites = Get-ChildItem IIS:\Sites
		foreach($site in $sites)
		{
			if ($site.Name -eq $name)
			{
				return $true
			}
		}
		
		return $false
	} catch {
		return $false
	}
}

Function WebsiteBinding-Exists
{ 
	[CmdletBinding()]
	Param(
		[Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$name,
		
		[Parameter(Position=1, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$protocol,
		
		[Parameter(Position=2, Mandatory=$True, ValueFromPipeline=$True)]
		[int]$port,
		
		[Parameter(Position=3, Mandatory=$False, ValueFromPipeline=$True)]
		[string]$HostHeader=""
	)

	try {
		$binding = (Get-WebBinding -Name $name -Protocol $protocol -Port $port -HostHeader $HostHeader)
		if ($binding.protocol -eq $protocol) {
			return $true
		} else {
			return $false
		}
	} catch {
		return $false
	}
}

Function SslBinding-Exists
{ 
	[CmdletBinding()]
	Param(
		[Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$IPAddress,

		[Parameter(Position=1, Mandatory=$True, ValueFromPipeline=$True)]
		[int]$Port=443
	)

	$binding = (Get-ChildItem IIS:\SslBindings | Where-Object {($_.IPAddress -eq $IPAddress) -and ($_.Port -eq $Port)} | Select-Object -First 1)
	if ($binding)
	{
		return $true
	}

	return $false
}

Function WebApplication-Exists
{ 
	[CmdletBinding()]
	Param(
		[Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$websiteName,
		[Parameter(Position=1, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$applicationName  
	)

	try {
		$applications = Get-WebApplication -Site $websiteName
		foreach($application in $applications)
		{
			if ($application.Path -eq "/$applicationName")
			{
				return $true
			}
		}
		
		return $false
	} catch {
		return $false
	}
}

Function VirtualDirectory-Exists
{ 
	[CmdletBinding()]
	Param(
		[Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$websiteName,
		
		[Parameter(Position=1, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$virtualDirectoryName,
		
		[Parameter(Mandatory=$False, ValueFromPipeline=$True)]
		[string]$applicationName
	)

	try {
		$virtualDirectory = $null
		if ($applicationName) {
			$virtualDirectory = Get-WebVirtualDirectory -Site $websiteName -Application $applicationName -Name $virtualDirectoryName
		} else {
			$virtualDirectory = Get-WebVirtualDirectory -Site $websiteName -Name $virtualDirectoryName
		}
		
		if ($virtualDirectory)
		{
			return $true
		}
		
		return $false
	} catch {
		return $false
	}
}

Function Get-AppPool
{ 
	[CmdletBinding()]
	Param(
		[Parameter(Position=0,Mandatory=$True,ValueFromPipeline=$True)]
		[string]$name
	)

	try {
		$appPools = Get-ChildItem IIS:\AppPools
		foreach($appPool in $appPools)
		{
			if ($appPool.Name -eq $name)
			{
				return $appPool
			}
		}
		
		return $null
	} catch {
		return $null
	}
}

Function Get-Website
{ 
	[CmdletBinding()]
	Param(
		[Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$name
	)

	try {
		$sites = Get-ChildItem IIS:\Sites
		foreach($site in $sites)
		{
			if ($site.Name -eq $name)
			{
				return $site
			}
		}
		
		return $null
	} catch {
		return $null
	}
}

Function Get-WebsiteBinding
{ 
	[CmdletBinding()]
	Param(
		[Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$name,
		
		[Parameter(Position=1, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$protocol,
		
		[Parameter(Position=2, Mandatory=$True, ValueFromPipeline=$True)]
		[int]$port,
		
		[Parameter(Position=3, Mandatory=$False, ValueFromPipeline=$True)]
		[string]$hostHeader=""
	)

	try {
		$binding = (Get-WebBinding -Name $name -Protocol $protocol -Port $port -HostHeader $hostHeader)
		
		if ($binding.protocol -eq $protocol) {
			return $binding
		} else {
			return $null
		}
	} catch {
		return $null
	}
}

Function Clear-WebsiteBindings
{ 
	[CmdletBinding()]
	Param(
		[Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$name
	)

	$bindings = (Get-WebBinding -Name $name)
	if ($bindings)
	{
		foreach ($binding in $bindings)
		{
			$binding | Remove-WebBinding
		}
		
		return $true
	}

	return $false
}

Function Remove-SslBinding
{ 
 [CmdletBinding()]
	Param(
		[Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$IPAddress,
		
		[Parameter(Position=1, Mandatory=$True, ValueFromPipeline=$True)]
		[int]$Port=443
	)

	$bindings = (Get-ChildItem IIS:\SslBindings | Where-Object {($_.IPAddress -eq $IPAddress) -and ($_.Port -eq $Port)})
	if ($bindings)
	{
		foreach ($binding in $bindings)
		{
			$binding | Remove-Item
		}
		
		return $true
	}
	
	return $false
}

Function Remove-VirtualDirectory
{ 
	[CmdletBinding()]
	Param(
		[Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$websiteName,
		
		[Parameter(Position=2, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$virtualDirectoryName,
		
		[Parameter(Mandatory=$False, ValueFromPipeline=$True)]
		[string]$applicationName		
	)

	try {
		if ($applicationName) {
			Remove-WebVirtualDirectory -Site $websiteName -Application $applicationName -Name $virtualDirectoryName
		} else {
			Remove-WebVirtualDirectory -Site $websiteName -Name $virtualDirectoryName
		}
		return $true
	} catch {
		return $false
	}
}

Function CreateOrUpdate-AppPool
{ 
	[CmdletBinding()]
	Param(
		[Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$name,
		
		[Parameter(Position=1, Mandatory=$False, ValueFromPipeline=$True)]
		[string]$pipelineMode="Integrated",
		
		[Parameter(Position=2, Mandatory=$False, ValueFromPipeline=$True)]
		[string]$runtimeVersion="v4.0", 
		
		[Parameter(Position=3, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$username,
		
		[Parameter(Position=4, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$password
	)

	try {
		if ((AppPool-Exists -Name $name) -eq $false) {
			New-WebAppPool -Name $name
		}

		$appPool = (Get-AppPool -Name $name)
		$appPool.managedPipelineMode = $pipelineMode
		$appPool.managedRuntimeVersion = $runtimeVersion
		$appPool.processModel.username = [string]($username)
		$appPool.processModel.password = [string]($password)
		$appPool.processModel.identityType = "SpecificUser"
		$appPool | Set-Item

		return $appPool
	} catch {
		return $null
	}
}

Function CreateOrUpdate-Website
{ 
	[CmdletBinding()]
	Param(
		[Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$name,
		
		[Parameter(Position=1, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$physicalPath,
		
		[Parameter(Position=2, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$applicationPoolName,
		
		[Parameter(Position=3, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$logfileDirectory,
		
		[Parameter(Position=4, Mandatory=$False, ValueFromPipeline=$True)]
		[int]$logFieldFlags=2199503
	)

	if ((Website-Exists -Name $name) -eq $false)
	{
		New-Website -Name $name -PhysicalPath $physicalPath -ApplicationPool $applicationPoolName
	}

	$LogFormat = @{
		IIS = 0;
		NCSA = 1;
		W3C = 2;
		Custom = 4;
	}    

	$websitePath = Join-Path IIS:\Sites $name
	Set-ItemProperty $websitePath PhysicalPath $physicalPath
	Set-ItemProperty $websitePath ApplicationPool $applicationPoolName
	Set-ItemProperty $websitePath Logfile.Directory $logfileDirectory
	Set-ItemProperty $websitePath Logfile.LogExtFileFlags $logFieldFlags
	Set-ItemProperty $websitePath Logfile.LogFormat $LogFormat.W3C

	$website = (Get-WebSite -Name $name)
	return $website
    
}

Function CreateOrUpdate-WebsiteBinding
{ 
	[CmdletBinding()]
	Param(
		[Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$name,
		
		[Parameter(Position=1, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$protocol,
		
		[Parameter(Position=2, Mandatory=$True, ValueFromPipeline=$True)]
		[int]$port,
		
		[Parameter(Position=3, Mandatory=$False, ValueFromPipeline=$True)]
		[string]$ipAddress="*",
		
		[Parameter(Position=4, Mandatory=$False, ValueFromPipeline=$True)]
		[string]$hostHeader=""
	)

	if ((WebsiteBinding-Exists -Name $name -Protocol $protocol -Port $port -HostHeader $hostHeader) -eq $true) {
    	$binding = (Get-WebsiteBinding -Name $name -Protocol $protocol -Port $port -HostHeader $hostHeader)
        $binding | Remove-WebBinding
	}
    
    New-WebBinding -Name $name -Protocol $protocol -Port $port -IPAddress $ipAddress -HostHeader $hostHeader
    $binding = (Get-WebsiteBinding -Name $name -Protocol $protocol -Port $port -HostHeader $hostHeader)
	
	return $binding
    
}

Function CreateOrUpdate-SslBinding
{ 
 [CmdletBinding()]
	Param(
		[Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$IPAddress,
		
		[Parameter(Position=1, Mandatory=$True, ValueFromPipeline=$True)]
		[int]$Port=443,
		
		[Parameter(Position=2, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$SslCertificateStore="My",
		
		[Parameter(Position=3, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$SslCertificateThumbprint
	)

	if((SslBinding-Exists -IPAddress $IPAddress -Port $Port) -eq $true)
	{
		"Removing existing binding"
		Remove-SslBinding -IPAddress $IPAddress -Port $Port
	}
	
	$sslBindingName = "$IPAddress!$Port"
	$sslBindingPath = Join-Path IIS:\SslBindings $sslBindingName
	$certificatePath = Join-Path cert:\LocalMachine\$SslCertificateStore $sslCertificateThumbprint
	$cert = (get-item $certificatePath)
	
	new-item $sslBindingPath -Value $cert
    
}

Function CreateOrUpdate-WebApplication
{ 
	[CmdletBinding()]
	Param(
		[Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$websiteName,
		[Parameter(Position=1, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$applicationName,
		[Parameter(Position=2, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$applicationPath,
		[Parameter(Position=3, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$applicationPoolName  
	)

	if ((WebApplication-Exists -WebsiteName $websiteName -ApplicationName $applicationName) -eq $true) {
    	Remove-WebApplication -Site $websiteName -Name $applicationName
	}

	$application = (New-WebApplication -Site $websiteName -Name $applicationName -PhysicalPath $applicationPath -ApplicationPool $applicationPoolName)
	return $application
    
}

Function CreateOrUpdate-VirtualDirectory
{ 
	[CmdletBinding()]
	Param(
		[Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$websiteName,
		
		[Parameter(Position=1, Mandatory=$False, ValueFromPipeline=$True)]
		[string]$virtualDirectoryName,
		
		[Parameter(Position=2, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$path,
		
		[Parameter(Mandatory=$False, ValueFromPipeline=$True)]
		[string]$applicationName		
	)

	try {
		
		if ((VirtualDirectory-Exists -WebsiteName $websiteName -ApplicationName $applicationName -VirtualDirectoryName $virtualDirectoryName) -eq $true)
		{
			Remove-VirtualDirectory -WebsiteName $websiteName -ApplicationName $applicationName -VirtualDirectoryName $virtualDirectoryName
		}
	
		if ($applicationName) {
			New-WebVirtualDirectory -Site $websiteName -Application $applicationName -Name $virtualDirectoryName -PhysicalPath $path
		} else {
			New-WebVirtualDirectory -Site $websiteName -Name $virtualDirectoryName -PhysicalPath $path
		}
		return $true
	} catch {
		return $false
	}
}