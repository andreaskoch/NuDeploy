Function Add-HostfileExtension
{ 
	[CmdletBinding()]
	Param(
		[Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$extensionFilePath
	)
	
    $hostsPath = “$env:windir\System32\drivers\etc\hosts”
    $hostfileExtensions = Get-Content -Path $extensionFilePath
    $hostfileEntries = Get-Content $hostsPath
    
    $hostFileContentHasChanged = $false
    foreach($hostfileExtension in $hostfileExtensions)
    {
        if ((Array-HasMemberIgnoreWhitespace -array $hostfileEntries -member $hostfileExtension) -eq $false)
        {
            $hostfileEntries += $hostfileExtension
            $hostFileContentHasChanged = $true
        }
    }
    
    if ($hostFileContentHasChanged -eq $true)
    {
        $hostfileEntries | Out-File -FilePath $hostsPath -Encoding ASCII
    }
}

Function Remove-HostfileExtension
{ 
	[CmdletBinding()]
	Param(
		[Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$extensionFilePath
	)
	
    $hostsPath = “$env:windir\System32\drivers\etc\hosts”
    $hostfileExtensions = Get-Content -Path $extensionFilePath
    $hostfileEntries = Get-Content $hostsPath
    
    $newHostFileEntries = @()
    $hostFileContentHasChanged = $false
    foreach($hostfileEntry in $hostfileEntries)
    {
        if ((Array-HasMemberIgnoreWhitespace -array $hostfileExtensions -member $hostfileEntry) -eq $false)
        {
            $newHostFileEntries += $hostfileEntry
        }
        else
        {
            $hostFileContentHasChanged = $true
        }
    }
    
    if ($hostFileContentHasChanged -eq $true)
    {
        $newHostFileEntries | Out-File -FilePath $hostsPath -Encoding ASCII
    }
}

Function Array-HasMemberIgnoreWhitespace
{
	[CmdletBinding()]
	Param(
		[Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
		[array]$array,
        
		[Parameter(Position=1, Mandatory=$False, ValueFromPipeline=$True)]
		[string]$member=""
	)
    
    if (!$member -or $member -eq "")
    {
        return $false
    }
    
    $memberBare = $member -replace "[\s\t]", ""
    foreach ($arrayMember in $array)
    {
        $arrayMemberBare = $arrayMember -replace "[\s\t]", ""
        if ($arrayMemberBare -eq $memberBare)
        {
            return $true
        }
    }
    
    return $false
}