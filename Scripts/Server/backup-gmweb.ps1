. "$PSScriptRoot\..\Init.ps1" # Initialize environment and GM admin framework

Start-Log "gmweb"

Backup-Repository -RepoName gmweb

Stop-Log