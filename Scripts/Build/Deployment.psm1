using namespace System.Collections.Specialized
using namespace System.Console

Set-Alias aws $(Get-Command aws.exe).Source
Set-Alias meld $(Get-Command meld.exe).Source
Set-Alias rbcp $(Get-Command Robocopy.exe).Source

function New-WslSession {
  New-PSSession -HostName $script:WslHost -UserName $script:WslUserName
}

function Read-Configs {
  param(
    [Parameter(Mandatory)]
    [Alias('Env')]
    [String] $TargetEnvironment,
    [Parameter(Mandatory)]
    [Alias('Project')]
    [String[]] $TargetProjects
  )
  begin {
    $hosting_config = $(Read-JsonConfig "$PSScriptRoot/deploy-data/local-configs/hosting.json")
    $project_config = $(Read-JsonConfig "$PSScriptRoot/deploy-data/local-configs/projects.json")
  }
  process {
    Write-FunctionBegin "Loading deployment settings and server configuration variables."
    $target_hosts = $hosting_config.hosts[$TargetEnvironment]
    $target_project_configs = $project_config | ? { $TargetProjects -contains $_.id }
    $config = @{
      data = [ordered]@{
        elb1 = $target_hosts.elb1;
        elb2 = $target_hosts.elb2;
        vpc = $TargetEnvironment;
        owner = $hosting_config.access.websvc.user;
        group = $hosting_config.access.websvc.group;
        prj = $target_project_configs;
      };
      settings = @{
        aws = $hosting_config.access.aws;
        websvc = $hosting_config.access.websvc;
        target = @{
          env = $TargetEnvironment;
          servers = $target_hosts;
          projects = $TargetProjects;
        };
        verbose = ($VerbosePreference -ne 'SilentlyContinue');
      };
    };
    Write-FunctionEnd
    return $config
  }
}

function Initialize-DeploySettings {
  param(
    [Parameter(Mandatory)]
    $DeploySettings
  )
  Write-FunctionBegin "Initializing script variables from deployment settings."
  $Env:AWS_PROFILE = $DeploySettings.aws.agent
  $global:DeployProjects = $DeploySettings.target.projects

  $script:DeployEnv = $DeploySettings.target.env
  $script:DeployAgent = $DeploySettings.aws.agent
  $script:DeployDataBucket = $DeploySettings.aws.bucket
  $script:DeploymentOwner = $DeploySettings.websvc.user
  $script:DeploymentGroup = $DeploySettings.websvc.group
  $script:DeployServers = $DeploySettings.target.servers
  $script:WslHost = $DeploySettings.wsl.domain
  $script:WslUserName = $DeploySettings.wsl.user
  $script:QuietDeploy = $(-not $DeploySettings.verbose)

  $script:LocalDeployBackupRoot = "$global:DataRoot/Deployment/backups/$script:DeployEnv"
  $script:LocalDeployDataRoot = "$global:DataRoot/Deployment/configs/$script:DeployEnv"
  $script:RemoteDeployBackupRoot = "s3://$script:DeployDataBucket/backups/$script:DeployEnv"
  $script:RemoteDeployDataRoot = "s3://$script:DeployDataBucket/configs/$script:DeployEnv"
  $script:LocalServerConfigs = "$script:LocalDeployDataRoot/servers"
  $script:LocalProjectConfigs = "$script:LocalDeployDataRoot/apps/projects"
  $script:LocalDatasetConfigs = "$script:LocalDeployDataRoot/apps/datasets"

  $script:PublishRoot = Convert-WslToUnc "/srv/gmweb"

  if(-Not $(Test-WslHostsMapping -WslHost $script:WslHost)) {
    throw "Invalid WSL host detected; run 'update-wsl-host.ps1' to map the correct hostname."
  }
  if(-Not $(Test-WslSshStatus)) {
    throw "WSL host is not running SSHD; run 'update-wsl-host.ps1' to start the SSHD service."
  }
  Start-WslSsh
  Write-FunctionEnd
}

function Get-SourceMaps {
  $source_maps = @(
    @{
      Source = "$PSScriptRoot/deploy-data/host-service-template-output/etc";
      Dest = '/etc';
    };
    @{
      Source = "$PSScriptRoot/deploy-data/host-assets/srv/gmweb";
      Dest = '/srv/gmweb';
      EnableLinuxReformat = $false;
    };
    @{
      Source = "$PSScriptRoot/deploy-data/host-scripts/srv/home";
      Dest = '/srv/home';
    };
    @{
      Source = "$PSScriptRoot/deploy-data/host-scripts/srv/scripts";
      Dest = '/srv/scripts';
    };
    @{
      Source = "$PSScriptRoot/deploy-data/host-configs/etc";
      Dest = '/etc';
    };
    @{
      Source = "$global:SolutionRoot/Submodules/GM-Command";
      Dest = '/srv/gmcmd';
    };
  )
  return $source_maps
}

