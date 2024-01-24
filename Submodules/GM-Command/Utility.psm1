using namespace System.Security.Principal
Import-Module 7Zip4Powershell
if($IsWindows) {
    Set-Alias 7z "$env:ProgramW6432\7-Zip\7z.exe"
} elseif($IsLinux) {
    Set-Alias 7z "/bin/7z"
}

function Out-Log($msg) {
    if([string]::IsNullOrWhiteSpace($Environment)) {
        $s_env = "";
    } else {
        $s_env = "($Environment)";
    }
    Write-Host "[$(Get-Date -UFormat "%Y.%m.%d.%H.%M.%S")] $msg $s_env"
}

function Get-EntryScriptName() {
    $caller_path = $(Get-EntryScriptPath)
    $caller_name = [System.IO.Path]::GetFileNameWithoutExtension($caller_path)
    return $caller_name
}

function Invoke-Disposable {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [AllowEmptyCollection()]
        [AllowNull()]
        [System.IDisposable]
        $InputObject,

        [Parameter(Mandatory = $true)]
        [scriptblock]
        $ScriptBlock
    )
    try
    {
        . $ScriptBlock
    }
    finally
    {
        if ($null -ne $InputObject -and $InputObject -is [System.IDisposable])
        {
            $InputObject.Dispose()
        }
    }
}

function Get-Timestamp($Value) {
    $success = $($Value -match '\d{4}\.\d{2}\.\d{2}\.\d{2}\.\d{2}\.\d{2}')
    if(-Not $success) { 
        return $null
    }
    $match = $matches[0]
    $dt = [DateTime]::ParseExact($match, 'yyyy.MM.dd.HH.mm.ss', $null)
    return $dt
}

function New-Timestamp {
    $timestamp = $(Get-Date -UFormat "%Y.%m.%d.%H.%M.%S")
    return $timestamp
}

Class TempDirectory: System.IDisposable { 
    [String] $Path
    [String] $Timestamp
    [Boolean] $Persist = $false
    [TempDirectory] static Create() {
        $instance = [TempDirectory]::new()
        $instance.Timestamp = $(New-Timestamp)
        $temp_path = [IO.Path]::Combine($global:TempRoot, $instance.Timestamp)
        $instance.Path = $(New-Item -Path $temp_path -ItemType Directory)
        New-Directory -Path $instance.Path
        return $instance
    }
    [void] Dispose() {
        if(-Not $this.Persist) {
            Remove-Item -LiteralPath "$($this.Path)" -Force -Recurse
        }
    }
    # ToString Method
    [String] ToString()
    {
        return $this.Path
    }
}

function New-TempDirectory {
    return [TempDirectory]::Create()
}

function Test-BuildTime($source_directory) {
    # Get the most recently written file in the source directory and determine its build time
    $(gci -Path "$source_directory" | sort LastWriteTime | select -last 1 | %{ $build_time =  $_.LastWriteTime })

    if(!$build_time) {
        Write-Error "No build time identified - make sure source directory exists: $source_directory"
        exit
    }

    # Timespan is [current time] - [build time]
    $ts = $(Get-Date) - $build_time
    $max_difference_in_minutes = 5;

    if($ts.TotalMinutes -gt $max_difference_in_minutes) {
        $message = "Last $build_type build happened {0:n0} minutes ago - continue?" -f $ts.TotalMinutes;
        $confirmation = Read-Host $message; 
        if ($confirmation -eq 'y') {
          Write-Host "Proceeding...";
        } else { 
            Write-Host "Aborting build validation: $(Get-Date -UFormat "%Y.%m.%d.%H.%M.%S")"
            exit;
        }
    } else {
        Write-Host "Source validated: $source_directory";
    }
}

function Start-RemoteScript() {
    Param(
      [Parameter(Mandatory=$true)]
      [Alias("Script")]
      [string]$ScriptPath,
      [Parameter(Mandatory=$true)]
      [Alias("Args")]
      [string]$ScriptArgs,
      [Parameter(Mandatory=$true)]
      [Alias("Host")]
      [string]$SshHost
    )
    $prev = $ErrorActionPreference
    $ErrorActionPreference = "Continue"
    Write-Host "Running command on $SshHost`: `"$ScriptPath`" $Args"
    ssh $SshHost "pwsh `"$ScriptPath`" $ScriptArgs" 2>$null
    $ErrorActionPreference = $prev
}

