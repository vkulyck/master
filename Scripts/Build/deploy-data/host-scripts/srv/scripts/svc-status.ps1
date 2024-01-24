#!/usr/bin/env pwsh
. "$PSScriptRoot/init.ps1"

Invoke-SystemdAction -Action Status -ServiceIds $global:ServiceIds