function Backup-RemoteData {
  Write-FunctionBegin "Backing up configs from S3 bucket $script:DeployDataBucket..."
  $temp_dir = New-TempDirectory
  Invoke-Disposable ($temp_dir) {
    $backup_name = "$(New-Timestamp)-$script:DeployEnv-config-backup".ToLower()
    $archive_path = "$script:LocalDeployBackupRoot/$backup_name.7z"
    $archive_source_path = Join-Path -Path $temp_dir -ChildPath $backup_name
    if(-Not $(Test-Path $archive_source_path)) {
      New-Directory $archive_source_path
    }
    aws s3 sync "$script:RemoteDeployDataRoot/" "$archive_source_path/"
    $source_count = $(Get-ChildItem $archive_source_path).Length
    if(0 -eq $source_count) {
      return;
    }
    if(Test-Path $archive_path) {
      Remove-Item -Path $archive_path -ErrorAction Ignore
    }
    Compress-With7zCli -ArchivePath $archive_path -SourcePath $archive_source_path -Unencrypted
    aws s3 sync "$script:LocalDeployBackupRoot/" "$script:RemoteDeployBackupRoot/"
  }
  Write-FunctionEnd
}

function Sync-DataStores {
  Write-FunctionBegin "Synchronizing S3 config files."
  if(-not $script:QuietDeploy) {
    aws s3 sync --delete "$script:LocalDeployDataRoot" "$script:RemoteDeployDataRoot" --dryrun
    $response = Request-Confirmation "Confirm S3 operations and press any key to continue:"
    if(-Not $response) {
      Write-Warning "Exiting the script."
      Exit
    }
  }
  aws s3 sync --delete "$script:LocalDeployDataRoot" "$script:RemoteDeployDataRoot"
  Write-FunctionEnd
}

function Build-Projects {
  param(
    [Parameter(Mandatory)]
    [string[]]$ProjectIDs
  )
  Write-FunctionBegin "Performing local build to WSL system."
  Push-Location $global:SolutionRoot
  $solutionFile = Join-Path -Path $global:SolutionRoot -ChildPath "GmWeb.sln"
  foreach($projectID in $ProjectIDs) {
    Write-FunctionBegin "Building $projectID."
    $projectName = "GmWeb.Web.$projectID"
    $publishProfile = "$global:SolutionRoot/$projectName/Properties/PublishProfiles/$projectID.local.pubxml"
    dotnet build $solutionFile /p:PublishProfile="$publishProfile" /p:DeployOnBuild=true
    Write-FunctionEnd
  }
  Pop-Location
  Write-FunctionEnd
}

function Publish-AppSettings {
  param(
    [Parameter(Mandatory)]
    [string[]]$ProjectIDs
  )
  Write-FunctionBegin "Publishing appsettings to projects: $($ProjectIDs -Join ', ')."
  foreach($projectID in $ProjectIDs) {
    $project_deploy_path = Join-Path $script:PublishRoot -ChildPath $projectID.ToLower()
    Push-Location $project_deploy_path
    Remove-Item -Path @(
      'appsettings.json'; 'appsettings.*.json'; 'Settings/';
    ) -Recurse -ErrorAction SilentlyContinue
    New-Directory 'Settings'
    Copy-Item -Path $script:LocalDatasetConfigs -Destination Settings/ -Recurse
    Copy-Item -Path $script:LocalProjectConfigs/appsettings.$projectID.json -Destination Settings/
    Pop-Location
  }
  Write-FunctionEnd
}