function Start-RemoteTask {
    Param(
      [Parameter(Mandatory=$true)]
      [string]$Name,
      [Parameter(Mandatory=$true)]
      [Alias("Host")]
      [string]$SshHost
    )
    $prev = $ErrorActionPreference
    $ErrorActionPreference = "Continue"
    Write-Host "Starting task '$Name' on host '$SshHost'"
    ssh $SshHost "schtasks /Run /TN `"$Name`""
    $ErrorActionPreference = $prev
}

function Stop-RemoteTask {
    Param(
      [Parameter(Mandatory=$true)]
      [string]$Name,
      [Parameter(Mandatory=$true)]
      [Alias("Host")]
      [string]$SshHost
    )
    $prev = $ErrorActionPreference
    $ErrorActionPreference = "Continue"
    Write-Host "Ending task '$Name' on server '$SshHost'"
    remote $SshHost "schtasks /End /TN `"$Name`""
    $ErrorActionPreference = $prev
}

function Test-ArchiveIntegrity($archive) {
    $result = & 7z t "$archive" 2>$null
    $result = [regex]::match($result,'Can''t open as archive: (\d+)').Groups[1].Value
    if($result -eq "1") {
        return $false
    } else {
        return $true
    }
}

function Push-EmphemeralLocation {
    param(
        [Parameter(Mandatory, Position=0, HelpMessage="The directory path to be tested, created, and relocated to.")]
        [String]$Path
    )
    if(-not $(Test-Directory $Path)) {
        New-Directory $Path
      }
    Push-Location $Path
}

function New-Directory {
    param(
        [Parameter(Mandatory,ValueFromPipeline,Position=0, HelpMessage="The path at which the new directory will be created recursively.")]
        [Alias('Paths')]
        [String[]] $Path
    )
    foreach($item in $Path) {
        if(-not $(Test-Path -Path $item -PathType Container)) {
            New-Item -Path $item -ItemType Directory -ErrorAction SilentlyContinue | Out-Null
        }
    }
}

function Test-Directory {
    param(
        [Parameter(Mandatory, ValueFromPipeline, Position=0, HelpMessage="The path of the expected directory.")]
        [String]$Path
    )
    process {
        Test-Path -Path $Path -PathType Container
    }
}

function Test-DirectoryEmpty {
    param(
        [Parameter(Mandatory, ValueFromPipeline, Position=0, HelpMessage="The path of the expected directory.")]
        [String] $Path
    )
    process {
        return -not (Test-DirectoryHasChildren -Path $Path)
    }
}

function Test-DirectoryHasChildren {
    param(
        [Parameter(Mandatory, ValueFromPipeline, Position=0, HelpMessage="The path of the expected directory.")]
        [String] $Path
    )
    process {
        $wildcard_path = $Path -replace '[\\\/]?\*?$','' -replace '$','\*'
        [bool] $has_children = Test-Path -Path $wildcard_path
        return $has_children
    }
}

function Get-7zKey([switch]$AllowUserInput) {
    $pass_src = [IO.Path]::Combine($PSScriptRoot, "key")
    if($(Test-Path -Path $pass_src)) {
        $pass = $((Get-Content $pass_src -Raw).Replace("`r`n","_nl_") | ConvertTo-SecureString -AsPlainText)
        return $pass
    } elseif($AllowUserInput) {
        $pass = Read-Host 'What is your password?' -AsSecureString
        return $pass
    } else {
        throw New-Object System.ArgumentException '7z encryption keyfile was not found.'
    }
}

function Get-7zCryptArgs([switch]$AllowUserInput) {
    $pass = Get-7zKey -AllowUserInput:$AllowUserInput | ConvertFrom-SecureString -AsPlainText
    $encryption_args = "-p""$pass"""
    return $encryption_args
}

function Compress-7zArchive {
    param(
        [Parameter(Mandatory)]
        [String] $ArchivePath,
        [Parameter(Mandatory)]
        [String] $SourcePath,
        [Switch] $Store = $false,
        [Switch] $Unencrypted = $false,
        [String[]] $Exclude = @()
    )
    process {
        New-Directory $([System.IO.Path]::GetDirectoryName($ArchivePath))
        $enc_args = if($Unencrypted) {""} else {Get-7zCryptArgs}
        Remove-Item -LiteralPath $ArchivePath -Force -Recurse -ErrorAction SilentlyContinue
    
        $CompressParams = @{
            Path = $SourcePath
            ArchiveFileName = $ArchivePath;
            Format = "SevenZip";
            CompressionLevel = "High";
            CompressionMethod = "Lzma2";
        }
        if($Store) {
            $CompressParams.CompressionLevel = "None";
        }
    
        if(-Not $Unencrypted) {
            $CompressParams.EncryptFilenames = $true
            $CompressParams.SecurePassword = $(Get-7zKey)
        }
        Compress-7Zip @CompressParams
    
        # TODO: Fork 7Zip4Powershell and add native exclusions.
        if($null -ne $Exclude) {
            foreach($ex in $Exclude) {
                7z d $CompressParams.ArchiveFileName "$ex" -r $enc_args
            }
        }
    }
}

