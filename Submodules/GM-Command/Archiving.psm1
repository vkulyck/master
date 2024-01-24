function Restore-Website($Website, $Archive, [switch]$Unencrypted) {
    $target_path = "$($global:WebDeployRoot)\$Website"
    Expand-7zArchive -ArchivePath $Archive.FullName -TargetPath $target_path -PruneRoot $true -Unencrypted:$Unencrypted
}

function Restore-DbCatalog($Catalog, $Archive, [switch]$KeepTempFiles, [switch]$Unencrypted) {
    $backup_name = $Archive.Name -replace '\.7z$','.bak'
    $temp_dir = New-TempDirectory
    $temp_dir.Persist = $KeepTempFiles
    Invoke-Disposable ($temp_dir) {
        $temp_backup_path = [System.IO.Path]::Combine($temp_dir, $backup_name)
        Expand-7zArchive -ArchivePath $Archive.FullName -TargetPath $temp_dir -PruneRoot $true -Unencrypted:$Unencrypted
        if(-Not $(Test-Path $temp_backup_path)) {
            $archive_contents = Get-ChildItem "$temp_dir\*.bak" -Recurse
            if($archive_contents.Count -gt 1) {
                throw New-Object System.ArgumentException "Too many backups found in expanded archive: $Archive"
            } elseif($archive_contents.Count -eq 0) {
                throw New-Object System.ArgumentException "No backups found in expanded archive: $Archive"
            }
            $temp_backup_path = $archive_contents[0].FullName
        }
        Restore-Catalog -Server $global:SqlServer -Catalog $Catalog -Backup $temp_backup_path -Overwrite
    }
}

function Backup-Repository {
    param(
        [Parameter(Mandatory=$true, Position=0, HelpMessage="The name of the repository to be archived, as defined by the path component of its remote URL.")]
        [Alias("Name")]
        [String]$RepoName,
        [Parameter(Mandatory=$false, HelpMessage="The path from which to begin the recursively ascending repository search.")]
        [Alias("Paths")]
        [String[]]$SearchPaths,
        [Parameter(Mandatory=$false, HelpMessage="Enable this flag to skip the encryption step during archiving.")]
        [Switch]$Unencrypted = $false
    )
    $Timestamp = "$(New-Timestamp)"
    $source_path = $(Resolve-RepoRoot $RepoName -SearchPaths $SearchPaths)
    $archive_name = "$Timestamp-$Env:COMPUTERNAME-Repo-$RepoName.7z"
    $archive_path = [System.IO.Path]::Combine($global:BackupRoot, 'Repo', $RepoName, $archive_name)
    $Exclusions = @("bin", "obj", "packages", ".vs")
    $Exclusions += @("Publish", "SolutionData", "Submodules")
    $Exclusions += @("*.dbmdl")
    Compress-7zArchive -ArchivePath $archive_path -SourcePath $source_path -Exclude $Exclusions -Unencrypted:$Unencrypted
    $archive = Get-Item $archive_path
    return $archive
}

function Backup-Website {
    param(
        [Parameter(Mandatory=$true, Position=0, HelpMessage="The name of the website to be archived.")]
        [String]$Website,
        [Parameter(Mandatory=$false, HelpMessage="Enable this flag to include configuration files in the backup archive.")]
        [Switch]$KeepConfigs = $false,
        [Parameter(Mandatory=$false, HelpMessage="Enable this flag to skip the encryption step during archiving.")]
        [Switch]$Unencrypted = $false,
        [Parameter(Mandatory=$false, HelpMessage="Enable this flag to disable compression and simply store the source files in a 7z container.")]
        [Switch]$Store = $false
    )
    $Timestamp = "$(New-Timestamp)"
    $source_path = [System.IO.Path]::Combine($global:WebDeployRoot, $Website)
    $archive_name = "$Timestamp-$Env:COMPUTERNAME-Web-$Website.7z"
    $archive_path = [System.IO.Path]::Combine($global:BackupRoot, 'Web', $Website, $archive_name)
    $exclude = if($KeepConfigs) {@()} else {@(,"appsettings*.json")}

    Compress-7zArchive `
        -ArchivePath $archive_path -SourcePath $source_path `
        -Unencrypted:$Unencrypted -Store:$Store `
        -Exclude $exclude
    $archive = Get-Item $archive_path
    return $archive
}

function Backup-DbCatalog($Catalog, [switch]$Unencrypted) {
    $Timestamp = New-Timestamp
    $archive_name = "$Timestamp-$Env:COMPUTERNAME-DB-$Catalog.7z"
    
    if([String]::IsNullOrWhitespace($global:BackupDirectoryOverride)) {
        $archive_directory = [System.IO.Path]::Combine($global:BackupRoot, 'DB', $Catalog)
    } else {
        $archive_directory = [System.IO.Path]::Combine($global:BackupRoot, 'DB', $global:BackupDirectoryOverride)
    }
    $archive_path = [System.IO.Path]::Combine($archive_directory, $archive_name)
    $temp_backup_directory = New-TempDirectory
    Invoke-Disposable ($temp_backup_directory) {
        $backup_name = $archive_name -replace '\.7z$','.bak'
        $temp_backup_path = [System.IO.Path]::Combine($temp_backup_directory, $backup_name)
        Backup-Catalog -Server $global:SqlServer -Catalog $Catalog -BackupPath $temp_backup_path | Out-Null
        Compress-7zArchive -ArchivePath $archive_path -SourcePath $temp_backup_path -Exclude @("*.config") -Unencrypted:$Unencrypted
    }
    Write-Output $archive_path
}

Export-ModuleMember -Function *
