. "$PSScriptRoot\..\init.ps1" # Initialize environment and GM admin framework

$SqlDirectories = @(
    "$global:SolutionRoot\SolutionData\Temp";
    "$global:SolutionRoot\SolutionData\Backups";
    "$global:SolutionRoot\GmWeb.Common";
    "$global:SolutionRoot\GmWeb.Db.Common";
);
$DirectoryLinks = @()
foreach($dir in $SqlDirectories) {
    $item = $(Get-Item $dir)
    if([String]::IsNullOrWhiteSpace($item.LinkTarget)) {
        continue
    }
    $DirectoryLinks += $item.LinkTarget
}
$SqlDirectories += $DirectoryLinks
Grant-SQLFileAccess -Server localhost -Paths $SqlDirectories