function Compress-With7zCli {
    param(
        [Parameter(Mandatory)]
        [String] $ArchivePath,
        [Parameter(Mandatory)]
        [String] $SourcePath,
        [Switch] $Store = $false,
        [Switch] $Unencrypted = $false,
        [String[]] $Exclude = @()
    )
    New-Directory $([System.IO.Path]::GetDirectoryName($ArchivePath))
    $enc_args = ""
    if(-not $Unencrypted) {
        $enc_args = Get-7zCryptArgs
    }
    if($null -ne $Exclude) {
        for($i = 0; $i -lt $Exclude.Length; $i++) {
            $Exclude[$i] = '-xr!"' + $Exclude[$i] + '"'
        }
    }
    Remove-Item -LiteralPath $ArchivePath -Force -Recurse -ErrorAction SilentlyContinue
    # mmt: Number of CPU Threads
    # mhe: Encrypt Headers
    # t7z: Filetype=7z
    # mx<N>: Compression Level: 1=Store, 9=Ultra
    $7z_args = "-mmt=10 -mhe -t7z";
    if($Store) {
        $7z_args += " -mx0"
    }
    if(-not $Unencrypted) {
        $enc_args = Get-7zCryptArgs
        $7z_args += $enc_args
    }
    $source_paths = $($SourcePaths | % { "`"$_`"" }) -Join " "
    $command = "7z a $7z_args $Exclude `"$ArchivePath`" $source_paths"
    Write-Host "Executing: $command"
    Invoke-Expression -Command $command | Out-Null
    $archive = Get-Item $ArchivePath
    return $archive
}

function Expand-7zArchive($ArchivePath, $TargetPath, [switch]$Unencrypted, [switch]$PruneRoot) {
    $enc_args = ""
    if(-not $Unencrypted) {
        $enc_args = Get-7zCryptArgs
    }
    # e: extract, o: output directory
    $prune_args = ""
    if($PruneRoot) {
        $prune_args = "-spe"
    }
    # spe: remove the archived root folder during extraction so that contents are extracted directly to the intended directory
    # aoa: always overwrite all (without prompt)
    7z x $enc_args $prune_args -aoa -o"$TargetPath" "$ArchivePath"
    $target = Get-Item $TargetPath
    return $target
}

function Resolve-RealPath {
    <#
        .SYNOPSIS
        Implementation of Unix realpath().

        .PARAMETER Path
        Must exist
    #>
    [CmdletBinding()]
    [OutputType([string])]
    param(
        [Parameter(Position = 0, Mandatory, ValueFromPipeline, ValueFromPipelineByPropertyName)]
        [Alias('FullName')]
        [string] $Path
    )

    process {
        if ($PSVersionTable.PSVersion.Major -lt 6 -or $IsWindows) {
            return [IO.Path]::GetFullPath($Path)
        }

        [string[]] $parts = ($Path.TrimStart([IO.Path]::DirectorySeparatorChar).Split([IO.Path]::DirectorySeparatorChar))
        [string] $realPath = ''
        foreach ($part in $parts) {
            $realPath += [string] ([IO.Path]::DirectorySeparatorChar + $part)
            $item = Get-Item $realPath
            if ($item.Target) {
                $realPath = $item.Target
            }
        }
        $realPath
    }
}
function Split-GitRemote {
    [CmdletBinding()]
    [OutputType([System.Uri])]
    param(
        [Parameter(Mandatory=$false, HelpMessage="The remote URI to be split into its component parts.")]
        [String]$RemoteUri = [NullString]::Value,
        [Parameter(Mandatory=$false, HelpMessage="The name of the remote which will be fed into 'git config' to retrieve its URI.")]
        [String]$RemoteName = "Origin"
    )
    if([String]::IsNullOrWhiteSpace($RemoteUri)) {
        $RemoteName = $($(git remote) | ? { $_ -eq $RemoteName })
        $RemoteUri = $(git config --get remote.$RemoteName.url)
    }
    if($RemoteUri -match '^git@\w+(\.\w+){1,}:') {
        # System.Uri doesn't recognize the SSH-SCP formatting frequently used for 
        # Git repositories. Instead of parsing the URI directly, we transform it
        # into a valid SSH uri.
        $RemoteUri = $RemoteUri -replace ':','/'
        $RemoteUri = "ssh://$RemoteUri"
    }
    if($RemoteUri -match '^(https?|ssh)://') {
        # If the remote URI begins with an expected scheme we assume it adheres to formatting conventions
        # that are recognized by System.Uri.
        $parts = [System.Uri]$RemoteUri
        return $parts
    }
    if($null -eq $RemoteName) {
        New-Object System.ArgumentException "The remote URI '$RemoteUri' could not be parsed by [System.Uri]."
    } else {
        New-Object System.ArgumentException "The URI for remote '$RemoteName' could not be parsed by [System.Uri]: $RemoteUri"
    }
}

function Resolve-RepoRoot {
    [CmdletBinding()]
    [OutputType([System.String])]
    param(
        [Parameter(Mandatory=$true, Position=0, HelpMessage="The relative path or leaf name of the repository defined by its remote:origin URI.")]
        [String]$RepoName,
        [Parameter(Mandatory=$false, HelpMessage="The path of the repository root itself or one of its descendant file system entries.")]
        [String[]]$SearchPaths = $null,
        [Parameter(Mandatory=$false, HelpMessage="The local name of the remote Git repository that should be used for URI resolution.")]
        [String]$RemoteName = "Origin"
    )
    if($RepoName -notmatch '\.git$') {
        $RepoName += '.git'
    }
    $SearchPaths += @($PWD, $PSScriptRoot)
    try {
        foreach($path in $SearchPaths) {
            Push-Location $path
            While($true) {
                $repoRoot = $(git rev-parse --show-toplevel)
                if($null -eq $repoRoot) {
                    break
                }
                if([String]::IsNullOrWhiteSpace($RepoName)) {
                    return $repoRoot
                }
                $remoteData = $(Split-GitRemote -RemoteName $RemoteName)
                $segs = $remoteData.Segments
                $subpath = [String]::Empty
                for($i = $segs.Length - 1; $i -gt 0; $i--) {
                    $subpath = "$($segs[$i])$subpath"
                    if($RepoName -eq $subpath) {
                        return $repoRoot
                    }
                }
                Pop-Location
                Push-Location "$repoRoot/.."
            }
        }
        throw New-Object System.Exception "No matching repository found; Name: $RepoName; Remote: $RemoteName; Starting Paths: $paths"
    }
    finally {
        Pop-Location
    }
}

function ConvertTo-WebPath($LocalPath, $Root) {
    Push-Location $Root
    $web_path = ((Resolve-Path -Path $LocalPath -Relative) -replace '\.?\\+','/') -replace '^/+',''
    Pop-Location
    return $web_path
}

function ConvertTo-LocalPath($WebPath, $Root) {
    $local_path = $(Join-Path -Path "$Root" -ChildPath $WebPath)
    return $local_path
}

function Select-LatestLocalArchive($Instance, $Tier) {
    $source_path = [System.IO.Path]::Combine($global:BackupRoot, $Tier, $Instance)
    $archive_filter = $(Join-Path -Path $source_path -ChildPath '*.7z')
    $archives = $(Get-ChildItem $archive_filter -Recurse)
    $parsed = $archives | Foreach-Object { @{ File = $_; Timestamp = $(Get-Timestamp $_.FullName);} }
    $latest = $parsed | Sort-Object { $_.Timestamp } -Descending | Select-Object -First 1
    return $latest.File
}

function Test-CommandExists {
  [OutputType([bool])]
	[CmdletBinding()]
	param($Name)

	$oldPreference = $ErrorActionPreference
	Write-Verbose "Previous ErrorActionPreference: $ErrorActionPreference"
	$ErrorActionPreference = "Stop"
	Write-Verbose "Set ErrorActionPreference to 'Stop'"

	try {
		$command=Get-Command $Name
		Write-Verbose "Command Exists $command"
		return $true
	}
	catch {
		Write-Verbose "Command $Name does not exist" 
		return $false
	}
	finally {
		$ErrorActionPreference=$oldPreference
		Write-Verbose "Restore ErrorActionPreference: $ErrorActionPreference"
	}
}

function Assert-AdminAccess {
	Add-Type -AssemblyName System.Security.Principal.Windows
	$principal = New-Object WindowsPrincipal([WindowsIdentity]::GetCurrent())
	$hasAdmin = $principal.IsInRole([WindowsBuiltInRole]::Administrator)
	if(-Not $hasAdmin) {
		Write-Host "This script requires Administrator privileges. Re-launch as Administrator to continue."
		exit
	}
}

function Start-Log($Project, $Descriptor) {
    if($null -eq $Project) {
        $Project = $(Get-EntryScriptName)
    }
    $timestamp = $(New-Timestamp)
    $filename = "$timestamp-$Descriptor.log"
    if([System.String]::IsNullOrWhiteSpace($Descriptor)) {
        $filename = "$timestamp.log"
    }
    $path = [System.IO.Path]::Combine($global:LogRoot, $Project, $filename)
    Start-Transcript -Path $path
}

function Stop-Log() {
    Stop-Transcript
}

function Request-Confirmation {
    param(
        [Parameter(Mandatory=$true,ValueFromPipeline=$true)]
        [String]$Message
    )
    process {
        Write-Host -ForegroundColor Yellow -Object "$Message " -NoNewline
        $key = $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
        if(0x4E -eq $key.VirtualKeyCode) {
            return $false
        } else {
            return $true
        }
        return $key
    }
}

<#
.SYNOPSIS
    Generates a password comprised of a parameterized number of dictionary words 
    concatenated into a space-delimited string. Available in English (en) or 
    German (de).

.NOTES
    Inspired by the XKCD comic #936: "Password Strength"
        Publication: https://xkcd.com/936/
        Direct Link: https://imgs.xkcd.com/comics/password_strength.png

.LINK
    Original Source Code: https://janikvonrotz.ch/2017/08/11/xkcd-powershell-password-generator/
#>
function Get-XkcdPassword {
    [CmdletBinding()]
    param(
        [int] $WordCount = 4,
        [Nullable[int]] $MinWordLength = 6,
        [Nullable[int]] $MaxWordLength = $null,
        [String] $Delimiter = ' ',
        [String] $DictionaryPath = [NullString]::Value
    )
    if($null -eq $DictionaryPath) {
        $DictionaryPath = [System.IO.Path]::Combine($PSScriptRoot, 'words-en.txt')
    }
    $words = @((Get-Content $DictionaryPath) -split "`n")

    if($null -ne $MinWordLength) {
        $words = @($words | ? { $_.Length -ge $MinWordLength })
    }
    if($null -ne $MaxWordLength) {
        $words = @($words | ? { $_.Length -le $MaxWordLength })
    }

    if($words.Length -eq 0) {
        if($ErrorActionPreference -eq [ActionPreference]::Stop) {
            throw "Error generating password: No words meet the specified criteria (word length between $MinWordLength and $MaxWordLength)"
        } else {
            return $null
        }
    }

    $parts = @(1..$WordCount | % { (Get-Random $words).Trim() })
    $password = $parts -Join $Delimiter
    return $password
}

$global:ProcessMessages = @()
$global:ProcessColors = @(
    [ConsoleColor]::Blue;
    [ConsoleColor]::Green;
    [ConsoleColor]::Cyan;
    [ConsoleColor]::Magenta;
);
function Write-FunctionBegin {
    param(
        [Parameter(Mandatory, ValueFromPipeline, Position = 0)]
        [String] $Message
    )
    begin {
        $pms = $global:ProcessMessages
        if($pms.Length -eq 0) {
            $empty_line = $null
        } else {
            $max_len = ($pms | Measure-Object -Property Length -maximum).maximum
            $empty_line = (' ' * ($max_len + 20))
        }
    }
    process {
        $fgc = $global:ProcessColors[$global:ProcessMessages.Length]
        $bgc = [Console]::BackgroundColor
        $global:ProcessMessages += $Message
        $msg = $Message -replace '\.*$','' -replace '$','...'
        if($null -ne $empty_line) {
            Write-Host "$empty_line`r" -NoNewline
        }
        Write-Host "$msg`r" -ForegroundColor $fgc -BackgroundColor $bgc -NoNewline
    }
}

function Write-FunctionEnd {
    begin {
        $pms = $global:ProcessMessages
        $max_len = ($pms | Measure-Object -Property Length -maximum).maximum
        $empty_line = (' ' * ($max_len + 20))
    }
    process {
        $msg = ($pms[($pms.Length - 1)]) -replace '\.*$','' -replace '$','...completed.'
        if ($pms.Length -eq 0) {
            Write-Error 'Called Write-FunctionEnd without a matching Write-FunctionBegin'
            exit
        }
        elseif ($pms.Length -eq 1) {
            $pms = @()
        }
        else {
            $pms = $pms[0..($pms.Length - 2)]
        }
        $bgc = [Console]::BackgroundColor
        $fgc = $global:ProcessColors[$pms.Length]
        Write-Host "$empty_line`r" -NoNewline
        Write-Host $msg -ForegroundColor $fgc -BackgroundColor $bgc
    }
    end {
        $global:ProcessMessages = $pms
    }
}

Export-ModuleMember -Function *
