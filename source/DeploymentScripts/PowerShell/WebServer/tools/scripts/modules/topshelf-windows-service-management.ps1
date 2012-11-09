
Function Install-TopShelfService {

    [CmdletBinding()]
    Param(
        [Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
        [string]$exePath,    
    
        [Parameter(Position=1, Mandatory=$False, ValueFromPipeline=$True)]
        [string]$displayName,
        
        [Parameter(Position=2, Mandatory=$False, ValueFromPipeline=$True)]
        [string]$instance,
        
        [Parameter(Position=3, Mandatory=$False, ValueFromPipeline=$True)]
        [string]$username,
        
        [Parameter(Position=4, Mandatory=$False, ValueFromPipeline=$True)]
        [string]$password,
        
        [Parameter(Position=5, Mandatory=$False, ValueFromPipeline=$True)]
        [string]$description,
        
        [Parameter(Position=6, Mandatory=$False, ValueFromPipeline=$True)]
        [string]$start,
        
        [Parameter(Position=7, Mandatory=$False, ValueFromPipeline=$True)]
        [switch]$startAfterInstallation
    )

    # ServiceName and DisplayName are always the same
    if ($displayName) {
        $displayNameParameter = "-servicename:$displayName -displayname:$displayName"
    }
    
    if ($instance) {
        $instanceParameter = "-instance:$instance"
    }
    
    switch ($start)
    {
        "autostart" {
            $startParameter = "--autostart"
        }
        "delayed" {
            $startParameter = "--delayed"
        }
        "disabled" {
            $startParameter = "--disabled"
        }
        "manual" {
            $startParameter = "--manual"
        }
    }
    
    Write-Host Installing TopShelf Service
    # the duplicity here is pretty ugly but the only way I found to allow spaces and special characters within these parameters
    if ($description) {
        if ($username -and $password) {
            & $exePath install $displayNameParameter $instanceParameter $startParameter -description `"$description`" -username `"$username`" -password `"$password`"
        } else {
            & $exePath install $displayNameParameter $instanceParameter $startParameter -description `"$description`"
        }
    } else {
        if ($username -and $password) {
            & $exePath install $displayNameParameter $instanceParameter $startParameter -username `"$username`" -password `"$password`"
        } else {
            & $exePath install $displayNameParameter $instanceParameter $startParameter
        }
    }
    
    if ($startAfterInstallation) {
        Write-Host Start Service After Installation
        $startCmd = "$exePath start $displayNameParameter $instanceParameter"
        Invoke-Expression $startCmd
    }
}

Function Uninstall-TopShelfService {

    [CmdletBinding()]
    Param(
        [Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
        [string]$exePath,
    
        [Parameter(Position=1, Mandatory=$False, ValueFromPipeline=$True)]
        [string]$displayName,
        
        [Parameter(Position=2, Mandatory=$False, ValueFromPipeline=$True)]
        [string]$instance
    )

    if ($displayName) {
        $displayNameParameter = "-servicename:$displayName -displayname:$displayName"
    }
    
    if ($instance) {
        $instanceParameter = "-instance:$instance"
    }
    
    Write-Host Uninstalling TopShelf Service
    
    $cmd = "$exePath uninstall $displayNameParameter $instanceParameter"
    Write-Host $cmd
    
    Invoke-Expression $cmd

}