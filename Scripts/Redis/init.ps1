Push-Location $PSScriptRoot
$global:SolutionRoot = "$(Resolve-Path $(git rev-parse --show-toplevel))"
Pop-Location
. "$global:SolutionRoot/Submodules/GM-Command/Init.ps1"