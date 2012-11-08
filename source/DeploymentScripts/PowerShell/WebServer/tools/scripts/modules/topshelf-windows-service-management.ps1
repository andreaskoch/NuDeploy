
Function Install-TopShelfService {

    [CmdletBinding()]
    Param(
        [Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
        [string]$exePath,    
    
        [Parameter(Position=1, Mandatory=$False, ValueFromPipeline=$True)]
        [string]$name,
        
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
    if ($name) {
        $nameParameter = "-servicename:$name -displayname:$name"
    }
    
    if ($instance) {
        $instanceParameter = "-instance:$instance"
    }
    
    if ($description) {
        $descriptionParameter = "-description `"$description`""
    }
    
    if ($username -and $password) {
        $userParameter = "-username `"$username`" -password `"$password`""
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
    
    $cmd = "$exePath install $nameParameter $instanceParameter $descriptionParameter $userParameter $startParameter"
    Write-Host Installing TopShelf Service
    Write-Host $cmd
    Invoke-Expression $cmd
    
    if ($startAfterInstallation) {
        Write-Host Start Service After Installation
        & $exePath start
    }
}

Function Uninstall-TopShelfService {

    [CmdletBinding()]
    Param(
        [Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
        [string]$exePath,
    
        [Parameter(Position=1, Mandatory=$False, ValueFromPipeline=$True)]
        [string]$name,
        
        [Parameter(Position=2, Mandatory=$False, ValueFromPipeline=$True)]
        [string]$instance
    )

    if ($name) {
        $nameParameter = "-servicename:$name -displayname:$name"
    }
    
    if ($instance) {
        $instanceParameter = "-instance:$instance"
    }
    
    Write-Host Uninstalling TopShelf Service
    
    $cmd = "$exePath uninstall $nameParameter $instanceParameter"
    Write-Host $cmd
    
    Invoke-Expression $cmd

}