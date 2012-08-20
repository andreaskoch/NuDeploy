# Deploy
param(
    [Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
    [string] $DeploymentType
)

"Executing Deploy.ps1 -DeploymentType $DeploymentType"

$packageDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$toolsDirectory = Join-Path $packageDirectory tools

$initScript = Get-ChildItem (Join-Path $toolsDirectory Init.ps1)
$installScript = Get-ChildItem (Join-Path $toolsDirectory Install.ps1)

"Executing the init script ($initScript)"
& $initScript

"Executing the install script ($installScript)"
& $installScript -DeploymentType $DeploymentType