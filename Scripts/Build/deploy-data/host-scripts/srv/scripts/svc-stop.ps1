#!/usr/bin/env pwsh
. "$PSScriptRoot/init.ps1"

Invoke-SystemdAction -Actions Stop -ServiceIds $global:ReverseServiceIds