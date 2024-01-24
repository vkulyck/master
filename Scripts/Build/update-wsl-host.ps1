#Requires -RunAsAdministrator
. "$PSScriptRoot/../init.ps1"

if(-Not $(Test-WslHostsMapping)) {
  Update-WslHostsMapping
}

if(-Not $(Test-WslSshStatus)) {
  Start-WslSsh
}