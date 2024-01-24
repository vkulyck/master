. "$PSScriptRoot\..\init.ps1" # Initialize environment and GM admin framework

Start-Log "restore-local-dbs"

$withS3 = $false
$env_xfers = [ordered]@{ "Staging" = ""; }
$catalogs = @( "GmCommon"; "GmIdentity"; )

foreach($source_env in $env_xfers.Keys) {
    foreach($catalog in $catalogs) {
        $target_env = $env_xfers[$source_env]
        $source_instance = "$source_env.$catalog" -Replace '^\.|\.$',''
        $target_instance = "$target_env.$catalog" -Replace '^\.|\.$',''
        if($withS3) {
            $Archive = Get-LatestS3Archive -Instance $source_instance -Tier DB
        } else {
            $Archive = Get-LatestLocalArchive -Instance $source_instance -Tier DB
        }
        Write-Host "Restoring $target_instance from $source_instance archive: $Archive" -ForegroundColor Yellow
        Restore-DbCatalog -Catalog $target_instance -Archive $Archive
    }
}

Stop-Log
