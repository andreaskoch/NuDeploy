Function Get-IniFile($file)
{
	$ini = @{}
	switch -regex -file $file
	{
		"^\[(.+)\]$" {
			$section = $matches[1].Trim()
			$ini[$section] = @{}
		}
		
		"^\s*([^#].+?)\s*=\s*(.*)"
		{
			$name,$value = $matches[1..2]
			$ini[$section][$name] = $value.Trim()
		}
	}
	$ini
}

Function Write-IniFile($content, $targetFilePath)
{
    $iniFileContent = @()
    foreach($sectionKeyValuePair in $content.GetEnumerator())
    {
        $sectionName = $sectionKeyValuePair.Key
        $sectionContent = $sectionKeyValuePair.Value
        
        $iniFileContent += "[$sectionName]"
        if ($sectionContent)
        {
            foreach($contentKeyValuePair in $sectionContent.GetEnumerator())
            {
                $key = $contentKeyValuePair.Key
                $value = $contentKeyValuePair.Value
                
                $iniFileContent += "$key = $value"
            }
        }
    }
    $iniFileContent | Out-File -FilePath $targetFilePath -Encoding Unicode -Force
}

Function Export-LocalSecurityPolicies
{
	[CmdletBinding()]
	Param(
		[Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$areas,
		
		[Parameter(Position=1, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$targetPath
	)
    
    $command = "secedit /export /cfg `"$targetPath`" /areas `"$areas`" /quiet"
    Invoke-Expression $Command    
}

Function Get-LocalSecurityPolicies
{
	[CmdletBinding()]
	Param(
		[Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$areas
	)
    
    $tmpFile = "exported-security-policy.inf"
    Export-LocalSecurityPolicies -Areas $areas -TargetPath $tmpFile
    if ((Test-Path -Path $tmpFile) -eq $false)
    {
        return $null
    }
    
    $iniFileContent = Get-IniFile -File $tmpFile
    Remove-Item $tmpFile
    
    return $iniFileContent
}

Function Set-LocalSecurityPolicies
{
	[CmdletBinding()]
	Param(
		[Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
		[hashtable]$Policies,
            
		[Parameter(Position=1, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$areas
	)
    
    $databaseFile = "secedit.sdb"
    $tmpLogFile = "SetLocalSecurityPolicies.log"
    $tmpFile = "updatedsecuritypolicy.inf"
    
    Write-IniFile -Content $Policies -TargetFilePath $tmpFile
    if ((Test-Path -Path $tmpFile) -eq $false)
    {
        return $false
    }
    
    $command = "secedit /configure /db `"$databaseFile`" /cfg `"$tmpFile`" /areas `"$areas`" /log `"$tmpLogFile`""
    Invoke-Expression $Command
    
    Remove-Item $tmpFile
    Remove-Item $tmpLogFile
    Remove-Item $databaseFile
    
    return $true
}

Function LocalSecurityPolicyValue-Exists
{
	[CmdletBinding()]
	Param(
		[Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
		[hashtable]$Policies,
        
		[Parameter(Position=1, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$SectionName,
        
		[Parameter(Position=2, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$Key
	)
    
    $section = $Policies[$SectionName]
    $entry = $section[$Key]
    if ($entry)
    {
        return $true
    }
    
    return $false
}

Function Get-LocalSecurityPolicyValues
{
	[CmdletBinding()]
	Param(
		[Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
		[hashtable]$Policies,
        
		[Parameter(Position=1, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$SectionName,
        
		[Parameter(Position=2, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$Key
	)
    
    if ((LocalSecurityPolicyValue-Exists -Policies $Policies -SectionName $SectionName -Key $Key) -eq $false)
    {
        return @()
    }
    
    $section = $Policies[$SectionName]
    $entry = $section[$Key]
    if ($entry -and ($entry.Length -gt 0))
    {
        return $entry.Split(",")
    }
    
    return @()
}

Function Add-ValueToLocalSecurityPolicies
{
	[CmdletBinding()]
	Param(
		[Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
		[hashtable]$Policies,
        
		[Parameter(Position=1, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$SectionName,
        
		[Parameter(Position=2, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$Key,
        
		[Parameter(Position=3, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$Value
	)
    
    if ((LocalSecurityPolicyValue-Exists -Policies $Policies -SectionName $SectionName -Key $Key) -eq $false)
    {
        return $Policies
    }
    
    [String[]]$values = (Get-LocalSecurityPolicyValues -Policies $Policies -SectionName $SectionName -Key $Key)
    if ($values -contains $value)
    {
        return $Policies
    }
    
    $values += $value
    
    $section = $Policies[$SectionName]
    $section[$Key] = [string]::join(",", $values)
    
    return $Policies
}

Function Remove-ValueFromLocalSecurityPolicies
{
	[CmdletBinding()]
	Param(
		[Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
		[hashtable]$Policies,
        
		[Parameter(Position=1, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$SectionName,
        
		[Parameter(Position=2, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$Key,
        
		[Parameter(Position=3, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$Value
	)
    
    if ((LocalSecurityPolicyValue-Exists -Policies $Policies -SectionName $SectionName -Key $Key) -eq $false)
    {
        return $Policies
    }
    
    [String[]]$values = (Get-LocalSecurityPolicyValues -Policies $Policies -SectionName $SectionName -Key $Key)
    if (($values -contains $value) -eq $false)
    {
        return $Policies
    }
    
    $revisedValues = @()
    foreach ($val in $values)
    {
        if (-not ($val -eq $value))
        {
            $revisedValues += $val
        }
    }
    
    $section = $Policies[$SectionName]
    $section[$Key] = [string]::join(",", $revisedValues)
    
    return $Policies
}