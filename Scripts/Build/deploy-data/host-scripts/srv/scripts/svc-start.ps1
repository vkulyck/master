#!/usr/bin/env pwsh
. "$PSScriptRoot/init.ps1"

Invoke-SystemdAction -Action Stop -ServiceIds $global:ReverseServiceIds
Invoke-SystemdAction -Action Start -ServiceIds $global:ServiceIds