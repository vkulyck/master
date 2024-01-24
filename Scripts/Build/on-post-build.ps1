param(
  [string]$SolutionDirectory,
  [string]$TargetDirectory,
  [string]$ProjectDirectory,
  [string]$ProjecPath,
  [string]$ProjectName
)

. $PSScriptRoot/sync-solution-appsettings.ps1 `
  -SolutionDirectory $SolutionDirectory `
  -TargetDirectory $TargetDirectory `
  -ProjectDirectory $ProjectDirectory

. $PSScriptRoot/sync-lfs-asset.ps1 `
  -SolutionDirectory $SolutionDirectory `
  -Asset wwwroot
# Post-Build Event [Always]:
# pwsh.exe "$(SolutionDir)Scripts\Build\on-post-build.ps1" -SolutionDirectory "$(SolutionDir)\" -TargetDirectory "$(TargetDir)\" -ProjectDirectory "$(ProjectDir)\" -ProjectPath "$(ProjectPath)" -ProjectName "$(ProjectName)"