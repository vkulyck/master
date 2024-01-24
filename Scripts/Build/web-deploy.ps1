[CmdletBinding()]
param(
  [Parameter(Mandatory)]
  [Alias('Env')]
  [ValidateSet('dev','team','beta','staging')]
  [String[]] $TargetEnvironments,
  [Parameter(Mandatory)]
  [Alias('Projects')]
  [ValidateSet('api','identity','rhi')]
  [String[]] $TargetProjects
)

begin {
  Import-Module ./Deployment.psm1 -Force
  . "$PSScriptRoot/init.ps1"
}
process {
  Start-Log "web-deploy"
  foreach($env in $TargetEnvironments) {
    $Configs = Read-Configs -TargetEnvironment $env -TargetProjects $TargetProjects
    Initialize-DeploySettings -DeploySettings $Configs.settings | Out-Null
    Compare-ProjectSettings -ProjectIDs $script:DeployProjects | Out-Null

    # # Format templates and generate host configs
    Format-HostServiceConfigs -DataSpace $Configs.data | Out-Null

    # # Copy version-controlled and formatted scripts to the pre-server-deployment directory
    $SourceMaps = Get-SourceMaps
    Sync-InstanceScripts -SourceMaps $SourceMaps | Out-Null

    # Backup service configs from the hosting servers to local server-specific backup directories
    Sync-ServerSettings -Action Backup -ProjectIDs $script:DeployProjects | Out-Null

    # Synchronize local configs to the remote data store and then archive the synchronized configs
    Sync-DataStores | Out-Null
    Backup-RemoteData | Out-Null

    # Deploy service configs from local storage to the hosting servers and set permissions
    Sync-ServerSettings -Action Deploy -ProjectIDs $script:DeployProjects | Out-Null
    Initialize-Scripts -ProjectIDs $script:DeployProjects | Out-Null

    # Build projects locally, publish to the WSL instance, and deploy from WSL to the hosting servers
    Build-Projects -ProjectIDs $script:DeployProjects | Out-Null
    Publish-AppSettings -ProjectIDs $script:DeployProjects | Out-Null
    Deploy-Projects -ProjectIDs $script:DeployProjects | Out-Null
  }
  Stop-Log
}