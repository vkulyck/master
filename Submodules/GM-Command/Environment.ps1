function Initialize-Globals {
    if($null -eq $global:EntryScriptRoot) {
        Write-Debug "The variable `$global:EntryScriptRoot has not been set."
        $global:EntryScriptRoot = $(Get-EntryScriptRoot)
        Write-Debug "Using value deduced from stack frame: '$global:EntryScriptRoot'"
    }
    if(-Not [String]::IsNullOrWhiteSpace($global:SolutionRoot)) {
        $global:DataRoot = Join-Path -Path $global:SolutionRoot -ChildPath "SolutionData"
    }
    if(-Not [String]::IsNullOrWhiteSpace($global:DataRoot)) {
        $global:LogRoot ??= Join-Path -Path $global:DataRoot -ChildPath "logs"
        $global:BackupRoot ??= Join-Path -Path $global:DataRoot -ChildPath "backups"
        $global:TempRoot ??= Join-Path -Path $global:DataRoot -ChildPath "temp"
        $global:AssetsRoot ??= Join-Path -Path $global:DataRoot -ChildPath "assets"
    }
    $global:SqlServer ??= 'localhost'
    $global:SqlServerCredentials ??= @{}
    $global:AwsCredentials ??= @{ ProfileName = "gm-web-agent"; }
    $global:SubsystemHost ??= $Env:SUBSYSTEM_HOST
    if($IsWindows) {
        $global:SubsystemUser ??= ($Env:USERNAME).ToLower()
    } elseif($IsLinux) {
        $global:SubsystemUser ??= ($Env:USER).ToLower()
    }
}

function Initialize-Directories {
    param(
        [Parameter(Position=0,ValueFromPipeline=$true)]
        [String[]]$Directories = @(
            $global:LogRoot;
            $global:BackupRoot;
            $global:TempRoot;
            $global:AssetsRoot;
        )
    )
    process {
        foreach($directory in $Directories) {
            if([String]::IsNullOrWhiteSpace($directory)) {
                continue
            }
            if(-Not $(Test-Path -Path $directory -PathType Container)) {
                New-Item -Path $directory -ItemType Directory
            }
        }
    }
}

function Get-EntryScriptPath {
    $cs = $(Get-PSCallStack)
    $mainIdx = $cs.Length - 1
    $main = $cs[$mainIdx]
    while([String]::IsNullOrWhiteSpace($main.ScriptName)) {
        $main = $cs[--$mainIdx]
    }
    return $main.ScriptName
}

function Get-EntryScriptRoot {
    return [System.IO.Path]::GetDirectoryName($(Get-EntryScriptPath))
}

function Initialize-ScriptEnvironment {
    $path = [System.IO.Path]::Combine($global:EntryScriptRoot, "Environment.ps1")
    if(Test-Path $path) {
        . "$path"
    }
}
