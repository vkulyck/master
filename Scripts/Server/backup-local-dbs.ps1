. "$PSScriptRoot\..\init.ps1" # Initialize environment and GM admin framework

$withS3 = $false
$environments = @( ""; )
$catalogs = @( "GmCommon"; "GmIdentity"; )

Start-Log "backup-local-dbs"

foreach($env in $environments) {
    foreach($catalog in $catalogs) {
        $instance = @($env; $catalog;) -Join '.' -Replace '^\.|\.$',''
        Write-Host "Performing backup: $instance" -ForegroundColor Yellow
        Start-Sleep -Seconds 1
        Backup-DbCatalog -Catalog $instance
        if($withS3) {
            Push-LatestS3Archive -Bucket "webdrive.staging.goodmojo.us" -Instance $instance -Tier DB
        }
    }
}

Stop-Log