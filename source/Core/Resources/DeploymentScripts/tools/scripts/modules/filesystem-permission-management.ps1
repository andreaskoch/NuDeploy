# See:
# http://www.powershell.nu/2009/02/13/set-folder-permissions-using-a-powershell-script/
# http://technet.microsoft.com/de-de/library/dd315261.aspx
# http://technet.microsoft.com/en-us/library/ff730951.aspx


Function Add-FolderAccess
{
	param(
        [Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$Path,

        [Parameter(Position=1, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$Access,
        
        [Parameter(Position=2, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$Permission
    )

	# Get ACL on Folder
	$GetACL = Get-Acl $Path

	# Set up AccessRule
	$Allinherit = [system.security.accesscontrol.InheritanceFlags]"ContainerInherit, ObjectInherit"
	$Allpropagation = [system.security.accesscontrol.PropagationFlags]"None"
	$AccessRule = New-Object system.security.AccessControl.FileSystemAccessRule($Access, $Permission, $AllInherit, $Allpropagation, "Allow")

	# Check if Access Already Exists
	$computer = gc env:computername
	$localUsername = "$computer\$Access"

	if ($GetACL.Access | Where { ($_.IdentityReference -eq $Access) -or ($_.IdentityReference -eq $localUsername)})
    {
		"Modifying Permissions For: $Access"
		$AccessModification = New-Object system.security.AccessControl.AccessControlModification
		$AccessModification.value__ = 2
		$Modification = $False
		$GetACL.ModifyAccessRule($AccessModification, $AccessRule, [ref]$Modification) | Out-Null
	}
    else
    {
		"Adding Permission: $Permission For: $Access"
		$GetACL.AddAccessRule($AccessRule)
	}

	Set-Acl -aclobject $GetACL -Path $Path
	"Permission: $Permission Set For: $Access"
}

Function Remove-FolderAccess
{
	param(
        [Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$Path,

        [Parameter(Position=1, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$Access
    )

	# Get ACL on Folder
	$GetACL = Get-Acl $Path

	# Check if Access Already Exists
	$computer = gc env:computername
    $localUsername = "$computer\$Access"
    
	$accessRule = ($GetACL.Access | Where { ($_.IdentityReference -eq $Access) -or ($_.IdentityReference -eq $localUsername)})
	if ($accessRule)
    {
		"Removing folder access for user '$Access'"
		$GetACL.RemoveAccessRule($accessRule)
		
		Set-Acl -aclobject $GetACL -Path $Path
		"Folder Access Permissions revoked for user '$Access'"
	}
    else
    {
		"User '$Access' is not listed in the ACL of the folder '$Path'"
	}
}