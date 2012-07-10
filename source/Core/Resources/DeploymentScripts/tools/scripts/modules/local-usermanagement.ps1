Function Get-LocalUser 
{ 
	[CmdletBinding()]
	Param(
		[Parameter(Position=0,
		Mandatory=$True,
		
		ValueFromPipeline=$True)]
		[string]$userName,
		
		[string]$computerName = $env:ComputerName
	)
	
	try {
		$user = [ADSI]"WinNT://$computerName/$userName,User"
		return $user
	} catch {
	}
	
	return $null
}

Function Get-LocalUserSid
{ 
	[CmdletBinding()]
	Param(
		[Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$userName,
		
		[Parameter(Position=1, Mandatory=$False, ValueFromPipeline=$True)]
		[string]$computerName = $env:ComputerName
	)
	
	try {
		if ((LocalUser-Exists -UserName $userName -ComputerName $computerName) -eq $true)
		{
			$objUser = New-Object System.Security.Principal.NTAccount($userName)
			$strSID = $objUser.Translate([System.Security.Principal.SecurityIdentifier])
			return $strSID.Value
		}
	} catch {
	}
	
	return $null
}

Function LocalUser-Exists
{ 
	[CmdletBinding()]
	Param(
		[Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$userName,
		
		[string]$computerName = $env:ComputerName
	)

	try {
		$localUser = (Get-LocalUser -userName $userName -computerName $computerName)
		if ($localUser.Name -eq $userName) {
			return $true
		}

		return $false;
	} catch {
		return $false
	}
}

Function Get-LocalGroup 
{ 
	[CmdletBinding()]
	Param(
		[Parameter(Position=0,Mandatory=$True,ValueFromPipeline=$True)]
		[string]$groupName,
		
		[string]$computerName = $env:ComputerName
	)
	
	try {
		$group = [ADSI]"WinNT://$computerName/$groupName,Group"
		return $group
	} catch {
	}
	
	return $null
}

Function Get-LocalGroupSid
{ 
	[CmdletBinding()]
	Param(
		[Parameter(Position=0,Mandatory=$True,ValueFromPipeline=$True)]
		[string]$groupName,
		
		[Parameter(Position=1,Mandatory=$False,ValueFromPipeline=$True)]
		[string]$computerName = $env:ComputerName
	)
	
	try {
		if ((LocalGroup-Exists -GroupName $GroupName -ComputerName $ComputerName) -eq $true)
		{
			$group = New-Object System.Security.Principal.NTAccount($GroupName)
			$strSID = $group.Translate([System.Security.Principal.SecurityIdentifier])
			return $strSID.Value
		}
	} catch {
	}
	
	return $null
}

Function LocalGroup-Exists
{ 
	[CmdletBinding()]
	Param(
		[Parameter(Position=0,Mandatory=$True,ValueFromPipeline=$True)]
		[string]$groupName,

		[string]$computerName = $env:ComputerName
	)

	try {
		$localGroup = (Get-LocalGroup -groupName $groupName -computerName $computerName)
		if ($localGroup.Name -eq $groupName) {
			return $true
		}

		return $false;
	} catch {
		return $false
	}
}

Function New-LocalUser 
{ 
	[CmdletBinding()] 
	Param( 
		[Parameter(Position=0,Mandatory=$True,ValueFromPipeline=$True)] 
		[string]$userName,
		
		[Parameter(Position=1,Mandatory=$True,ValueFromPipeline=$True)]
		[string]$password,
		
		[string]$computerName = $env:ComputerName,
		
		[string]$fullName = "",
		
		[string]$description = ""
	)
 
	$AccountOptions = @{
		ACCOUNTDISABLE = 2;
		LOCKOUT = 16;
		PASSWD_CANT_CHANGE = 64;
		NORMAL_ACCOUNT = 512;
		DONT_EXPIRE_PASSWD = 65536
	}

	try {
		$computer = [ADSI]"WinNT://$computerName" 
		$user = $computer.Create("User", $userName) 
		$user.setpassword($password) 
		$user.put("description",$description)
		$user.put("fullname",$fullName)
		$user.SetInfo()
		
		return $true
	} catch {
		return $false
	}	
}
 
Function New-LocalGroup 
{ 
	[CmdletBinding()] 
	Param( 
		[Parameter(Position=0,Mandatory=$True,ValueFromPipeline=$True)] 
		[string]$GroupName,
		
		[string]$computerName = $env:ComputerName,
		
		[string]$description = ""
	) 

	try {
		$adsi = [ADSI]"WinNT://$computerName"
		$objgroup = $adsi.Create("Group", $groupName)
		$objgroup.SetInfo()
		$objgroup.description = $description
		$objgroup.SetInfo()
		
		return $true
	} catch {
		return $false
	}
}
 
Function Set-LocalGroup 
{ 
	[CmdletBinding()] 
	Param( 
		[Parameter(Position=0,Mandatory=$True,ValueFromPipeline=$True)] 
		[string]$userName,
		
		[Parameter(Position=1,Mandatory=$True,ValueFromPipeline=$True)]
		[string]$GroupName,
		
		[string]$computerName = $env:ComputerName,
		
		[Parameter(ParameterSetName='addUser')]
		[switch]$add,
		
		[Parameter(ParameterSetName='removeuser')]
		[switch]$remove
	)
	
	try {
		$group = [ADSI]"WinNT://$ComputerName/$GroupName,group" 
		if($add) 
		{ 
			$group.add("WinNT://$ComputerName/$UserName") 
		} 
		
		if($remove) 
		{ 
			$group.remove("WinNT://$ComputerName/$UserName") 
		}
		
		return $true
	} catch {
		return $false
	}
}
 
Function Set-LocalUserPassword 
{ 
	[CmdletBinding()] 
	Param( 
		[Parameter(Position=0,Mandatory=$True,ValueFromPipeline=$True)]
		[string]$userName,
		
		[Parameter(Position=1,Mandatory=$True,ValueFromPipeline=$True)] 
		[string]$password,
		
		[string]$computerName = $env:ComputerName
	)
	
	try {
		$user = [ADSI]"WinNT://$computerName/$username,user"
		$user.setpassword($password)
		$user.SetInfo()
		
		return $false
	} catch {
		return $false
	}
}
 
Function Set-LocalUser 
{ 
	[CmdletBinding()] 
	Param( 
		[Parameter(Position=0,Mandatory=$True,ValueFromPipeline=$True)]
		[string]$userName,
		
		[Parameter(Position=1,Mandatory=$True,ValueFromPipeline=$True,ParameterSetName='EnableUser')] 
		[string]$password,
		
		[Parameter(ParameterSetName='EnableUser')]
		[switch]$enable,
		
		[Parameter(ParameterSetName='DisableUser')]
		[switch]$disable,
		
		[string]$computerName = $env:ComputerName,
		
		[string]$description = ""
	)
 
	try {
		$EnableUser = 512 # ADS_USER_FLAG_ENUM enumeration value from SDK 
		$DisableUser = 2  # ADS_USER_FLAG_ENUM enumeration value from SDK 
		$User = [ADSI]"WinNT://$computerName/$userName,User" 
		  
		if($enable) 
		{ 
			$User.setpassword($password) 
			$User.description = $description 
			$User.userflags = $EnableUser 
			$User.setinfo() 
		}
		
		if($disable) 
		{ 
			$User.description = $description 
			$User.userflags = $DisableUser 
			$User.setinfo() 
		}
		
		return $true
	} catch {
		return $false
	}
}
 
Function Remove-LocalUser 
{ 
	[CmdletBinding()] 
	Param( 
		[Parameter(Position=0,Mandatory=$True,ValueFromPipeline=$True)]
		[string]$userName,
		
		[string]$computerName = $env:ComputerName
	)
	
	try {
		$User = [ADSI]"WinNT://$computerName" 
		$user.Delete("User",$userName)
		
		return $true
	} catch {
		return $false
	}
}
 
Function Remove-LocalGroup 
{
	[CmdletBinding()] 
	Param( 
		[Parameter(Position=0,Mandatory=$True,ValueFromPipeline=$True)]
		[string]$GroupName,
		
		[string]$computerName = $env:ComputerName
	)
	
	try {
		$Group = [ADSI]"WinNT://$computerName"
		$Group.Delete("Group",$GroupName)
		
		return $true
	} catch {
		return $false
	}
}
 
function Test-IsAdministrator 
{
	try {
		$currentUser = [Security.Principal.WindowsIdentity]::GetCurrent()
		(New-Object Security.Principal.WindowsPrincipal $currentUser).IsInRole([Security.Principal.WindowsBuiltinRole]::Administrator)
	} catch {
	}
}