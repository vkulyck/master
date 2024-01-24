Push-Location $PSScriptRoot
$global:SolutionRoot = "$(Resolve-Path $(git rev-parse --show-toplevel))"
$global:EnvironmentInitialized = $false
Pop-Location
. "$global:SolutionRoot/Submodules/GM-Command/Init.ps1"