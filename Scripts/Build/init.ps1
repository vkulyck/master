Push-Location $PSScriptRoot
$global:SolutionRoot = "$(Resolve-Path $(git rev-parse --show-toplevel))"
. "./GM-Command/Init.ps1"
Pop-Location