function Get-RsyncCommand {
  param(
    [String[]] $LocalPaths = @(),
    [String[]] $ServerPaths = @(),
    [Parameter(Mandatory)]
    [String] $Server,
    [Parameter(Mandatory)]
    [ValidateSet('Deploy','Backup')]
    [string] $Action,
    [Switch] $Delete = $false,
    [Switch] $Relative = $false
  )
  # TODO: Add support for list files
  $rsync_args = @(
    "rsync";
    # -v, --verbose: Verbose console output.
    # -z, --compress: Compress for net transfer.
    "--verbose"; "--compress"; "--human-readable";
  )
  if($Delete) {
    $rsync_args += "--delete";
  }
  if($Relative) {
    $rsync_args += "--relative";
  }
  if('Backup' -eq $Action) {
    if($LocalPaths.Count -gt 1) {
      throw [System.ArgumentException]::new("Requested an rsync-backup command with more than one destination.", 'LocalPaths')
    }
    elseif($LocalPaths.Count -eq 0) {
      $LocalPaths = $(Convert-WinToWsl ".")
    }
    $server_path_list = $($ServerPaths | % { ":`"$_`"" }) -join " "
    $rsync_args += @(
      # -a, --archive: Enable archive mode [-rlptgoD (no -H,-A,-X)]
      "--archive";
      "--ignore-missing-args";
      "--exclude='wwwroot'"
      # Backup: Server -> Local
      "$Server$server_path_list"; "`"$LocalPaths`"";
    )
  } elseif('Deploy' -eq $Action) {
    if($ServerPaths.Count -gt 1) {
      throw [System.ArgumentException]::new("Requested an rsync-deploy command with more than one destination.", 'ServerPaths')
    }
    elseif($ServerPaths.Count -eq 0) {
      $ServerPaths = @('/';)
    }
    $local_path_list = $($LocalPaths | % { "`"$_`"" }) -join " "
    $rsync_args += @(
      # -p, --perms: Enable permission copying.
      # -o, --owner: Enable owner assignment.
      # -g, --group: Enable group assignment.
      # -r, --recursive: Recurse source path.
      # --rsync-path: Set the remote 'rsync' command.
      "--perms"; "--owner"; "--group"; "--recursive"; 
      "--chown=$script:DeploymentOwner`:$script:DeploymentGroup";
      "--chmod=Dug=rwx,Do=rx,Fug=rw,Fo=r";
      "--rsync-path 'sudo rsync'";
      # Deploy: Local -> Server
      "$local_path_list"; "$Server`:`"$ServerPaths`"";
    )
  }
  $rsync_cmd = $rsync_args -Join ' '
  return $rsync_cmd
}

function Deploy-Projects {
  param(
    [Parameter(Mandatory=$true)]
    [string[]]$ProjectIDs
  )
  Write-FunctionBegin "Deploying local WSL to $script:DeployEnv Servers."
  $processed_servers = @{}
  $session = New-WslSession
  foreach($projectID in $ProjectIDs) {
    Write-FunctionBegin "Deploying $projectID."
    $servers = $script:DeployServers[$projectID]
    foreach($server in $servers) {
      Write-FunctionBegin "Transferring $projectID to $server."
      $rsync_cmd = Get-RsyncCommand -LocalPaths "/srv/gmweb/$projectID" -ServerPaths "/srv/gmweb/" -Server $server -Action Deploy -Delete
      Invoke-Command -Session $session -Scriptblock {
        Invoke-Expression -Command $using:rsync_cmd
      }
      $processed_servers[$server] = $true
      Write-FunctionEnd
    }
    Write-FunctionEnd
  }
  foreach($server in $processed_servers.Keys) {
    Write-FunctionBegin "Reloading deployed services on $server."
    Invoke-Command -Session $session -Scriptblock {
      ssh $using:server "sudo systemctl daemon-reload";
      ssh $using:server "sudo /srv/scripts/gmrun";
    }
    Write-FunctionEnd
  }
  Remove-PSSession $session
  Write-FunctionEnd
}

function Initialize-Scripts {
  param(
    [Parameter(Mandatory=$true)]
    [string[]]$ProjectIDs
  )
  Write-FunctionBegin "Setting permissions on $script:DeployEnv instance scripts."
  $processed_servers = @{}
  $session = New-WslSession
  foreach($projectID in $ProjectIDs) {
    $servers = $script:DeployServers[$projectID]
    foreach($server in $servers) {
      if($processed_servers.ContainsKey($server)) {
        continue;
      }
      Invoke-Command -Session $session -Scriptblock {
        ssh $using:server "bash /srv/scripts/reset-permissions.sh"
      }
      $processed_servers[$server] = $true
    }
  }
  Invoke-Command -Session $session -Scriptblock {
    ssh $using:server "ln -sf /srv/home/dot_addendum ~/.addendum"
    ssh $using:server "ln -sf /srv/home/dot_bash_prompt ~/.bash_prompt"
    ssh $using:server "ln -sf /srv/home/dot_git_prompt ~/.git_prompt"
    ssh $using:server "ln -sf /srv/home/dot_vimrc ~/.vimrc"
    ssh $using:server "ln -sfn /srv/home/dot_vim ~/.vim"
  }
  Remove-PSSession $session
  Write-FunctionEnd
}

function Sync-ServerSettings {
  param(
    [Parameter(Mandatory=$true)]
    [string[]]$ProjectIDs,
    [Parameter(Mandatory=$true)]
    [ValidateSet('Deploy','Backup')]
    [String] $Action
  )
  Write-FunctionBegin "Performing $($Action.ToString().ToLower()) on $script:DeployEnv service configs."
  Push-Location "$script:LocalServerConfigs/deploy"

  $etc_files = Get-ChildItem 'etc' -Recurse -File | % { $(Resolve-Path -Relative $_) -replace '^\.','' -replace '\\','/' }
  $sources = @(
    @{ paths = $etc_files; delete = $false; };
    @{ paths = @( '/srv/gmcmd/'; '/srv/scripts/'; '/srv/home/';); delete = $true; };
  )
  $processed_servers = @{}
  $session = New-WslSession
  foreach($projectID in $ProjectIDs) {
    $servers = $script:DeployServers[$projectID]
    foreach($server in $servers) {
      if($processed_servers.ContainsKey($server)) {
        continue;
      }
      if($Action -eq 'Backup') {
        Push-EmphemeralLocation "$script:LocalServerConfigs/current/$server"
      }
      foreach($source in $sources) {
        if($Action -eq 'Backup') {
          $paths = $source.paths
          $rsync_cmd = Get-RsyncCommand -Relative -ServerPaths $paths -Server $server -Action $Action -Delete:$source.delete
        } elseif($Action -eq 'Deploy') {
          $wsl_root = $("." | Convert-WinToWsl)
          $paths = $source.paths | % { "$wsl_root.$_" } # Adding a period int the path sets the root for rsync --relative
          $rsync_cmd = Get-RsyncCommand -Relative -LocalPaths $paths -Server $server -Action $Action -Delete:$source.delete
        }
        Invoke-Command -Session $session -Scriptblock {
          Invoke-Expression $using:rsync_cmd
        }
      }
      if($Action -eq 'Backup') {
        Pop-Location
      }
      $processed_servers[$server] = $true
    }
  }
  Pop-Location
  Remove-PSSession $session
  Write-FunctionEnd "Performing $($Action.ToString().ToLower()) on $script:DeployEnv service configs."
}

function Compare-ProjectSettings {
  param(
    [Parameter(Mandatory=$true)]
    [string[]]$ProjectIDs
  )
  if($script:QuietDeploy) {
    return
  }
  foreach($projectID in $ProjectIDs) {
    $active_settings = "$script:PublishRoot/$ProjectID/Settings/appsettings.$ProjectID.json"
    $stored_settings = "$script:LocalProjectConfigs/appsettings.$ProjectID.json"
    meld $active_settings $stored_settings
    Request-Confirmation "Confirm setting synchronization and press any key to continue:"
  }
}

function Sync-InstanceScripts {
  param(
    [Parameter(Mandatory=$true)]
    [Hashtable[]] $SourceMaps
  )
  Write-FunctionBegin "Synchronizing source control files with pre-deployment directory."
  # Robocopy source scripts with /MIR to remove unmapped files in the destination
  $stage_root = "$script:LocalServerConfigs/deploy"
  if(Test-Path -Path $stage_root) {
    Remove-Item $stage_root -Recurse
    New-Directory $stage_root
  }
  foreach($map in $SourceMaps) {
    $dest = Join-Path -Path $stage_root -ChildPath $map.Dest
    if(-not $(Test-Path $dest -PathType Container)) {
      New-Directory $dest
    }
    rbcp $map.Source $dest /E /XF key /XF .git* /XD .vscode
    if(-Not $map.Contains('EnableLinuxReformat')) {
      $map['EnableLinuxReformat'] = $true
    }
    if($map['EnableLinuxReformat']) {
      Push-Location $dest
      Write-FunctionBegin "Reformatting files for linux in [deploy-root]$($map.Dest)"
      $scripts = $(Get-ChildItem -Recurse -File)
      foreach($script in $scripts) {
        $orig = Get-Content -Path $script -Raw
        if($orig.Contains("`r`n")) {
          $repl = $orig -replace "`r`n","`n"
          $repl | Set-Content $script -NoNewline
        }
      }
      Write-FunctionEnd
      Pop-Location
    }
  }
  Write-FunctionEnd
}

function Format-HostServiceConfigs {
  param(
    [IOrderedDictionary] $DataSpace
  )
  Write-FunctionBegin "Processing configuration templates and formatting output."
  $InputDirectory = "$PSScriptRoot/deploy-data/host-service-templates"
  $OutputDirectory = "$PSScriptRoot/deploy-data/host-service-template-output"

  Remove-Item -Path $OutputDirectory -Recurse -ErrorAction SilentlyContinue
  if(-not $(Test-Path -Path $OutputDirectory -PathType Container)) {
    New-Directory $OutputDirectory
  }
  $tuples = Get-DataTuples -DataSpace $DataSpace | Convert-NestedTuple
  foreach($tuple in $tuples) {
    Format-TemplateDirectory -Data $tuple -InDir $InputDirectory -OutDir $OutputDirectory
  }
  Write-FunctionEnd
}

Export-ModuleMember -Function *
