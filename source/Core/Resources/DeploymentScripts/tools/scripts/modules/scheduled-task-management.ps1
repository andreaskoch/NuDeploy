# see: http://msdn.microsoft.com/en-us/library/windows/desktop/bb736357(v=vs.85).aspx

function Get-ScheduledTask
{
    [CmdletBinding()]

    param(
        [Parameter(
        Mandatory=$false,
        ValueFromPipeline=$true,
        ValueFromPipelineByPropertyName=$true)]
        [String[]]$ComputerName = "localhost",

        [Parameter(Mandatory=$false)]
        [String[]]$RunAsUser,

        [Parameter(Mandatory=$false)]
        [String[]]$TaskName,

        [parameter(Mandatory=$false)]
        [alias("WS")]
        [switch]$WithSpace
     )

    Begin
    {

        $Script:Tasks = @()
    }

    Process
    {
        $schtask = schtasks.exe /query /s $ComputerName  /V /FO CSV | ConvertFrom-Csv

        if ($schtask)
        {
            foreach ($sch in $schtask)
            {
                if ($sch."Run As User" -match "$($RunAsUser)" -and $sch.TaskName -match "$($TaskName)")
                {
                    $sch  | Get-Member -MemberType Properties | ForEach -Begin {$hash=@{}} -Process {
                        If ($WithSpace)
                        {
                            ($hash.($_.Name)) = $sch.($_.Name)
                        }
                        Else
                        {
                            ($hash.($($_.Name).replace(" ",""))) = $sch.($_.Name)
                        }
                    } -End {
                        $script:Tasks += (New-Object -TypeName PSObject -Property $hash)
                    }
          }
            }
        }
    }

    End
    {
        $Script:Tasks
    }
}

Function Remove-ScheduledTask
{
	param(
        [Parameter(Position=0, Mandatory=$False, ValueFromPipeline=$True)]
		[string]$ComputerName = "localhost",
        
        [Parameter(Position=1, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$TaskName,
		
        [Parameter(Position=2, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$TaskLocation
	)
    
	if ((Exists-ScheduledTask -ComputerName $ComputerName -TaskName $TaskName -TaskLocation $TaskLocation) -eq $false)
	{
        return $false
    }    
	
    $taskLocationAndName = Join-Path $TaskLocation $TaskName
    $Command = "schtasks.exe /delete /s '$ComputerName' /tn '$taskLocationAndName' /F"
    Invoke-Expression $Command
    
    return $true
}

Function Remove-ScheduledTaskFolder
{
	param(
        [Parameter(Position=0, Mandatory=$False, ValueFromPipeline=$True)]
		[string]$ComputerName = "localhost",
		
        [Parameter(Position=1, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$TaskLocation
	)
    
    $Command = "schtasks.exe /delete /s '$ComputerName' /tn '$TaskLocation' /F"
    Invoke-Expression $Command
    return $true
}

Function Exists-ScheduledTask
{
	param(
        [Parameter(Position=0, Mandatory=$False, ValueFromPipeline=$True)]
		[string]$ComputerName = "localhost",
        
        [Parameter(Position=1, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$TaskName,
		
        [Parameter(Position=2, Mandatory=$False, ValueFromPipeline=$True)]
		[string]$TaskLocation=""
	)
	
    $TaskNameIncludingDirectory = Join-Path \ $TaskLocation
    $TaskNameIncludingDirectory = Join-Path $TaskNameIncludingDirectory $TaskName
    
    $scheduledTask = (Get-ScheduledTask -ComputerName $ComputerName -TaskName $TaskName)
	if ($scheduledTask -and ($scheduledTask.TaskName -eq $TaskNameIncludingDirectory))
	{
		return $true
	}
	else
	{
		return $false
	}
}

Function CreateOrUpdate-ScheduledTask
{
	param(
        [Parameter(Position=0, Mandatory=$False, ValueFromPipeline=$True)]
		[string]$ComputerName = "localhost",
        
        [Parameter(Position=1, Mandatory=$False, ValueFromPipeline=$True)]
		[string]$RunAsUser="System",
        
        [Parameter(Position=2, Mandatory=$False, ValueFromPipeline=$True)]
		[string]$RunAsUserPassword,
        
        [Parameter(Position=3, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$TaskName,
		
        [Parameter(Position=4, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$TaskLocation,		
        
        [Parameter(Position=5, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$TaskRun,
        
        [Parameter(Position=6, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$Schedule = "Daily",
        
        [Parameter(Position=7, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$StartTime,
        
        [Parameter(Position=8, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$EndTime,
        
        [Parameter(Position=9, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$Interval = "1"
	)
	
	if ((Exists-ScheduledTask -ComputerName $ComputerName -TaskName $TaskName -TaskLocation $TaskLocation) -eq $true)
	{
		Remove-ScheduledTask -ComputerName $ComputerName -TaskName $TaskName -TaskLocation $TaskLocation
	}
	
	Create-ScheduledTask -ComputerName $ComputerName -TaskName $TaskName -TaskLocation $TaskLocation -TaskRun $TaskRun -RunAsUser $RunAsUser -RunAsUserPassword $RunAsUserPassword -Schedule $Schedule -StartTime $StartTime -EndTime $EndTime -Interval $Interval
}

Function Create-ScheduledTask
{
	param(
        [Parameter(Position=0, Mandatory=$False, ValueFromPipeline=$True)]
		[string]$ComputerName = "localhost",
        
        [Parameter(Position=1, Mandatory=$False, ValueFromPipeline=$True)]
		[string]$RunAsUser="System",
        
        [Parameter(Position=2, Mandatory=$False, ValueFromPipeline=$True)]
		[string]$RunAsUserPassword,
        
        [Parameter(Position=3, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$TaskName,
		
        [Parameter(Position=4, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$TaskLocation,
        
        [Parameter(Position=5, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$TaskRun,
        
        [Parameter(Position=6, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$Schedule = "Daily",
        
        [Parameter(Position=7, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$StartTime,
        
        [Parameter(Position=8, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$EndTime,
        
        [Parameter(Position=9, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$Interval = "1"
	)
	$taskLocationAndName = Join-Path $TaskLocation $TaskName
	$Command = "schtasks.exe /create /s `"$ComputerName`" /tn `"$taskLocationAndName`" /tr `"$TaskRun`" /sc $Schedule /mo $Interval /st $StartTime /et $EndTime /ru `"$RunAsUser`" /rp `"$RunAsUserPassword`" /F"
	Invoke-Expression $Command
}