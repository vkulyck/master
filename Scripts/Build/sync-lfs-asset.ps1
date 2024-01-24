Param(
  [Parameter(Mandatory=$true)]
  [String]$SolutionDirectory,
  [Parameter(Mandatory=$true)]
  [String]$Asset,
  [Switch]$ReArchive = $false,
  [Switch]$NoRevalidate = $false
)

$global:AssetsRoot = "$SolutionDirectory/SolutionData/Assets"
. "$PSScriptRoot/../init.ps1"

$AssetDirectory = "$global:AssetsRoot/$Asset"
$AssetArchive = "$global:AssetsRoot/$Asset.7z"
$AssetTimestamp = "$AssetDirectory/.timestamp"

Write-Information "Validating asset: $Asset"
$ExtractedAt = $(Get-Item $AssetTimestamp -ErrorAction Ignore).LastWriteTime
$ArchivedAt = $(Get-Item -Path $AssetArchive -ErrorAction Ignore).LastWriteTime

Write-Information "Asset $Asset was last extracted at $ExtractedAt, and was last archived at $ArchivedAt."
if($null -ne $ExtractedAt -and ($null -eq $ArchivedAt -or $ArchivedAt -lt $ExtractedAt)) {
  Write-Information "Asset $Asset is fresh"
  if($ReArchive) {
    Remove-Item $AssetArchive -ErrorAction Ignore | Out-Null
    Compress-With7zCli -ArchivePath $AssetArchive -SourcePath $AssetDirectory -Unencrypted
  }
  exit
}

Write-Information "Asset $Asset is stale; decompressing asset archive."
Remove-Item -Path $AssetDirectory -Recurse -ErrorAction Ignore | Out-Null
New-Directory $AssetDirectory | Out-Null
Expand-7zArchive -ArchivePath $AssetArchive -TargetPath $AssetDirectory -PruneRoot $true -Unencrypted
$null > $AssetTimestamp

Write-Information "Asset $Asset has been extracted; re-running asset verification."
if(-not $NoRevalidate) {
  . $MyInvocation.MyCommand.Path -SolutionDirectory $SolutionDirectory -Asset $Asset -NoRevalidate
}