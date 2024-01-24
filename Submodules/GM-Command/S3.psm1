using module ./Utility.psm1
$script:creds = $global:AwsCredentials

function Push-LatestS3Archive {
    param(
        [Parameter(Mandatory=$true)]
        [string]$Bucket,
        [Parameter(Mandatory=$true)]
        [string]$Instance,
        [Parameter(Mandatory=$true)]
        [string]$Tier
    )
    $archive = Select-LatestLocalArchive -Instance $Instance -Tier $Tier
    $archive_path = $archive.FullName
    $rel_path = ConvertTo-WebPath -LocalPath $archive_path -Root $global:DataRoot
    Write-S3Object -BucketName $Bucket -File $archive_path -Key $rel_path @script:creds
}

function Get-S3Archive {
    param(
        [Parameter(Mandatory=$true)]
        [string]$Bucket,
        [Parameter(Mandatory=$true)]
        [string]$ArchiveKey
    )
    $archive_path = ConvertTo-LocalPath -WebPath $ArchiveKey -Root $global:DataRoot
    if(Test-LocalCopy -LocalPath $archive_path -RemotePath $ArchiveKey -Bucket $Bucket) {
        # Do nothing
        Write-Host "Skipping S3 download - File exists: $archive_path"
    } else {
        Read-S3Object -BucketName $Bucket -Key $ArchiveKey -File $archive_path @script:creds | Out-Null
    }
    $archive = Get-Item $archive_path
    return $archive
}

function Get-LatestS3Archive {
    param(
        [Parameter(Mandatory=$true)]
        [string]$Bucket,
        [Parameter(Mandatory=$true)]
        [string]$Instance,
        [Parameter(Mandatory=$true)]
        [string]$Tier
    )
    $remote_archive = Select-LatestS3Archive -Bucket $Bucket -Instance $Instance -Tier $Tier
    $archive_key = $remote_archive.Key
    $local_archive = Get-S3Archive -Bucket $Bucket -ArchiveKey $archive_key
    return $local_archive
}

function Get-LatestLocalArchive {
    param(
        [Parameter(Mandatory=$true)]
        [string]$Instance,
        [Parameter(Mandatory=$true)]
        [string]$Tier
    )
    $local_archive = Select-LatestLocalArchive -Instance $Instance -Tier $Tier
    return $local_archive
}

function Select-LatestS3Archive {
    param(
        [Parameter(Mandatory=$true)]
        [string]$Bucket,
        [Parameter(Mandatory=$true)]
        [string]$Instance,
        [Parameter(Mandatory=$true)]
        [string]$Tier
    )
    Import-Module AWS.Tools.S3
    $archive_key_prefix = "Backups/$Tier/$Instance"
    [Amazon.S3.Model.S3Object[]]$results = $(Get-S3Object -BucketName $Bucket -Prefix $archive_key_prefix @script:creds)
    foreach($result in $results) {
        $result.LastModified = $(Get-Timestamp -Value $result.Key)
    }
    $results = $results | Sort-Object -Property LastModified -Descending
    return $results[0]
}

function Get-FileHashMD5 {
    param(
        [Parameter(Mandatory=$true)]
        [string]$Path
    )
    $md5 = New-Object -TypeName System.Security.Cryptography.MD5CryptoServiceProvider

    $blocksize = (1024*1024*5)
    $startblocks = (1024*1024*16)
    $lines = 0
    [byte[]] $binHash = @()

    $reader = [System.IO.File]::Open($Path,"OPEN","READ")
    
    if ((Get-Item $Path).length -gt $startblocks) {
        $buf = New-Object byte[] $blocksize
        while (($read_len = $reader.Read($buf,0,$buf.length)) -ne 0){
            $lines   += 1
            $binHash += $md5.ComputeHash($buf,0,$read_len)
        }
        $binHash=$md5.ComputeHash( $binHash )
    }
    else {
        $lines   = 1
        $binHash += $md5.ComputeHash($reader)
    }

    $reader.Close()
    
    $hash = [System.BitConverter]::ToString( $binHash )
    $hash = $hash.Replace("-","").ToLower()

    if ($lines -gt 1) {
        $hash = $hash + "-$lines"
    }

    return $hash
}

function Test-LocalCopy {
    param(
        [Parameter(Mandatory=$true)]
        [string]$LocalPath,
        [Parameter(Mandatory=$true)]
        [string]$RemotePath,
        [Parameter(Mandatory=$true)]
        [string]$Bucket
    )
    Import-Module AWS.Tools.S3
    try {
        $remote_metadata =  $(Get-S3ObjectMetadata -BucketName $Bucket -Key $RemotePath @script:creds)
        # Get remote hash with quotations removed
        $remote_hash = $remote_metadata.ETag.Substring(1, $remote_metadata.ETag.Length - 2)
        $local_hash = $(Get-FileHashMD5 -Path $LocalPath)
        return ($local_hash -eq $remote_hash)
    }
    catch {
        Write-Host $_
        return $false
    }
}

Export-ModuleMember Test-LocalCopy, Push-LatestS3Archive, Get-S3Archive, Get-LatestS3Archive, Get-LatestLocalArchive, Select-LatestS3Archive
