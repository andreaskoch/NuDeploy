
Function Publish-DacPac {

    [CmdletBinding()]
    Param(

		[Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
        [string]$sqlPackageExePath,    

        [Parameter(Position=1, Mandatory=$True, ValueFromPipeline=$True)]
        [string]$packagePath,    
    
        [Parameter(Position=2, Mandatory=$False, ValueFromPipeline=$True)]
        [string]$publishProfilePath
       
    )

    
    Write-Host Publishing DacPac

	& $sqlPackageExePath /Action:Publish /SourceFile:$packagePath /Profile:$publishProfilePath
   
}

